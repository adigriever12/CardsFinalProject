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
            List<int> recommendedIds = SlopeOneCalcDB();

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser);

            var rankingAvg = db.UserRanking.GroupBy(t => new { RestuarantId = t.Restuarant.RestuarantId })
                .Select(g => new
                {
                    Average = g.Average(p => p.Rating),
                    RestuarantId = g.Key.RestuarantId
                }).ToList();

            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>(580);
            List<RecommendedData> recommended = new List<RecommendedData>(6);

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
                //UserRanking findIfRank = userRankingList.ToList().Find(t => t.Restuarant.RestuarantId == item.RestuarantId);
                UserRanking findIfRank = new UserRanking();

                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }
                else
                {
                    data.ratedByMe = true;
                    data.myRating = 1;// Convert.ToInt32(Math.Round(findIfRank.Rating)); // TODO : check if we need rating to be double
                }

                // Average ranking
                var findAvg = 3;// rankingAvg.Find(t => t.RestuarantId == item.RestuarantId);
                if (findAvg == null)
                {
                    data.ratingAvg = 0;
                }
                else
                {
                    data.ratingAvg = 3;// Convert.ToInt32(Math.Round(findAvg.Average));
                }
                #endregion

                data.score = 1;// ScoreResturant(item.Location, item.Category, 0, 0, data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        public ActionResult Groupon()
        {
            List<int> recommendedIds = SlopeOneCalcDB();

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser);
            var rankingAvg = db.UserRanking.GroupBy(t => new { RestuarantId = t.Restuarant.RestuarantId })
                .Select(g => new
                {
                    Average = g.Average(p => p.Rating),
                    RestuarantId = g.Key.RestuarantId
                }).ToList();

            var addresses = db.Groupun_Restuarants.Include(a => a.Location).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            foreach (Groupun_Restuarant item in addresses)
            {
                RestaurantData data = new RestaurantData();
                data.id = item.Groupun_RestuarantId;
                data.lat = item.Location.lat;
                data.lng = item.Location.lng;
                data.name = item.Name;
                data.description = item.Description;
                data.copunDescription = item.CopunDescription;
                data.category = item.Category.Name;
                data.kosher = item.Kosher;
                data.expiration = item.Expiration;
                data.openingHours = item.Hours;
                data.phone = item.PhoneAndContent;
                data.address = item.Location.Address;

                #region Ranking

                // If i ranked

                UserRanking findIfRank = null;
                //TODO : fix this and delete the line above
                // UserRanking findIfRank = userRankingList.ToList().Find(t => t.Restuarant.RestuarantId == item.Groupun_RestuarantId);

                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }
                else
                {
                    data.ratedByMe = true;
                    data.myRating = Convert.ToInt32(Math.Round(findIfRank.Rating)); // TODO : check if we need rating to be double
                }

                // Average ranking
                var findAvg = rankingAvg.Find(t => t.RestuarantId == item.Groupun_RestuarantId);
                if (findAvg == null)
                {
                    data.ratingAvg = 0;
                }
                else
                {
                    data.ratingAvg = Convert.ToInt32(Math.Round(findAvg.Average));
                }
                #endregion

                data.score = ScoreResturant(item.Location, item.Category, 0, 0, data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        public ActionResult Leumi()
        {
            List<int> recommendedIds = SlopeOneCalcDB();

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser);
            var rankingAvg = db.UserRanking.GroupBy(t => new { RestuarantId = t.Restuarant.RestuarantId })
                .Select(g => new
                {
                    Average = g.Average(p => p.Rating),
                    RestuarantId = g.Key.RestuarantId
                }).ToList();

            var addresses = db.Laumi_Restuarants.Include(a => a.Location).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            foreach (Laumi_Restuarant item in addresses)
            {
                RestaurantData data = new RestaurantData();
                data.id = item.Laumi_RestuarantId;
                data.lat = item.Location.lat;
                data.lng = item.Location.lng;
                data.name = item.Name;
                data.description = item.Description;
                data.copunDescription = item.Perks;
                data.phone = item.Phone;
                data.address = item.Location.Address;

                #region Ranking

                // If i ranked

                UserRanking findIfRank = null;
                //TODO : fix this and delete the line above
                // UserRanking findIfRank = userRankingList.ToList().Find(t => t.Restuarant.RestuarantId == item.Groupun_RestuarantId);

                if (findIfRank == null)
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }
                else
                {
                    data.ratedByMe = true;
                    data.myRating = Convert.ToInt32(Math.Round(findIfRank.Rating)); // TODO : check if we need rating to be double
                }

                // Average ranking
                var findAvg = rankingAvg.Find(t => t.RestuarantId == item.Laumi_RestuarantId);
                if (findAvg == null)
                {
                    data.ratingAvg = 0;
                }
                else
                {
                    data.ratingAvg = Convert.ToInt32(Math.Round(findAvg.Average));
                }
                #endregion

                Category category = new Category();
                category.CategoryId = 0;

                data.score = ScoreResturant(item.Location, category, 0, 0, data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // serializer.MaxJsonLength = int.MaxValue;
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        private List<int> SlopeOneCalcDB()
        {
            List<int> recommendedIds = new List<int>(6);

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

        private double ScoreResturant(Location location, Category category, double lat, double lng, int avgRanking, IQueryable<UserRanking> userRankingList, int userRating)
        {
            GeoCoordinate current = new GeoCoordinate();
            current.Latitude = lat;
            current.Longitude = lng;

            GeoCoordinate restaurant = new GeoCoordinate();
            restaurant.Latitude = Convert.ToDouble(location.lat);
            restaurant.Longitude = Convert.ToDouble(location.lng);

            double distance = 0;// 1 / current.GetDistanceTo(restaurant);

            double categoryGreatness = 0;

            if (userRankingList.Where(x => x.Restuarant.Category.CategoryId == category.CategoryId).Count() > 0)
            {
                var results = userRankingList.Include(x => x.Restuarant).GroupBy(y => y.Restuarant.Category).Select(g => new
                {
                    count = g.Count(),
                    category = g.Key.CategoryId
                }).ToList();

                var r = results.Find(c => c.category == category.CategoryId).count;

                var visitsCount = userRankingList.Count();

                categoryGreatness = r / visitsCount;

            }

            double badRating = 0;

            if (userRating == 1 || userRating == 2)
            {
                badRating = (-1) * (1 / userRating);
            }

            double result = distance + avgRanking + categoryGreatness + badRating;
            
            double normalizeMinMax = Math.Round((result - (-1)) / (7 - (-1)));

            int markerCategoryByScore = Convert.ToInt32(Math.Round(normalizeMinMax * 5));

            return markerCategoryByScore;
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


        [HttpPost]
        public int UpdateRank(int rank, int restuarantId)
        {
            string curUser = User.Identity.GetUserId();

            var userRankingList = db.UserRanking.Include(x => x.Restuarant).Where(a => a.Restuarant.RestuarantId == restuarantId && a.ApplicationUser.Id == curUser);

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
                newUserRanking.Restuarant = db.Restuarants.Find(restuarantId);
                newUserRanking.ApplicationUser = db.Users.Find(curUser);

                db.UserRanking.Add(newUserRanking);
            }

            db.SaveChanges();

            // Return new average
            double rankingAvg = db.UserRanking.Where(a => a.Restuarant.RestuarantId == restuarantId).Average(b => b.Rating);
            return Convert.ToInt32(Math.Floor(rankingAvg));
        }

        public ActionResult _List(double lat, double lng)
        {

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
        public double score;
        public bool ratedByMe;
        public string address;
        public int myRating;
        public int ratingAvg;
        public string copunDescription;
        public string expiration;
    }
}