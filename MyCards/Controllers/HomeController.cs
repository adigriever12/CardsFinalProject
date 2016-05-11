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
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Hever);

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser && 
                                                                                a.Type == Restaurants_Type.Hever);

            var rankingAvg = db.UserRanking.Where(a => a.Type == Restaurants_Type.Hever)
                .GroupBy(t => new { RestuarantId = t.RestuarantId })
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
                data.address = item.Location.Address;

                #region Ranking

                // If i ranked
                UserRanking findIfRank = userRankingList.ToList().Find(t => t.RestuarantId == item.RestuarantId);

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
                var findAvg = rankingAvg.Find(t => t.RestuarantId == item.RestuarantId);
                if (findAvg == null)
                {
                    data.ratingAvg = 0;
                }
                else
                {
                    data.ratingAvg = Convert.ToInt32(Math.Round(findAvg.Average));
                }
                #endregion

                data.score = ScoreResturant(Restaurants_Type.Hever, item.Location, item.Category,
                                            0, 0, data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        public ActionResult Groupon()
        {
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Groupun);

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                                a.Type == Restaurants_Type.Groupun);

            var rankingAvg = db.UserRanking.Where(a => a.Type == Restaurants_Type.Groupun)
                .GroupBy(t => new { RestuarantId = t.RestuarantId })
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

                UserRanking findIfRank = userRankingList.ToList().Find(t => t.RestuarantId == item.Groupun_RestuarantId);

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

                data.score = ScoreResturant(Restaurants_Type.Groupun, item.Location, item.Category, 0, 0,
                                            data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        public ActionResult Leumi()
        {
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Laumi);

            string curUser = User.Identity.GetUserId();
            IQueryable<UserRanking> userRankingList = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                                a.Type == Restaurants_Type.Laumi);
            var rankingAvg = db.UserRanking.Where(a => a.Type == Restaurants_Type.Laumi)
                .GroupBy(t => new { RestuarantId = t.RestuarantId })
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

                UserRanking findIfRank = userRankingList.ToList().Find(t => t.RestuarantId == item.Laumi_RestuarantId);

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

                data.score = ScoreResturant(Restaurants_Type.Laumi, item.Location, category, 0, 0,
                                            data.ratingAvg, userRankingList, data.myRating);

                addressesList.Add(data);

                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            addressesList.Sort((x, y) => y.score.CompareTo(x.score));

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string addressesString = serializer.Serialize(addressesList);

            ViewBag.restaurantsJsonMap = addressesString;
            ViewBag.restaurantsList = addressesList;
            ViewBag.recommended = recommended;

            return View();
        }

        private List<int> SlopeOneCalcDB(Restaurants_Type type)
        {
            List<int> recommendedIds = new List<int>();
            
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string userid = User.Identity.GetUserId();

                using (var cmd = new SqlCommand("Procedure", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@type", type); // TODO : Add type to the function
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

        private double ScoreResturant(Restaurants_Type type, Location location, Category category, double lat, double lng, int avgRanking, IQueryable<UserRanking> userRankingList, int userRating)
        {
            GeoCoordinate current = new GeoCoordinate();
            current.Latitude = lat;
            current.Longitude = lng;

            GeoCoordinate restaurant = new GeoCoordinate();
            restaurant.Latitude = Convert.ToDouble(location.lat);
            restaurant.Longitude = Convert.ToDouble(location.lng);

            double distance = 0;// 1 / current.GetDistanceTo(restaurant);

            double categoryGreatness = 0;

            var visitsCount = userRankingList.Count();
            var categoryCount = getCategoryCount(type, userRankingList, category);

            if (visitsCount != 0)
            {
                categoryGreatness = categoryCount / visitsCount;
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
        
        public int getCategoryCount(Restaurants_Type type, IQueryable<UserRanking> userRankingList, Category category)
        {
            if (type == Restaurants_Type.Hever)
            {
                var restaurantCatCount = from a in userRankingList
                                         join b in db.Restuarants on a.RestuarantId equals b.RestuarantId
                                         where a.Type == Restaurants_Type.Hever
                                         select new
                                         {
                                             catId = b.Category.CategoryId,
                                         };

                var results = restaurantCatCount.GroupBy(y => y.catId).Select(g => new
                {
                    count = g.Count(),
                    category = g.Key
                }).ToList();

                if (results.Count() != 0 && results.Find(c => c.category == category.CategoryId) != null)
                {
                    return (results.Find(c => c.category == category.CategoryId).count);
                }
                else
                {
                    return 0;
                }
                
            }
            else if (type == Restaurants_Type.Groupun)
            {
                var restaurantCatCount = from a in userRankingList
                                         join b in db.Groupun_Restuarants on a.RestuarantId equals b.Groupun_RestuarantId
                                         where a.Type == Restaurants_Type.Groupun
                                         select new
                                         {
                                             catId = b.Category.CategoryId,
                                         };

                var results = restaurantCatCount.GroupBy(y => y.catId).Select(g => new
                {
                    count = g.Count(),
                    category = g.Key
                }).ToList();

                if (results.Count() != 0 && results.Find(c => c.category == category.CategoryId) != null)
                {
                    return (results.Find(c => c.category == category.CategoryId).count);
                }
                else
                {
                    return 0;
                }

            }
            return 0;
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
        public int UpdateRank(int rank, int restuarantId, int restaurantType)
        {
            string curUser = User.Identity.GetUserId();

            var userRankingList = db.UserRanking.Where(a => a.RestuarantId == restuarantId &&
                                                            a.ApplicationUser.Id == curUser &&
                                                            a.Type == (Restaurants_Type)restaurantType);

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
                newUserRanking.Type = (Restaurants_Type)restaurantType;
                newUserRanking.ApplicationUser = db.Users.Find(curUser);
                newUserRanking.RestuarantId = restuarantId;
                db.UserRanking.Add(newUserRanking);
            }

            db.SaveChanges();

            Restaurants_Type type = (Restaurants_Type)Enum.Parse(typeof(Restaurants_Type), restaurantType.ToString());

            // Return new average
            double rankingAvg = db.UserRanking.Where(a => a.RestuarantId == restuarantId &&
                                                           a.Type == type)
                                             .Average(b => b.Rating);
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