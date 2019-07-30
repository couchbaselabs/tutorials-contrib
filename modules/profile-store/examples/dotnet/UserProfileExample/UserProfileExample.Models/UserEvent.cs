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
}