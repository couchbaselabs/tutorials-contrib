using System;
using System.Data.SQLite;
using System.Linq;
using Dapper;

namespace Cust360Simulator.Core
{
    public class OnlineStoreRepository
    {
        private readonly SQLiteConnection _sqlLiteConnection;

        public OnlineStoreRepository(SQLiteConnection sqlLiteConnection)
        {
            _sqlLiteConnection = sqlLiteConnection;
        }

        public void UpdateExisting()
        {
            // TODO: find a random customer ID
            var randomCustomerId = _sqlLiteConnection.Query<int>(@"
                SELECT id FROM customer_details ORDER BY random() LIMIT 1").First();

            CreateRandomOnlineOrder(randomCustomerId);
        }

        public void CreateRandomOnlineOrder(int customerId)
        {
            var purchaseDate = DateTime.Now.AddHours(-1 * Faker.RandomNumber.Next(48, 500));
            var orderId = _sqlLiteConnection.Query<int>(
                "INSERT INTO [order] (purchase_date, customer_id) VALUES (@PurchaseDate, @CustomerId); SELECT last_insert_rowid() AS orderId;",
                new { PurchaseDate = purchaseDate, CustomerId = customerId }).First();

            var randomNumProducts = Faker.RandomNumber.Next(1, 6);
            var randomProductIds = _sqlLiteConnection.Query<int>("SELECT product_id FROM product ORDER BY RANDOM() LIMIT @NumProducts", new { NumProducts = randomNumProducts });
            foreach (var randomProductId in randomProductIds)
            {
                var randomQuantity = Faker.RandomNumber.Next(1, 5);
                var randomPrice = Faker.RandomNumber.Next(1000, 26000) * 0.01;
                _sqlLiteConnection.Execute(
                    "INSERT INTO order_item (order_id, product_id, quantity, price) VALUES (@OrderId, @ProductId, @Quantity, @Price)",
                    new
                    {
                        OrderId = orderId,
                        ProductId = randomProductId,
                        Quantity = randomQuantity,
                        Price = randomPrice
                    });
            }
        }
    }
}