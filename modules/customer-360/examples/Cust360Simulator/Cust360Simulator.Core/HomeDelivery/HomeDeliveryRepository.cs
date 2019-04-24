using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace Cust360Simulator.Core.HomeDelivery
{
    public class HomeDeliveryRepository
    {
        private readonly MySqlConnection _dbConnection;

        public HomeDeliveryRepository(MySqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
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
}