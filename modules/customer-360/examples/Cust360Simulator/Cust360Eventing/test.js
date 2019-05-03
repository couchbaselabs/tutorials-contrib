var assert = require('assert');
var eventing = require('./eventingFunctions');

var log = function(x,y) { console.log(x + " | " + y);}

describe('given a user delivery doc', function() {

    var cust = {};
    var doc = {
        schema: {
          name: 'dbserver1.inventory.customers.Envelope',
          optional: false,
          type: 'struct'
        },
        payload: {
          op: 'c',
          before: null,
          after: {
            zipcode: '04006',
            password: '5owhiy1j.isq',
            city: 'West Evertchester',
            name: 'Sabryna Kovacek',
            address_line_1: '600 Sporer Curve',
            phonenumber: '811.128.4320 x4808',
            id: 1029,
            address_line_2: 'Suite 528',
            state: 'NH',
            email: 'sabryna_kovacek@farrell.co.uk'
          },
          source: {
            ts_sec: 1556559563,
            query: null,
            thread: 28,
            server_id: 223344,
            version: '0.9.3.Final',
            file: 'mysql-bin.000003',
            connector: 'mysql',
            pos: 11971,
            name: 'dbserver1',
            gtid: null,
            row: 0,
            snapshot: false,
            db: 'inventory',
            table: 'customers'
          },
          ts_ms: 1556559563137
        }
      };

    beforeEach(function() {
        cust = {};
    });

    it('should create a new customer 360 doc if one doesnt exist', function() {
        var expectedDocKey = doc.payload.after.email;

        eventing.IngestUserDelivery(doc, cust, log);

        assert.notStrictEqual(cust[expectedDocKey], undefined);
    });

    it('should map the appropriate fields to the customer 360 doc', function() {
        var k = doc.payload.after.email;
        var origData = doc.payload.after;

        eventing.IngestUserDelivery(doc, cust, log);

        assert.equal(cust[k].type, "user");
        assert.notStrictEqual(cust[k].profile, undefined);
        assert.equal(cust[k].profile.name, origData.name);
        assert.equal(cust[k].profile.address1, origData.address_line_1);
        assert.equal(cust[k].profile.address2, origData.address_line_2);
        assert.equal(cust[k].profile.phone, origData.phonenumber);
        assert.equal(cust[k].profile.state, origData.state);
        assert.equal(cust[k].profile.city, origData.city);
    });

    it('should create a xref in the customer 360 doc', function() {
        var k = doc.payload.after.email;

        eventing.IngestUserDelivery(doc, cust, log);

        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 1);
        assert.equal(cust[k].xrefs[0].external_system, "home_delivery");
        assert.equal(cust[k].xrefs[0].external_ID, doc.payload.after.id);
        assert.equal(cust[k].xrefs[0].pass, doc.payload.after.password);
    });

    it("should update an existing customer 360 doc", function() {
        // arrange
        var k = doc.payload.after.email;
        var origData = doc.payload.after;

        cust[k] = {
            type: 'user',
            profile: {
              name: 'Perry Krug',
              address1: '123 Mockingbird Ln',
              address2: 'Apt A',
              phone: '555-555-5555',
              email: 'perry@gmail.com'
            },
            xrefs: [
              {
                external_system: 'home_delivery',
                external_ID: 'p_krug',
                pass: '8ds8f03'
              }
            ]
          }

        // act
        eventing.IngestUserDelivery(doc, cust, log);

        // assert
        assert.equal(cust[k].profile.name, origData.name);
        assert.equal(cust[k].profile.address1, origData.address_line_1);
        assert.equal(cust[k].profile.address2, origData.address_line_2);
        assert.equal(cust[k].profile.phone, origData.phonenumber);
        assert.equal(cust[k].profile.state, origData.state);
        assert.equal(cust[k].profile.city, origData.city);
        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 1);
        assert.equal(cust[k].xrefs[0].external_system, "home_delivery");
        assert.equal(cust[k].xrefs[0].external_ID, doc.payload.after.id);
        assert.equal(cust[k].xrefs[0].pass, doc.payload.after.password);
    });

    it("should return the email address", function() {
        var result = eventing.IngestUserDelivery(doc, cust, log);

        assert.equal(result, doc.payload.after.email);
    });
});

