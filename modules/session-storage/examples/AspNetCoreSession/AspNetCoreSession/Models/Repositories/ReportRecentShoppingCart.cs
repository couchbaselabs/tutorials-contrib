using System;

namespace AspNetCoreSession.Models.Repositories
{
    public class ReportRecentShoppingCart
    {
        public string SessionStoreId { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumItems { get; set; }
    }
}