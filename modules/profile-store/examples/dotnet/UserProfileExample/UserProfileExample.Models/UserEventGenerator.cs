using System;

namespace UserProfileExample.Models
{
    public static class UserEventGenerator
    {
        public static UserEvent Create(string userId)
        {
            var userEvent = new UserEvent();
            userEvent.Id = "userevent::" + Guid.NewGuid().ToString();
            //userEvent.CreatedDate = DateTime.Now.AddSeconds(-1 * Faker.RandomNumber.Next(0, 300));
            userEvent.CreatedDate = DateTime.Now.AddSeconds(-1 * Faker.RandomNumber.Next(0, 86400));
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