using System;

namespace UserProfileExample.Models
{
    public class UserEvent
    {
        public UserEvent()
        {
            Type = "userEvent";
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public EventType EventType { get; set; }
        public string Type { get; }
    }

    public enum EventType
    {
        ProductViewed = 0,
        ProductAddedToCart = 1,
        ProfileUpdated = 2
    }

    public static class FakeUserEvent
    {
        public static UserEvent Create(string userId)
        {
            var userEvent = new UserEvent();
            userEvent.Id = Guid.NewGuid().ToString();
            userEvent.CreatedDate = DateTime.Now.AddSeconds(-1 * Faker.RandomNumber.Next(0, 300));
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