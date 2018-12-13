using System.Collections.Generic;
using System.Linq;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.N1QL;

namespace AspNetCoreSession.Models.Repositories
{
    public interface ISessionStorageRepository
    {
        int GetCountOfAllSessions();
        List<ReportRecentShoppingCart> GetReportRecentShoppingCarts();
        List<ReportMostCommonShoppingCartItems> GetReportMostCommonShoppingCartItems();
    }

    public class SessionStorageRepository : ISessionStorageRepository
    {
        private readonly IBucket _bucket;

        public SessionStorageRepository(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("sessionstore");
        }

        public int GetCountOfAllSessions()
        {
            var n1ql = @"SELECT VALUE COUNT(*) FROM `" + _bucket.Name + "`";
            var query = QueryRequest.Create(n1ql);
            return _bucket.Query<int>(query).Rows.First();
        }

        /// <summary>
        /// Get the most recent 10 shopping carts: their ID, date created, and the number of items in them
        /// </summary>
        /// <returns></returns>
        public List<ReportRecentShoppingCart> GetReportRecentShoppingCarts()
        {
            var n1ql = @"
                SELECT 
                    META().id AS sessionStoreId,
                    BASE64_DECODE(s.shoppingcart).DateCreated AS dateCreated,
                    ARRAY_COUNT(BASE64_DECODE(s.shoppingcart).Items) AS numItems
                FROM " + _bucket.Name + @" s
                WHERE s.shoppingcart IS NOT MISSING
                ORDER BY STR_TO_UTC(BASE64_DECODE(s.shoppingcart).DateCreated) DESC
                LIMIT 10;";
            var query = QueryRequest.Create(n1ql);
            return _bucket.Query<ReportRecentShoppingCart>(query).Rows;
        }

        public List<ReportMostCommonShoppingCartItems> GetReportMostCommonShoppingCartItems()
        {
            var n1ql = @"
                SELECT 
                    i.ItemName,
                    SUM(i.Quantity) AS totalQuantity
                FROM " + _bucket.Name + @" s
                UNNEST BASE64_DECODE(s.shoppingcart).Items i
                WHERE s.shoppingcart IS NOT MISSING
                GROUP BY i.ItemName
            ORDER BY SUM(i.Quantity) DESC;";
            var query = QueryRequest.Create(n1ql);
            return _bucket.Query<ReportMostCommonShoppingCartItems>(query).Rows;
        }
    }
}