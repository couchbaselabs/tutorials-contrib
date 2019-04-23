using System;
using System.Collections.Generic;

namespace Cust360Simulator.Core
{
    public class OnlineCustomerDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}