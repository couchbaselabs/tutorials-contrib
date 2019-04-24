using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Cust360Simulator.Core.HomeDelivery;
using Cust360Simulator.Core.Loyalty;
using Cust360Simulator.Core.OnlineStore;
using MySql.Data.MySqlClient;
using Npgsql;
using Dapper;

namespace Cust360Simulator.Core
{
    public class EnterpriseData
    {
        private readonly SQLiteConnection _onlineStoreDb;
        private readonly NpgsqlConnection _loyaltyDb;
        private readonly MySqlConnection _homeDeliveryDb;
        private readonly OnlineStoreRepository _onlineStoreRepository;

        public EnterpriseData(
            SQLiteConnection onlineStoreDb,
            NpgsqlConnection loyaltyDb,
            MySqlConnection homeDeliveryDb,
            OnlineStoreRepository onlineStoreRepository)
        {
            _onlineStoreDb = onlineStoreDb;
            _loyaltyDb = loyaltyDb;
            _homeDeliveryDb = homeDeliveryDb;
            _onlineStoreRepository = onlineStoreRepository;
        }

        public void EnsureSchemas()
        {
            // NOTE: surrounding these in try/catch is such a hack
            // but this is not meant to be true data migration system
            // I just want to make sure all these tables are in place before proceeding
            // a real system would use a tool like FluentMigrator/EF/etc to do this
            try
            {
                _homeDeliveryDb.Execute(@"ALTER TABLE `customers`
	ALTER `first_name` DROP DEFAULT;
ALTER TABLE `customers`
	CHANGE COLUMN `first_name` `name` VARCHAR(500) NOT NULL AFTER `id`,
	ADD COLUMN `password` VARCHAR(255) NOT NULL AFTER `email`,
	ADD COLUMN `address_line_1` VARCHAR(255) NOT NULL AFTER `password`,
	ADD COLUMN `address_line_2` VARCHAR(255) NOT NULL AFTER `address_line_1`,
	ADD COLUMN `city` VARCHAR(255) NOT NULL AFTER `address_line_2`,
	ADD COLUMN `state` VARCHAR(2) NOT NULL AFTER `city`,
	ADD COLUMN `zipcode` VARCHAR(10) NOT NULL AFTER `state`,
	ADD COLUMN `phonenumber` VARCHAR(25) NOT NULL AFTER `zipcode`,
	DROP COLUMN `last_name`;

ALTER TABLE `customers`
	ALTER `address_line_2` DROP DEFAULT;
ALTER TABLE `customers`
	CHANGE COLUMN `address_line_2` `address_line_2` VARCHAR(255) NULL AFTER `address_line_1`;");
            }
            catch { }

            try
            {
                _loyaltyDb.Execute(@"CREATE TABLE ""members"" (
	""id"" SERIAL PRIMARY KEY,
	""password"" VARCHAR(50) NOT NULL DEFAULT E'0',
	""fname"" VARCHAR(50) NOT NULL DEFAULT E'0',
	""lname"" VARCHAR(50) NOT NULL DEFAULT E'0',
    ""email"" VARCHAR(100) NOT NULL DEFAULT E'0',
	""points"" INTEGER NOT NULL DEFAULT E'0'
);");
            }
            catch { }

            try
            {
                _onlineStoreDb.Execute(@"CREATE TABLE [customer_details] (
[id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[password] VARCHAR(50)  NOT NULL,
[email] VARCHAR(100)  NOT NULL,
[name] VARCHAR(150)  NOT NULL
);");
            }
            catch { }

            try { 
            _onlineStoreDb.Execute(@"CREATE TABLE [product] (
[product_id] INTEGER  NOT NULL PRIMARY KEY,
[name] VARCHAR(50)  NOT NULL,
[description] TEXT  NOT NULL,
[category] VARCHAR(50)  NOT NULL
);");
            }
            catch { }

            try { 
            _onlineStoreDb.Execute(@"CREATE TABLE [order] (
[order_id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[purchase_date] DATE  NOT NULL,
[customer_id] INTEGER  NOT NULL,
FOREIGN KEY(customer_id) REFERENCES customer_details(id)
);");
            }
            catch { }

            try
            {
                _onlineStoreDb.Execute(@"CREATE TABLE [order_item] (
[order_id] INTEGER NOT NULL,
[product_id] INTEGER NOT NULL,
[quantity] INTEGER NOT NULL,
[price] FLOAT NOT NULL,
FOREIGN KEY(product_id) REFERENCES product(product_id),
FOREIGN KEY(order_id) REFERENCES [order](order_id)
);");
            } catch { }

            PopulateProductTable();
        }

        private void PopulateProductTable()
        {
            // populate product table
            // same hardcoded data every time
            _onlineStoreDb.Execute("DELETE FROM product;");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (100, 'Tennis Shoes','Footwear','Lorem ipsum dolor sit amet')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (110, 'Dress Shoes','Footwear','consectetur adipiscing elit, sed do eiusmod')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (200, 'T-shirt','Clothing','tempor incididunt ut labore et dolore magna aliqua')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (210, 'Blouse','Clothing','Ut enim ad minim veniam')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (300, 'Jacket','Outerwear','quis nostrud exercitation ullamco laboris nisi ut')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (310, 'Coat','Outerwear','aliquip ex ea commodo consequat')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (320, 'Scarf','Outerwear','Duis aute irure dolor in reprehenderit in voluptate')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (400, 'Necklace','Jewelry','velit esse cillum dolore eu fugiat nulla pariatur')");
            _onlineStoreDb.Execute(
                @"INSERT INTO product (product_id, name, category, description) VALUES (410, 'Earrings','Jewelry','Excepteur sint occaecat cupidatat non proident')");
        }

        /// <summary>
        /// Create a person and split their data up amongst the enterprise
        /// This data will be joined together in the Customer 360 system
        /// Not every person will necessarily be in all three systems
        /// But they should be in at least the loyalty system OR the home delivery system
        /// </summary>
        public void PopulateAPersonAcrossEnterprise()
        {
            // base data to be shared/matched across enterprise
            var firstName = Faker.Name.First();
            var lastName = Faker.Name.Last();
            var email = Faker.Internet.Email(firstName + " " + lastName);

            // generate a loyalty member
            var member = new LoyaltyMember();
            member.Password = Path.GetRandomFileName();
            member.FirstName = firstName;
            member.LastName = lastName;
            member.Email = email;
            member.Points = Faker.RandomNumber.Next(1, 100);

            // generate a home delivery customer
            var homeDeliveryCustomer = new HomeDeliveryCustomer();
            homeDeliveryCustomer.Name = firstName + " " + lastName;                 // same name
            homeDeliveryCustomer.Email = email;                                     // same email
            homeDeliveryCustomer.Password = Path.GetRandomFileName();               // different password
            homeDeliveryCustomer.AddressLine1 = Faker.Address.StreetAddress();
            if ((Faker.RandomNumber.Next(0, 100) % 2) == 0)
                homeDeliveryCustomer.AddressLine2 = Faker.Address.SecondaryAddress();
            homeDeliveryCustomer.City = Faker.Address.City();
            homeDeliveryCustomer.State = Faker.Address.UsStateAbbr();
            homeDeliveryCustomer.ZipCode = Faker.Address.ZipCode();
            homeDeliveryCustomer.PhoneNumber = Faker.Phone.Number();

            var onlineStoreCustomer = new OnlineCustomerDetails();
            onlineStoreCustomer.Name = firstName + " " + lastName;                  // same name
            onlineStoreCustomer.Email = email;                                      // same email
            onlineStoreCustomer.Password = Path.GetRandomFileName();                // different password

            // randomly decide if the person will be:
            //  in loyalty system
            //  in home delivery system
            //  or both
            var isInLoyaltySystem = false;
            var isInHomeDeliverySystem = false;
            var dice = Faker.RandomNumber.Next(1, 100);
            if (dice < 33)
            {
                isInLoyaltySystem = true;
                isInHomeDeliverySystem = true;
            } else if (dice > 67)
            {
                isInLoyaltySystem = false;
                isInHomeDeliverySystem = true;
            }
            else
            {
                isInLoyaltySystem = true;
                isInHomeDeliverySystem = false;
            }

            // and whether they will be in the online store
            var isInOnlineStoreSystem = (Faker.RandomNumber.Next(1, 100) < 50);

            if (isInLoyaltySystem)
            {
                _loyaltyDb.Execute(@"INSERT INTO members (password, fname, lname, points) VALUES (@Password,@FirstName,@LastName,@Points);", member);
            }

            if (isInHomeDeliverySystem)
            {
                _homeDeliveryDb.Execute(
                    @"INSERT INTO customers (name, email, password, address_line_1, address_line_2, city, state, zipcode, phonenumber) VALUES (
                        @Name, @Email, @Password, @AddressLine1, @AddressLine2, @City, @State, @ZipCode, @PhoneNumber);",
                    homeDeliveryCustomer);
            }

            if (isInOnlineStoreSystem)
            {
                var customerDetailsId = _onlineStoreDb.Query<int>(
                    "INSERT INTO customer_details (password, email, name) VALUES (@Password, @Email, @Name); SELECT last_insert_rowid() AS customerId",
                    onlineStoreCustomer).First();

                CreateOrders(customerDetailsId);
            }
        }

        /// <summary>
        /// Given a customer ID, this method will create a random number
        /// of randomly generated orders for that customer. Each order will include a random number
        /// of randomly generated order items of randomly selected products
        /// </summary>
        /// <param name="customerDetailsId"></param>
        private void CreateOrders(int customerDetailsId)
        {
            var randomNumOrders = Faker.RandomNumber.Next(1, 4);
            for (var i = 0; i < randomNumOrders; i++)
            {
                _onlineStoreRepository.CreateRandomOnlineOrder(customerDetailsId);
            }
        }
    }
}