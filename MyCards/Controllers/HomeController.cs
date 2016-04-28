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
        List<RestaurantData> addressesList;
        public ActionResult Index(string currentLocation)
        {
            List<int> recommendedIds = new List<int>();
            addressesList = new List<RestaurantData>();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string userid = User.Identity.GetUserId();

                using (var cmd = new SqlCommand("Procedure", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userid);
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

            string curUser = User.Identity.GetUserId();
            var userRankingList = db.UserRanking.Where(a => a.UserId == curUser).ToList();
            var rankingAvg = db.UserRanking.GroupBy(t => new { RestuarantId = t.RestuarantId })
                .Select(g => new
                {
                    Average = g.Average(p => p.rating),
                    RestuarantId = g.Key.RestuarantId
                }).ToList();

            
            List<RecommendedData> recommended = new List<RecommendedData>();

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

                // If i ranked
                var findIfRank = userRankingList.Find(t => t.RestuarantId == item.RestuarantId);
                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }
                else
                {
                    data.ratedByMe = true;
                    data.myRating = Convert.ToInt32(Math.Floor(findIfRank.rating)); // TODO : check if we need rating to be double
                }

                // Average ranking
                var findAvg = rankingAvg.Find(t => t.RestuarantId == item.RestuarantId);
                if (findAvg == null)
                {
                    data.ratingAvg = 0;
                }
                else
                {
                    data.ratingAvg = Convert.ToInt32(Math.Floor(findAvg.Average));
                }



                data.score = ScoreResturant(item);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsData = addressesString;

            //ViewBag.restaurants = addresses;
            ViewBag.restaurants = addressesList;
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


        [HttpPost]
        public int UpdateRank(int rank, int restuarantId)
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

            // Return new average
            double rankingAvg = db.UserRanking.Where(a => a.RestuarantId == restuarantId).Average(b => b.rating);
            return Convert.ToInt32(Math.Floor(rankingAvg));
        }

        public ActionResult _List(string lat, string lng)
         {
            ViewBag.restaurants = addressesList;

            return PartialView();
        }
    }
    public class RecommendedData
    {
        public RecommendedData(int id, string name, byte[] image)
        {
            Id = id;
            Name = name;
            if (image != null)
            {
                Image = Convert.ToBase64String(image);
            }
        }

        public int Id;
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
        public bool handicapAccessibility;
        public int score;
        public bool ratedByMe;
        public string address;
        public int myRating;
        public int ratingAvg;
    }
}