using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;

namespace UserProfileExample.Models
{
    // tag::UserRepository[]
    public class UserRepository
    {
        private IBucket _bucket;

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
    // end::UserRepository[]
}