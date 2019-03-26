namespace UserProfileExample.Models
{
    // tag::User[]
    public class User
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    // end::User[]
}