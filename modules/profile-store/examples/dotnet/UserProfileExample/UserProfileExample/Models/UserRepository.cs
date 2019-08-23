using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Linq;
using Couchbase.Linq.Extensions;
using Couchbase.N1QL;
using Couchbase.Search;
using Couchbase.Search.Queries.Compound;
using Couchbase.Search.Queries.Simple;

namespace UserProfileExample.Models
{
    /*
    // tag::UserRepositorySimple[]
    public class UserRepository
    {
        private readonly IBucket _bucket;

        public UserRepository(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("user_profile");
        }

        public User FindById(string id)
        {
            var result = _bucket.Get<User>(id);
            var user = result.Value;
            user.Id = id;
            return user;
        }

        public void Save(User user)
        {
            _bucket.Upsert(user.Id, new
            {
                user.CountryCode,
                user.Password,
                user.UserName
            });
        }
    }
    // end::UserRepositorySimple[]
    */

    public class UserRepository
    {
        private readonly IBucket _bucket;
        private readonly BucketContext _bucketContext;

        public UserRepository(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("user_profile");
            _bucketContext = new BucketContext(_bucket);
        }

        public User FindById(string id)
        {
            var result = _bucket.Get<User>(id);
            var user = result.Value;
            user.Id = id;
            return user;
        }

        public void Save(User user)
        {
            _bucket.Upsert(user.Id, new
            {
                user.Addresses,
                user.CountryCode,
                user.Enabled,
                user.FirstName,
                user.LastName,
                user.MiddleName,
                user.Password,
                user.Preferences,
                user.SecurityRoles,
                user.SocialSecurityNumber,
                user.Telephones,
                user.TenantId,
                user.UserName,
                user.Type
            });
        }

        // tag::query[]
        public List<User> ListTenantUsers(int tenantId, int offset, int limit)
        {
            var n1ql = $@"Select meta().id as id, username, tenantId, firstName, lastname
                from `{_bucket.Name}`
                where type = 'user'
                and tenantId = $tenantId
                order by firstName asc
                limit $limit
                offset $offset";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("tenantId", tenantId);
            query.AddNamedParameter("limit", limit);
            query.AddNamedParameter("offset", offset);

            var results = _bucket.Query<User>(query);

            return results.Rows;
        }

        public List<User> FindActiveUsersByFirstName(string firstName, bool enabled, string countryCode, int limit, int offset) {
            var results = _bucketContext.Query<User>()
                .Where(u => u.Type == "user")
                .Where(u => u.FirstName.ToLower() == firstName.ToLower())
                .Where(u => u.Enabled == enabled)
                .Where(u => u.CountryCode == countryCode)
                .Select(u => new { key = N1QlFunctions.Meta(u).Id, document = u })
                .Skip(offset)
                .Take(limit)
                .ToList();
            results.ForEach(r => r.document.Id = r.key);
            return results.Select(r => r.document).ToList();
        }
        // end::query[]

        // tag::FtsListActiveUsers[]
        public List<User> FtsListActiveUsers(string firstName, bool enabled, string countryCode, int limit, int skip)
        {
            // tag::fuzzy[]
            var firstNameFuzzy = new MatchQuery(firstName).Fuzziness(1).Field("firstName");
            var firstNameSimple = new MatchQuery(firstName).Field("firstName");
            var nameQuery = new DisjunctionQuery(firstNameSimple, firstNameFuzzy);
            // end::fuzzy[]

            // tag::filter[]
            var isEnabled = new BooleanFieldQuery(enabled).Field("enabled");
            var countryFilter = new MatchQuery(countryCode).Field("countryCode");
            // end::filter[]

            // tag::conj[]
            var conj = new ConjunctionQuery(nameQuery, isEnabled, countryFilter);
            // end::conj[]

            // tag::result[]
            var searchQuery = new SearchQuery();
            searchQuery.Fields("id", "tenantId", "firstName", "lastName", "userName");
            searchQuery.Index = "user_index";
            searchQuery.Query = conj;
            searchQuery.Skip(skip);
            searchQuery.Limit(limit);

            var result = _bucket.Query(searchQuery);
            var users = new List<User>();
            if (result != null && !result.Errors.Any())
            {
                foreach (var hit in result.Hits)
                {
                    var user = new User();
                    user.Id = hit.Id;
                    user.TenantId = int.Parse(hit.Fields["tenantId"].ToString());
                    user.FirstName = hit.Fields["firstName"];
                    user.LastName = hit.Fields["lastName"];
                    user.UserName = hit.Fields["userName"];
                    users.Add(user);
                }
            }

            return users;
            // end::result[]
        }
        // end::FtsListActiveUsers[]

        public async Task AddEventAsync(UserEvent userEvent)
        {
            await AddEventsAsync(new List<UserEvent> { userEvent });
        }

        // tag::AddEventsAsync[]
        public async Task AddEventsAsync(List<UserEvent> events)
        {
            var tasks = events.Select(e => _bucket.InsertAsync(e.Id, new
            {
                e.CreatedDate,
                e.CreatedDateTimestamp,
                e.EventType,
                e.UserId,
                e.Type
            }));
            await Task.WhenAll(tasks);
        }
        // end::AddEventsAsync[]

        // tag::FindLatestUserEvents[]
        public async Task<List<UserEvent>> FindLatestUserEvents(string userId, EventType eventType, int limit, int offset)
        {
            var n1ql = $@"SELECT META(e).id, e.userId, e.createdDate, e.eventType
                            FROM `{_bucket.Name}` e
                            WHERE e.type = 'userEvent'
                            AND e.eventType = $eventType
                            AND e.userId = $userId
                            LIMIT $limit
                            OFFSET $offset";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("eventType", eventType);
            query.AddNamedParameter("userId", userId);
            query.AddNamedParameter("limit", limit);
            query.AddNamedParameter("offset", offset);

            var result = await _bucket.QueryAsync<UserEvent>(query);

            return result.Rows;
        }
        // end::FindLatestUserEvents[]

        // example URL: http://localhost:5000/api/eventTimeSeries?eventType=0&startDate=2019-08-06&endDate=2019-08-09
        public async Task<List<dynamic>> GetTimeSeriesDataForEventType(EventType eventType, DateTime startDate, DateTime endDate)
        {
            var n1ql = @"with segmentedUserEvents AS (
                select 
		            DATE_ADD_STR(DATE_TRUNC_STR(u.createdDate,""hour""), ROUND(DATE_PART_STR(u.createdDate,""minute"")/15,0)*15,""minute"") as segment,
		            u.eventType,
		            count(*) as numEvents
	            from user_profile u
	            where u.type = 'userEvent'
	            group by DATE_ADD_STR(DATE_TRUNC_STR(u.createdDate,""hour""), ROUND(DATE_PART_STR(u.createdDate,""minute"")/15,0)*15,""minute""), u.eventType
            )
            select
               e.segment,
               SUM(e.numEvents) OVER(PARTITION BY e.segment) AS numEvents
            from segmentedUserEvents e
            where e.eventType = $eventType
            and e.segment BETWEEN $startDate AND $endDate
            order by e.segment;";

            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("eventType", eventType);
            query.AddNamedParameter("startDate", startDate);
            query.AddNamedParameter("endDate", endDate);

            var result = await _bucket.QueryAsync<dynamic>(query);

            return result.Rows;
        }
    }
}