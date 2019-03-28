using System;
using System.Collections.Generic;
using System.Linq;
using Faker;

namespace UserProfileExample.Models
{
    /*
    // tag::User[]
    public class User
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    // end::User[]
    */

    // tag::User2[]
    public class User
    {
        public User()
        {
            Type = "user";
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public int TenantId { get; set; }
        public string CountryCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SocialSecurityNumber { get; set; }
        public List<Telephone> Telephones { get; set; }
        public List<Preference> Preferences { get; set; }
        public List<Address> Addresses { get; set; }
        public List<string> SecurityRoles { get; set; }
        public string Type { get; private set;}
    }
    // end::User2[]

    public class Telephone
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }

    public class Preference
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Address
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
    }

    public static class FakeUser
    {
        public static User Create(string firstName = null, bool? enabled = null, string countryCode = null)
        {
            var user = new User();
            user.Addresses = CreateFakeAddresses();
            user.CountryCode = countryCode ?? Faker.Address.Country().Substring(0,2).ToUpper();
            user.Enabled = enabled ?? (Faker.RandomNumber.Next(100,200) % 2) == 0;
            user.FirstName = firstName ?? Faker.Name.First();
            user.Id = Guid.NewGuid().ToString();
            user.LastName = Faker.Name.Last();
            user.MiddleName = Faker.Name.First();
            user.Password = Faker.Lorem.Words(1).First();
            user.Preferences = CreateFakePreferences();
            user.SecurityRoles = CreateFakeSecurityRoles();
            user.SocialSecurityNumber = Faker.RandomNumber.Next(111111111,999999999).ToString();
            user.Telephones = CreateFakeTelephones();
            user.TenantId = Faker.RandomNumber.Next(1,10);
            user.UserName = Faker.Internet.UserName();
            return user;
        }

        private static List<Address> CreateFakeAddresses()
        {
            var num = Faker.RandomNumber.Next(1,3);
            var addresses = new List<Address>();
            for(var i=0;i<num;i++)
            {
                var address = new Address();
                address.City = Faker.Address.City();
                address.CountryCode = Faker.Address.Country().Substring(0,2).ToUpper();
                address.Name = Faker.Name.FullName();
                address.Number = Faker.RandomNumber.Next(100,9999).ToString();
                address.State = Faker.Address.UsState();
                address.Street = Faker.Address.StreetName();
                address.ZipCode = Faker.Address.ZipCode();
                addresses.Add(address);
            }
            return addresses;
        }

        private static List<Preference> CreateFakePreferences()
        {
            var num = Faker.RandomNumber.Next(1,3);
            var prefs = new List<Preference>();
            for(var i=0;i<num;i++)
            {
                var pref = new Preference();
                pref.Name = Faker.Lorem.Words(1).First();
                pref.Value = Faker.Internet.DomainWord();
                prefs.Add(pref);
            }
            return prefs;
        }

        private static List<string> CreateFakeSecurityRoles()
        {
            var roles = new List<string>();
            roles.Add("USER");
            roles.Add("ADMIN");
            roles.Add("EDITOR");
            roles.Add("VIEWER");
            roles.Add("MANAGE");

            var num = Faker.RandomNumber.Next(1,4);
            for(var i=0;i<num;i++)
            {
                roles.RemoveAt(Faker.RandomNumber.Next(0,roles.Count));
            }
            return roles;
        }

        private static List<Telephone> CreateFakeTelephones()
        {
            var num = Faker.RandomNumber.Next(1,3);
            var phones = new List<Telephone>();
            var phoneNames = new List<string> { "cell", "home", "office", "emergency"};
            for(var i=0;i<num;i++)
            {
                var phone = new Telephone();
                phone.Name = phoneNames[Faker.RandomNumber.Next(0, phoneNames.Count)];
                phone.Number = Faker.Phone.Number();
                phones.Add(phone);
            }
            return phones;
        }
    }
}