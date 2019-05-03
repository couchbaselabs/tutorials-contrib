module.exports = {
    OnUpdate: OnUpdate,
    IngestUserDelivery: IngestUserDelivery,
    IngestLoyaltyMember: IngestLoyaltyMember,
    PollOnlineCrm: PollOnlineCrm
};

// source bucket: staging
// metadata bucket: customer360_metadata
// function name: ingestionCustomer360
// description: ingest data from staging and create/update customer 360 documents in customer360 bucket
// bindings:
//	Alias, customer360, cust

function OnUpdate(doc, meta) {
	// when an email comes along, save it here for later
	// to poll the Online CRM API
	var emailToPollOnlineCRM = '';
	
	if(IsUserDelivery(doc)) {
        emailToPollOnlineCRM = IngestUserDelivery(doc, cust, log);
	}

	if(IsLoyaltyMember(doc)) {
        emailToPollOnlineCRM = IngestLoyaltyMember(doc, cust, log);
	}

	if(emailToPollOnlineCRM) {
        PollOnlineCrm(doc, emailToPollOnlineCRM, cust, curl, log);
	}
}

function IngestUserDelivery(doc, cust, log)
{
    log('INGEST', 'User Delivery System');
		
    var email = doc.payload.after.email;

    var customerDoc = GetCustomer360DocIfItExists(cust, email);
    if(!customerDoc) {
        customerDoc = {};
    }
    customerDoc = UpdateCustomerDocFromUserDelivery(customerDoc, doc);
    
    cust[email] = customerDoc;

    return email;
}

function IngestLoyaltyMember(doc, cust, log)
{
    log('INGEST', 'Loyalty System');
		
    var email = doc.email;
    
    var customerDoc = GetCustomer360DocIfItExists(cust, email);
    if(!customerDoc) {
        customerDoc = {};
    }
    customerDoc = UpdateCustomerDocFromLoyalty(customerDoc, doc);
    
    cust[email] = customerDoc;
    
    return email;
}

function PollOnlineCrm(doc, email, cust, curl, log)
{
    var customerDoc = GetCustomer360DocIfItExists(cust, email);
    
    customerDoc = UpdateCustomerDocFromOnlineStore(customerDoc, email, cust, curl, log);

//	ImportNewOrders(cust, customerDoc.Id, curl, log);
}

// this function will examine a document
// and determine if it's being ingested from
// the user delivery system
function IsUserDelivery(doc) {
    if(doc.payload)
      if(doc.payload.source)
        if(doc.payload.source.name)
            return doc.payload.source.name === 'dbserver1';
    return false;
  }

// get a customer 360 document
// if it exists
// otherwise return null
function GetCustomer360DocIfItExists(bucket, email) {
    try
    {
        if(email != null)
        {
            return bucket[email];
        }
    }
    catch (e)
    {
        log('Error GetCustomer360DocIfItExists :', email,'; Exception:', e);
    }
    return null;
}

// given a customer360 document and an incoming user delivery document
// update the customer 360 document
// creating a xref if necessary
function UpdateCustomerDocFromUserDelivery(customerDoc, userDeliveryDoc) {
	var payloadAfter = userDeliveryDoc.payload.after;
	customerDoc.type = "user";
	if(!customerDoc.profile)
		customerDoc.profile = {};
	customerDoc.profile.name = payloadAfter.name;
	customerDoc.profile.address1 = payloadAfter.address_line_1;
	if(payloadAfter.address_line_2)
		customerDoc.profile.address2 = payloadAfter.address_line_2;
	customerDoc.profile.phone = payloadAfter.phonenumber;
	customerDoc.profile.state = payloadAfter.state;
	customerDoc.profile.city = payloadAfter.city;
	
	if(!customerDoc.xrefs)
		customerDoc.xrefs = [];
	var createNewXref = true;
	for(var i=0; i< customerDoc.xrefs.length; i++)
	{
        var xref = customerDoc.xrefs[i];
		if(xref.external_system == "home_delivery") {
			xref.external_ID = payloadAfter.id;
			xref.pass = payloadAfter.password;
			createNewXref = false;
		}
	}
	if(createNewXref) {
		var xref = {
			external_system: "home_delivery",
			external_ID: payloadAfter.id,
			pass: payloadAfter.password
		};
		customerDoc.xrefs.push(xref);
	}
	return customerDoc;
}

// this function will examine a document
// and determine if it's being ingested from
// the loyalty system
function IsLoyaltyMember(doc) {
	if(doc.points)
		return true;
	return false;
}

// given a customer360 document and an incoming loyalty document
// update the customer 360 document
// creating a xref if necessary
function UpdateCustomerDocFromLoyalty(customerDoc, loyaltyMemberDoc) {
	customerDoc.type = "user";
	if(!customerDoc.profile)
		customerDoc.profile = {};
	customerDoc.profile.name = loyaltyMemberDoc.firstName + " " + loyaltyMemberDoc.lastName;
	customerDoc.loyaltyPoints = loyaltyMemberDoc.points;
	
	if(!customerDoc.xrefs)
		customerDoc.xrefs = [];
    var createNewXref = true;
	for(var i=0; i< customerDoc.xrefs.length; i++)
	{
        var xref = customerDoc.xrefs[i];
		if(xref.external_system == "loyalty") {
			xref.external_ID = loyaltyMemberDoc.id;
			xref.pass = loyaltyMemberDoc.password;
			createNewXref = false;
		}
	}
	if(createNewXref) {
		var xref = {
			external_system: "loyalty",
			external_ID: loyaltyMemberDoc.id,
			pass: loyaltyMemberDoc.password
		};
		customerDoc.xrefs.push(xref);
	}
	return customerDoc;
}

function UpdateCustomerDocFromOnlineStore(customerDoc, email, cust, curl, log)
{
	var onlineStoreCustomerDetailsUrl = "http://172.17.0.8/api/onlineStore/getCustomerDetailsByEmail?customerEmail=" + email;
	var onlineStoreCustomerDetails = curl(onlineStoreCustomerDetailsUrl, { "method" : "GET"});

	customerDoc.profile.name = onlineStoreCustomerDetails.Name;
	if(!customerDoc.xrefs)
		customerDoc.xrefs = [];
	var createNewXref = true;
	for(var i=0; i< customerDoc.xrefs.length; i++)
	{
        var xref = customerDoc.xrefs[i];
		if(xref.external_system == "onlineStore") {
			log('INFO', 'Updating existing xref ' + email);
			xref.external_ID = onlineStoreCustomerDetails.Id;
			xref.pass = onlineStoreCustomerDetails.Password;
			createNewXref = false;
		}
	}
	if(createNewXref) {
		log('INFO', 'Creating a new xref ' + email);
		var xref = {
			external_system: "onlineStore",
			external_ID: onlineStoreCustomerDetails.Id,
			pass: onlineStoreCustomerDetails.Password
		};
		customerDoc.xrefs.push(xref);
	} else {
		log('INFO', 'Not creating a new xref ' + email);
	}

	// pull in the orders too
	var ordersUrl = "http://172.17.0.8/api/onlineStore/getOrdersByCustomerId?customerId=" + onlineStoreCustomerDetails.Id;
	var orders = curl(ordersUrl, { "method" : "GET"});

	cust[email] = customerDoc;

	for(var i=0;i<orders.length;i++)
	{
		var order = orders[i];
		cust[order.OrderId] = orders[i];
	}
}