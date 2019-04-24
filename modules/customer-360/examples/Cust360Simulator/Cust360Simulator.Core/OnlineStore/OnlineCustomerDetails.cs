using System;
using System.Collections.Generic;

namespace Cust360Simulator.Core.OnlineStore
{
    public class OnlineCustomerDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class OnlineStoreOrder
    {
        public int OrderId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int CustomerId { get; set; }
        public List<OnlineStoreOrderItem> Items { get; set; }
    }

    public class OnlineStoreOrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OnlineStoreProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }
}