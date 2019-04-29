// source bucket: staging
// metadata bucket: customer360_metadata
// function name: ingestionCustomer360
// description: ingest data from staging and create/update customer 360 documents in customer360 bucket
// bindings:
//	Alias, customer360, cust

function OnUpdate(doc, meta) {
	// when an email comes along, save it here for later
	var emailToPollOnlineCRM = '';
	
	// is this coming from the user delivery system?
    if(IsUserDelivery(doc)) {
      log('INGEST', 'User Delivery System');
      
      var email = doc.payload.after.email;

      var customerDoc = GetCustomer360DocIfItExists(email);
      if(!customerDoc)
        customerDoc = {};
	  customerDoc = UpdateCustomerDocFromUserDelivery(customerDoc, doc);
      
      cust[email] = customerDoc;
	  
	  emailToPollOnlineCRM = email;
    }

	if(IsLoyaltyMember(doc)) {
		log('INGEST', 'Loyalty System');
		
		var email = doc.email;
		
		var customerDoc = GetCustomer360DocIfItExists(email);
		if(!customerDoc)
			customerDoc = {};
		customerDoc = UpdateCustomerDocFromLoyalty(customerDoc, doc);
		
		cust[email] = customerDoc;
		
		emailToPollOnlineCRM = email;
	}
//    var newobj = {};
//    newobj.stuff = curl("http://172.17.0.8/api/onlineStore/getCustomerDetailsByEmail?customerEmail=lou.hodkiewicz@koepp.info", { "method" : "GET"});

//    log('CURL', newobj.stuff.Email);
//    newobj.incoming = doc;
    
//    cust[meta.id] = newobj;
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

// this function will examine a document
// and determind if it's being ingested from
// the loyalty system
function IsLoyaltyMember(doc) {
	if(doc.points)
		return true;
	return false;
}

// get a customer 360 document
// if it exists
// otherwise return null
function GetCustomer360DocIfItExists(email) {
    try
    {
        if(email != null)
        {
            return cust[email];
        }
    }
    catch (e)
    {
        log('Error GetCustomer360DocIfItExists :', email,'; Exception:', e);
    }
    return null;
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
	for(xref in customerDoc.xrefs)
	{
		if(xref["external_system"] == "loyalty") {
			xref["external_ID"] = loyaltyMemberDoc.id;
			xref["pass"] = loyaltyMemberDoc.password;
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
	for(xref in customerDoc.xrefs)
	{
		if(xref["external_system"] == "home_delivery") {
			xref["external_ID"] = payloadAfter.id;
			xref["pass"] = payloadAfter.password;
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

function OnDelete(meta) {
}