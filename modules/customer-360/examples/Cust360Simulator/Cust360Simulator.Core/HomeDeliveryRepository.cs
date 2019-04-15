using System.Data;
using System.IO;
using System.Linq;
using Dapper;

namespace Cust360Simulator.Core
{
    public class HomeDeliveryRepository
    {
        private readonly IDbConnection _dbConnection;

        public HomeDeliveryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// This will create a new randomish user
        /// </summary>
        public void CreateNewCustomer()
        {
            var customer = new HomeDeliveryCustomer();
            customer.Name = Faker.Name.FullName();
            customer.Email = Faker.Internet.Email();
            customer.Password = Path.GetRandomFileName();
            customer.AddressLine1 = Faker.Address.StreetAddress();
            if((Faker.RandomNumber.Next(0,100) % 2) == 0)
                customer.AddressLine2 = Faker.Address.SecondaryAddress();
            customer.City = Faker.Address.City();
            customer.State = Faker.Address.UsStateAbbr();
            customer.ZipCode = Faker.Address.ZipCode();
            customer.PhoneNumber = Faker.Phone.Number();

            _dbConnection.Execute(
                @"INSERT INTO customers (name, email, password, address_line_1, address_line_2, city, state, zipcode, phonenumber) VALUES (
                        @Name, @Email, @Password, @AddressLine1, @AddressLine2, @City, @State, @ZipCode, @PhoneNumber);", customer);
        }

        /// <summary>
        /// This will pick one of the users at random and update
        /// </summary>
        public void UpdateExistingCustomer()
        {
            var customer = _dbConnection.Query<HomeDeliveryCustomer>("SELECT * FROM customers ORDER BY RAND() LIMIT 1;").First();
            if ((Faker.RandomNumber.Next(0, 100) % 2) == 0)
                customer.PhoneNumber = Faker.Phone.Number();
            if ((Faker.RandomNumber.Next(0, 100) % 3) == 0)
                customer.City = Faker.Address.City();

            _dbConnection.Execute(@"
                UPDATE customers SET
                    phonenumber = @PhoneNumber,
                    city = @City
                WHERE id = @Id", new {customer.PhoneNumber, customer.City, customer.Id});
        }
    }

    public class HomeDeliveryCustomer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}