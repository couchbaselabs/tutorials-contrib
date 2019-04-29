using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;

namespace Cust360Simulator.Core.OnlineStore
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
            var randomCustomerId = _sqlLiteConnection.Query<int>(@"
                SELECT id FROM customer_details ORDER BY random() LIMIT 1").First();

            CreateRandomOnlineOrder(randomCustomerId);
        }

        /// <summary>
        /// Given a customer ID, this will create a randomish order
        /// Containing a random number of randomish items linked
        /// to randomly selected products
        /// </summary>
        /// <param name="customerId"></param>
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

        public bool DoesCustomerExistsByEmail(string customerEmail)
        {
            var customerCount = _sqlLiteConnection.Query<int>(@"
                SELECT COUNT(1) FROM customer_details WHERE email = @CustomerEmail",
                new {CustomerEmail = customerEmail}).FirstOrDefault();
            return customerCount > 0;
        }

        public OnlineCustomerDetails GetCustomerDetailsByEmail(string customerEmail)
        {
            var customer = _sqlLiteConnection.Query<OnlineCustomerDetails>(@"
                SELECT * FROM customer_details WHERE email = @CustomerEmail",
                new { CustomerEmail = customerEmail}).First();
            return customer;
        }

        public List<OnlineStoreOrder> GetOrdersByCustomerId(int customerId)
        {
            var rawOrders = _sqlLiteConnection.Query<dynamic>(@"
                select o.order_id, o.purchase_date, i.product_id, i.quantity, i.price
                from [order] o
                join order_item i ON o.order_id = i.order_id
                where o.customer_id = @CustomerId", new {CustomerId = customerId}).ToList();

            // grouping the results of the join with Linq
            // in order to optimize the total number of SQL calls / loops
            // if we were using a full OR/M (and no dapper), the OR/M would typically
            // handle this for us
            // but instead, I'm grouping by order_id/purchase_date (attributes of the aggregate root)
            // and then putting all the order_item information into a List inside the aggregate root
            var orderGroup = rawOrders.GroupBy(r => new { r.order_id, r.purchase_date})
                .Select(r => new OnlineStoreOrder
                {
                    OrderId = (int)r.Key.order_id,
                    PurchaseDate = r.Key.purchase_date,
                    CustomerId = customerId,
                    Items = r.Select(i => new OnlineStoreOrderItem
                    {
                        Quantity = (int)i.quantity,
                        Price = (decimal)i.price,
                        ProductId = (int)i.product_id,
                        OrderId = (int)i.order_id
                    }).ToList()
                });

            return orderGroup.ToList();
        }

        public OnlineStoreProduct GetProductDetailsByProductId(int productId)
        {
            return _sqlLiteConnection.Query<OnlineStoreProduct>(@"
                SELECT *, product_id AS ProductId FROM product WHERE product_id = @ProductId",
                new { ProductId = productId}).First();
        }
    }
}