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

        public ActionResult Index()
        {
/*
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (var cmd = new SqlCommand("Procedure", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user", 5);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // do something with the row
                            var s1 = reader.GetInt32(0);
                            var s2 = reader.GetDouble(3);

                            
                        }
                    }
                }
            }

    */




            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            string curUser = User.Identity.GetUserId();
            var userRankingList = db.UserRanking.Where(a => a.UserId == curUser).ToList();

            List<RestaurantData> addressesList = new List<RestaurantData>();

            foreach (Restuarant item in addresses)
            {
                RestaurantData data = new RestaurantData();
                data.id = item.RestuarantId;
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
                data.score = item.Score;
                data.address = item.Location.Address;

                var findIfRank = userRankingList.Find(t => t.RestuarantId == item.RestuarantId);
                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                }
                else
                {
                    data.ratedByMe = true;
                }

                addressesList.Add(data);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsData = addressesString;

            //ViewBag.restaurants = addresses;
            ViewBag.restaurants = addressesList;

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


        [HttpPost]
        public void UpdateRank(int rank, int restuarantId)
        {
            string curUser = User.Identity.GetUserId();
            
            var userRankingList = db.UserRanking.Where(a => a.RestuarantId == restuarantId && a.UserId == curUser);
            var t = db.Restuarants.Count();
            if (userRankingList.Count() > 0)
            {
                UserRanking userRanking = userRankingList.First();
                userRanking.rating = rank;

                db.Entry(userRanking).State = EntityState.Modified;
            }
            else
            {
                UserRanking newUserRanking = new UserRanking();
                newUserRanking.rating = rank;
                newUserRanking.RestuarantId = restuarantId;
                newUserRanking.UserId = curUser;

                db.UserRanking.Add(newUserRanking);
            }

            db.SaveChanges();

        }
    }
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
    public bool ratedByMe;
    public string address;
}