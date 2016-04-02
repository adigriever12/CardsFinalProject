using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCards.Models;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Net;
using System.Xml.Linq;

namespace MyCards.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();
            List<locationGmaps> addressesList = new List<locationGmaps>();

            foreach (Restuarant item in addresses)
            {
                locationGmaps location = new locationGmaps();
                location.id = item.Location.LocationId;
                location.lat = item.Location.lat;
                location.lng = item.Location.lng;
                location.name = item.Name;
                location.openingHours = item.OpeningHours;
                location.description = item.Description;
                location.cuisine = item.Cuisine.Name;
                location.category = item.Category.Name;
                location.kosher = item.Kosher;
                location.phone = item.Phone;
                location.handicapAccessibility = item.HandicapAccessibility;

                /*location.image = "";
                if (item.Image != null)
                {
                    location.image = Convert.ToBase64String(item.Image);
                }*/
                addressesList.Add(location);
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
           // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);
            
            ViewBag.restaurantsData = addressesString;

            ViewBag.restaurants = addresses;

            return View();
        }

        [HttpGet]
        public string GetImage(int id)
        {
            var image = db.Restuarants.Find(id).Image;

            if (image != null)
            {
                return Convert.ToBase64String(image);
            }
            
            return "";
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
public class locationGmaps
{
    public string lat;
    public string lng;
    public string name;
    public string openingHours;
    public string description;
   // public string image;
    public int id;
    public string cuisine;
    public string category;
    public string kosher;
    public string phone;
    public bool   handicapAccessibility;
}