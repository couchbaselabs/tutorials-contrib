using System.Data;
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

        public void CreateNewCustomer()
        {
            var customer = new HomeDeliveryCustomer();
            customer.FirstName = Faker.Name.First();
            customer.LastName = Faker.Name.Last();
            customer.Email = Faker.Internet.Email();

            _dbConnection.Execute(
                @"INSERT INTO customers (first_name, last_name, email) VALUES (
                        @FirstName, @LastName, @Email);", new
                {
                    customer.FirstName,
                    customer.LastName,
                    customer.Email
                });
        }

        public void UpdateExistingCustomer()
        {

        }
    }

    public class HomeDeliveryCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}