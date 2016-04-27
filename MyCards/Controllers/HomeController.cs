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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.AspNet.Identity;

namespace MyCards.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string currentLocation)
        {
            List<int> recommendedIds = new List<int>();

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string userid = User.Identity.GetUserId();

                using (var cmd = new SqlCommand("Procedure", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", "5");
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // do something with the row
                            recommendedIds.Add(reader.GetInt32(0));
                            var rating = reader.GetDouble(3);
                        }
                    }
                }
            }
            
            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            foreach (Restuarant item in addresses)
            {
                RestaurantData data = new RestaurantData();
                data.id = item.Location.LocationId;
                data.lat = item.Location.lat;
                data.lng = item.Location.lng;
                data.name = item.Name;
                data.openingHours = item.OpeningHours;
                data.description = item.Description;
                data.cuisine = item.Cuisine.Name;
                data.category = item.Category.Name;
                data.kosher = item.Kosher;
                data.phone = item.Phone;
                data.handicapAccessibility = item.HandicapAccessibility;

                data.score = ScoreResturant(item);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(item.Name, item.Image));
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsData = addressesString;
            ViewBag.restaurants = addresses;
            ViewBag.recommended = recommended;

            return View();
        }

        private int ScoreResturant(Restuarant item)
        {
            

            return 1;
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

public class RecommendedData
{
    public RecommendedData(string name, byte[] image)
    {
        Name = name;
        if (image != null)
        {
            Image = Convert.ToBase64String(image);
        }
    }

    public string Name;
    public string Image;
}

public class RestaurantData
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
    public int score;
}