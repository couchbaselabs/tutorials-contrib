using AspNetCoreSession.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Couchbase.Extensions.Session;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreSession.Controllers
{
    public class HomeController : Controller
    {
        private readonly Faker _faker;

        public HomeController()
        {
            _faker = new Faker("en");
        }

        public IActionResult Index()
        {
            var model = new IndexView();

            // tag::getuser[]
            if(HttpContext.Session.Get("user") != null)
                model.User = HttpContext.Session.GetObject<dynamic>("user");
            // end::getuser[]
            if (HttpContext.Session.Get("preferences") != null)
                model.Prefs = HttpContext.Session.GetObject<dynamic>("preferences");
            if (HttpContext.Session.Get("location") != null)
                model.Location = HttpContext.Session.GetObject<dynamic>("location");

            return View(model);
        }

        public RedirectToActionResult AddUserDataToSession()
        {
            // tag::setuser[]
            HttpContext.Session.SetObject("user", new
            {
                UserName = _faker.Internet.UserName(),
                SMS = _faker.Phone.PhoneNumber()
            });
            // end::setuser[]

            return RedirectToAction("Index");
        }

        public RedirectToActionResult AddPreferencesDataToSession()
        {
            HttpContext.Session.SetObject("preferences", new
            {
                Region = _faker.Address.CountryCode(),
                Employer = _faker.Company.CompanyName()
            });

            return RedirectToAction("Index");
        }

        public RedirectToActionResult AddLocationDataToSession()
        {
            HttpContext.Session.SetObject("location", new
            {
                ClosestStoreLocation = _faker.Address.FullAddress(),
                GeoCoordinates = new
                {
                    Lat = _faker.Address.Latitude(),
                    Lon = _faker.Address.Longitude()
                }
            });

            return RedirectToAction("Index");
        }

        public RedirectToActionResult ClearUserDataInSession()
        {
            // tag::clearuser[]
            HttpContext.Session.Remove("user");
            // end::clearuser[]
            return RedirectToAction("Index");
        }

        public RedirectToActionResult ClearPreferencesDataInSession()
        {
            HttpContext.Session.Remove("preferences");
            return RedirectToAction("Index");
        }

        public RedirectToActionResult ClearLocationDataInSession()
        {
            HttpContext.Session.Remove("location");
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
