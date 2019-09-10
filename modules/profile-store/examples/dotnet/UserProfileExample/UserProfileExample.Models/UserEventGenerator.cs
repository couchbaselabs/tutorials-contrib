using System;

namespace UserProfileExample.Models
{
    public static class UserEventGenerator
    {
        public static UserEvent Create(string userId)
        {
            var randomDate = DateTime.Now.AddSeconds(-1 * Faker.RandomNumber.Next(0, 86400));

            var userEvent = new UserEvent();
            userEvent.Id = "userevent::" + Guid.NewGuid();
            userEvent.CreatedDate = randomDate;
            userEvent.CreatedDateTimestamp = ((DateTimeOffset)randomDate).ToUnixTimeSeconds() * 1000;
            userEvent.UserId = userId;
            switch (Faker.RandomNumber.Next(0,3))
            {
                case 0:
                    userEvent.EventType = EventType.ProductAddedToCart;
                    break;
                case 1:
                    userEvent.EventType = EventType.ProductViewed;
                    break;
                case 2:
                    userEvent.EventType = EventType.ProfileUpdated;
                    break;
            }
            return userEvent;
        }
    }
}