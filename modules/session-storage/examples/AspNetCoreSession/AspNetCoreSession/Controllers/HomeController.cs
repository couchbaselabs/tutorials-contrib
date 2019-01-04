using System;
using System.Collections.Generic;
using AspNetCoreSession.Models;
using AspNetCoreSession.Models.Repositories;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Couchbase.Extensions.Session;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreSession.Controllers
{
    public class HomeController : Controller
    {
        private readonly Faker _faker;
        private readonly ISessionStorageRepository _sessionRepo;

        public HomeController(ISessionStorageRepository sessionRepo, Faker faker)
        {
            _faker = faker;
            _sessionRepo = sessionRepo;
        }

        public IActionResult Index()
        {
            var model = new IndexView();

            // tag::getuser[]
            if(HttpContext.Session.Get("user") != null)
                model.User = HttpContext.Session.GetObject<dynamic>("user");
            // end::getuser[]
            if (HttpContext.Session.Get("shoppingcart") != null)
                model.ShoppingCart = HttpContext.Session.GetObject<dynamic>("shoppingcart");
            if (HttpContext.Session.Get("location") != null)
                model.Location = HttpContext.Session.GetObject<dynamic>("location");

            model.SessionCount = _sessionRepo.GetCountOfAllSessions();

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

        public RedirectToActionResult AddShoppingCartDataToSession()
        {
            AddRandomShoppingCartToSession();

            return RedirectToAction("Index");
        }

        private void AddRandomShoppingCartToSession()
        {
            string[] POSSIBLE_SHOPPING_CART_ITEMS = { "Socks", "T-Shirt", "Hat", "Tennis Shoes", "Scarf", "Gloves", "Necklace", "Watch" };
            var cart = new
            {
                DateCreated = DateTime.Now,
                Items = new List<dynamic>()
            };
            var randomNumberOfItems = _faker.Random.Int(1, 3);
            for (var i = 0; i < randomNumberOfItems; i++)
            {
                cart.Items.Add(new
                {
                    ItemName = _faker.PickRandom(POSSIBLE_SHOPPING_CART_ITEMS),
                    Price = _faker.Random.Decimal(0.99M, 49.99M),
                    Quantity = _faker.Random.Int(1, 5)
                });
            }
            HttpContext.Session.SetObject("shoppingcart", cart);
        }

        public RedirectToActionResult AddLocationDataToSession()
        {
            HttpContext.Session.SetObject("location", new
            {
                ClosestStoreLocation = _faker.Address.FullAddress(),
                Country = _faker.Address.Country(),
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

        public RedirectToActionResult ClearShoppingCartDataInSession()
        {
            HttpContext.Session.Remove("shoppingcart");
            return RedirectToAction("Index");
        }

        public RedirectToActionResult ClearLocationDataInSession()
        {
            HttpContext.Session.Remove("location");
            return RedirectToAction("Index");
        }

        public IActionResult NewSession()
        {
            HttpContext.Response.Cookies.Delete(".MyApp.Cookie");
            return RedirectToAction("Index");
        }

        public IActionResult ReportRecentShoppingCarts()
        {
            var recentShoppingCartReport = _sessionRepo.GetReportRecentShoppingCarts();
            return View(recentShoppingCartReport);
        }

        public IActionResult ReportMostCommonShoppingCartItems()
        {
            var mostCommonShoppingCartItems = _sessionRepo.GetReportMostCommonShoppingCartItems();
            return View(mostCommonShoppingCartItems);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