describe('given a loyalty member doc', function() {
    var cust = {};
    var doc = {
        id: '27',
        password: 'xrinn5xp.swd',
        firstName: 'Aron',
        lastName: 'Sawayn',
        email: 'aron.sawayn@stehr.co.uk',
        points: '47'
      };
    var meta = {};

    beforeEach(function() {
        cust = {};
    });

    it('should create a new customer 360 doc if one doesnt exist', function() {
        var expectedDocKey = doc.email;

        eventing.IngestLoyaltyMember(doc, cust, log);

        assert.notStrictEqual(cust[expectedDocKey], undefined);
    });

    it('should map the appropriate fields to the customer 360 doc', function() {
        var k = doc.email;

        eventing.IngestLoyaltyMember(doc, cust, log);

        assert.equal(cust[k].type, "user");
        assert.notStrictEqual(cust[k].profile, undefined);
        assert.equal(cust[k].profile.name, doc.firstName + " " + doc.lastName);
        assert.equal(cust[k].loyaltyPoints, doc.points);
    });

    it('should create a xref in the customer 360 doc', function() {
        var k = doc.email;

        eventing.IngestLoyaltyMember(doc, cust, log);

        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 1);
        assert.equal(cust[k].xrefs[0].external_system, "loyalty");
        assert.equal(cust[k].xrefs[0].external_ID, doc.id);
        assert.equal(cust[k].xrefs[0].pass, doc.password);
    });

    it("should update an existing customer 360 doc", function() {
        var k = doc.email;

        cust[k] = {
            type: 'user',
            profile: {
              name: 'Perry Krug',
              address1: '123 Mockingbird Ln',
              address2: 'Apt A',
              phone: '555-555-5555',
              email: 'perry@gmail.com'
            },
            xrefs: [
              {
                external_system: 'loyalty',
                external_ID: 'p_krug',
                pass: '8ds8f03'
              }
            ]
          };

          eventing.IngestLoyaltyMember(doc, cust, log);

        assert.equal(cust[k].profile.name, doc.firstName + " " + doc.lastName);
        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 1);
        assert.equal(cust[k].xrefs[0].external_system, "loyalty");
        assert.equal(cust[k].xrefs[0].external_ID, doc.id);
        assert.equal(cust[k].xrefs[0].pass, doc.password);
    });

    it("should return the email address", function() {
        var result = eventing.IngestLoyaltyMember(doc, cust, log);

        assert.equal(result, doc.email);
    });

});

describe('given an email address for the online CRM REST API', function() {
    var cust = {};
    var doc = {
        id: '27',
        password: 'xrinn5xp.swd',
        firstName: 'Aron',
        lastName: 'Sawayn',
        email: 'aron.sawayn@stehr.co.uk',
        points: '47'
      };
    var meta = {};
    // setup a mock curl
    var curlDoc = {
        Id: 15,
        Name: 'Aron Sawayn UPDATED',
        Email: 'aron.sawayn@stehr.co.uk',
        Password: '5hqnbeeo.dk0'
    };
    var orderDocs = [
      {
        "OrderId": 2,
        "PurchaseDate": "2019-04-23T21:36:2403680944",
        "CustomerId": 2,
        "Items": [ { "OrderId": 2, "ProductId": 210, "Quantity": 1, "Price": 153.52 },
          { "OrderId": 2, "ProductId": 110, "Quantity": 3, "Price": 184.38 }
        ]
      },
      {
        "OrderId": 3,
        "PurchaseDate": "2019-04-18T22:36:2404570443",
        "CustomerId": 2,
        "Items": [ { "OrderId": 3, "ProductId": 100, "Quantity": 2, "Price": 181.22 } ]
      }
    ];
    var curl = function(url, options) {
      if(url.indexOf("getCustomerDetailsByEmail") > -1)
        return curlDoc; // if the url contains getCustomerDetailsByEmail
      if(url.indexOf("getOrdersByCustomerId") > -1)
        return orderDocs; // if the url contains getOrdersByCustomerId
    };

    beforeEach(function() {
        cust[doc.email] = {
            id: 'fe52295a-7fb7-46db-936f-dfcacbb197ae',
            type: 'user',
            profile: {
              name: 'Perry Krug',
              address1: '123 Mockingbird Ln',
              address2: 'Apt A',
              phone: '555-555-5555',
              email: 'aron.sawayn@stehr.co.uk'
            },
            xrefs: [
              {
                external_system: 'home_delivery',
                external_ID: 'p_krug',
                pass: '8ds8f03'
              },
              {
                external_system: 'loyalty',
                external_ID: 'aron.sawayn@stehr.co.uk',
                pass: '319d0a31'
              }
            ]
          };
    });

    it("should update user by polling the REST API", function() {
        var k = doc.email;

        eventing.PollOnlineCrm(doc, k, cust, curl, log);

        assert.equal(cust[k].profile.name, curlDoc.Name);
    });

    it("should add a xref", function() {
        var k = doc.email;

        eventing.PollOnlineCrm(doc, k, cust, curl, log);

        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 3);
        assert.equal(cust[k].xrefs[2].external_system, "onlineStore");
        assert.equal(cust[k].xrefs[2].external_ID, curlDoc.Id);
        assert.equal(cust[k].xrefs[2].pass, curlDoc.Password);
    });

    it("should update an existing xref", function() {
        var k = doc.email;

        // setup an existing xref for online store
        cust[doc.email].xrefs.push({
            external_system: 'onlineStore',
            external_ID: 15,
            pass: '5hqnbeeo.dk0 NEW PASSWORD'
        });

        eventing.PollOnlineCrm(doc, k, cust, curl, log);

        assert.notStrictEqual(cust[k].xrefs, undefined);
        assert.equal(cust[k].xrefs.length, 3);
        assert.equal(cust[k].xrefs[2].external_system, "onlineStore");
        assert.equal(cust[k].xrefs[2].external_ID, curlDoc.Id);
        assert.equal(cust[k].xrefs[2].pass, curlDoc.Password);        
    });

    it("should import new orders", function() {
      var k = doc.email;
      var expectedOrderKey = orderDocs[0].OrderId;
      
      // act
      eventing.PollOnlineCrm(doc, k, cust, curl, log);

      assert.notStrictEqual(cust[expectedOrderKey], undefined);
    });
});
