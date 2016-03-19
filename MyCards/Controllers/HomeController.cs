using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCards.Models;
using System.Data.Entity;

namespace MyCards.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            var addresses = db.Restuarants.Include(a => a.Location).ToArray();
            string addressesString = "";

            for (int i = 0; i < addresses.Count(); i++)
            {
                addressesString += addresses.ElementAt(i).Location.Address + " " + addresses.ElementAt(i).Location.City + "~";
            }

            //ViewBag.restaurantsData = addressesString.ToString();
            ViewBag.restaurantsData = "קדושי מצרים יהוד";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}