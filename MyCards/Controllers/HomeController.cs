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
using System.Device.Location;

namespace MyCards.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        private void trymethod(bool calcScore, double lat, double lng)
        {
            List<int> recommendedIds = SlopeOneCalcDB();

            string curUser = User.Identity.GetUserId();
            var userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser).ToList();
            var rankingAvg = db.UserRanking.GroupBy(t => new { RestuarantId = t.Restuarant.RestuarantId })
                .Select(g => new
                {
                    Average = g.Average(p => p.Rating),
                    RestuarantId = g.Key.RestuarantId
                }).ToList();

            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
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
                //data.score = item.Score;
                data.address = item.Location.Address;       

                #region Ranking

                // If i ranked
                UserRanking findIfRank = userRankingList.Find(t => t.Restuarant.RestuarantId == item.RestuarantId);

                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }
                else
                {
                    //data.category

                    data.ratedByMe = true;
                    data.myRating = Convert.ToInt32(Math.Floor(findIfRank.Rating)); // TODO : check if we need rating to be double
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
                #endregion

                if (calcScore)
                {
                    data.score = ScoreResturant(item, lat, lng, data.ratingAvg, userRankingList);
                }

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;
        }

        private List<int> SlopeOneCalcDB()
        {
            List<int> recommendedIds = new List<int>();

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

            return recommendedIds;
        }

        private int ScoreResturant(Restuarant item, double lat, double lng, int avgRanking, List<UserRanking> userRankingList)
        {

            double score = 0;

            string curUser = User.Identity.GetUserId();

            GeoCoordinate current = new GeoCoordinate();
            current.Latitude = lat;
            current.Longitude = lng;

            GeoCoordinate restaurant = new GeoCoordinate();
            restaurant.Latitude = Convert.ToDouble(item.Location.lat);
            restaurant.Longitude = Convert.ToDouble(item.Location.lng);

            double distance = current.GetDistanceTo(restaurant);



            //score = 50 * distance + 20 * avgRanking + 

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

            var userRankingList = db.UserRanking.Where(a => a.Restuarant.RestuarantId == restuarantId && a.ApplicationUser.Id == curUser);
            var t = db.Restuarants.Count();
            if (userRankingList.Count() > 0)
            {
                UserRanking userRanking = userRankingList.First();
                userRanking.Rating = rank;

                db.Entry(userRanking).State = EntityState.Modified;
            }
            else
            {
                UserRanking newUserRanking = new UserRanking();
                newUserRanking.Rating = rank;
                newUserRanking.Restuarant.RestuarantId = restuarantId;
                newUserRanking.ApplicationUser.Id = curUser;

                db.UserRanking.Add(newUserRanking);
            }

            db.SaveChanges();

            // Return new average
            double rankingAvg = db.UserRanking.Where(a => a.Restuarant.RestuarantId == restuarantId).Average(b => b.Rating);
            return Convert.ToInt32(Math.Floor(rankingAvg));
        }

        public ActionResult _List(double lat, double lng)
        {
            trymethod(true, lat, lng);

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