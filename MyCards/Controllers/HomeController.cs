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

            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Hever).ToDictionary(x => x.RestuarantId);//.Select(x => new Tuple<int, int>(x.RestuarantId, x.Ranking));
  
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

                if (resturantsUserRanked.ContainsKey(item.RestuarantId))
                {
                    data.ratedByMe = true;
                    data.myRating = resturantsUserRanked[item.RestuarantId].Ranking;
                }
                else
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }

                data.ratingAvg = item.RankningUsersSum == 0 ? 0 : (int)(item.RankingsSum / item.RankningUsersSum);


                #endregion

                data.score = ScoreResturant(data.ratingAvg, data.myRating);

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

            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Groupun).ToDictionary(x => x.RestuarantId);

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

                if (resturantsUserRanked.ContainsKey(item.Groupun_RestuarantId))
                {
                    data.ratedByMe = true;
                    data.myRating = resturantsUserRanked[item.Groupun_RestuarantId].Ranking;
                }
                else
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }

                data.ratingAvg = item.RankningUsersSum == 0 ? 0 : (int)(item.RankingsSum / item.RankningUsersSum);
                #endregion

                data.score = ScoreResturant(data.ratingAvg, data.myRating);

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

            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Laumi).ToDictionary(x => x.RestuarantId);

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

                if (resturantsUserRanked.ContainsKey(item.Laumi_RestuarantId))
                {
                    data.ratedByMe = true;
                    data.myRating = resturantsUserRanked[item.Laumi_RestuarantId].Ranking;
                }
                else
                {
                    data.ratedByMe = false;
                    data.myRating = 0;
                }

                data.ratingAvg = item.RankningUsersSum == 0 ? 0 : (int)(item.RankingsSum / item.RankningUsersSum);
                #endregion

                Category category = new Category();
                category.CategoryId = 0;

                data.score = ScoreResturant(data.ratingAvg, data.myRating);

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

        public ActionResult American()
        {
            /* List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.American);

             string curUser = User.Identity.GetUserId();

             var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                  a.Type == Restaurants_Type.American).ToDictionary(x => x.RestuarantId);

             var addresses = db.American_Restuarants.Include(a => a.Location).OrderBy(n => n.Name).ToArray();

             List<RestaurantData> addressesList = new List<RestaurantData>();
             List<RecommendedData> recommended = new List<RecommendedData>();

             foreach (Laumi_Restuarant item in addresses)
             {
                 RestaurantData data = new RestaurantData();
                 data.id = item.American_RestuarantId;
                 data.lat = item.Location.lat;
                 data.lng = item.Location.lng;
                 data.name = item.Name;
                 data.description = item.Description;
                 data.copunDescription = item.Perks;
                 data.phone = item.Phone;
                 data.address = item.Location.Address;
                 data.expiration = item.Expiration;

                 #region Ranking

                 if (resturantsUserRanked.ContainsKey(item.American_RestuarantId))
                 {
                     data.ratedByMe = true;
                     data.myRating = resturantsUserRanked[item.American_RestuarantId].Ranking;
                 }
                 else
                 {
                     data.ratedByMe = false;
                     data.myRating = 0;
                 }

                 data.ratingAvg = item.RankningUsersSum == 0 ? 0 : (int)(item.RankingsSum / item.RankningUsersSum);
                 #endregion

                 Category category = new Category();
                 category.CategoryId = 0;

                 data.score = ScoreResturant(data.ratingAvg, data.myRating);

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
              */
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
                            var rating = reader.GetInt32(3);
                        }
                    }
                }
            }
            
            return recommendedIds;
        }

        private double ScoreResturant(int avgRanking, int userRating)
        {
            double badRating = 0;

            if (userRating == 1 || userRating == 2)
            {
                badRating = (-1) * (1 / userRating);
            }

            double result = avgRanking + badRating;
            
            double normalizeMinMax = Math.Round((result - (-1)) / (7 - (-1)));

            int markerCategoryByScore = Convert.ToInt32(Math.Round(normalizeMinMax * 5));

            return markerCategoryByScore;
        }
        
        public int getCategoryCount(Restaurants_Type type, IQueryable<UserRanking> userRankingList, Category category)
        {
            /*if (type == Restaurants_Type.Hever)
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

            }*/
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

        private int updateRestaurantsRank(Restaurants_Type restaurantType, int restuarantId, int rank)
        {
            if (restaurantType == Restaurants_Type.Hever)
            {
                var row = db.Restuarants.Find(restuarantId);
                row.RankningUsersSum++;
                row.RankingsSum += rank;
                db.Entry(row).State = EntityState.Modified;
                return row.RankingsSum / row.RankningUsersSum;
            }
            else if (restaurantType == Restaurants_Type.Groupun)
            {
                var row = db.Groupun_Restuarants.Find(restuarantId);
                row.RankningUsersSum++;
                row.RankingsSum += rank;
                db.Entry(row).State = EntityState.Modified;
                return row.RankingsSum / row.RankningUsersSum;
            }
            else if (restaurantType == Restaurants_Type.Laumi)
            {
               var row = db.Laumi_Restuarants.Find(restuarantId);
                row.RankningUsersSum++;
                row.RankingsSum += rank;
                db.Entry(row).State = EntityState.Modified;
                return row.RankingsSum / row.RankningUsersSum;
            }
            else if(restaurantType == Restaurants_Type.American)
            {
                //var row = db.American_Restuarants.Find(restuarantId);
                //row.RankningUsersSum++;
                //row.RankingsSum += rank;
                //db.Entry(row).State = EntityState.Modified;
                //return row.RankingsSum / row.RankningUsersSum;
            }

            return 0;
        }

        [HttpPost]
        public int UpdateRank(int rank, int restuarantId, int restaurantType)
        {
            string curUser = User.Identity.GetUserId();

            int average = updateRestaurantsRank((Restaurants_Type)restaurantType, restuarantId, rank);

            var userRankingList = db.UserRanking.Where(a => a.RestuarantId == restuarantId &&
                                                            a.ApplicationUser.Id == curUser &&
                                                            a.Type == (Restaurants_Type)restaurantType);

            if (userRankingList.Count() > 0)
            {
                UserRanking userRanking = userRankingList.First();
                userRanking.Ranking = rank;
                  
                db.Entry(userRanking).State = EntityState.Modified;
            }
            else
            {
                UserRanking newUserRanking = new UserRanking();
                newUserRanking.Ranking = rank;
                newUserRanking.Type = (Restaurants_Type)restaurantType;
                newUserRanking.ApplicationUser = db.Users.Find(curUser);
                newUserRanking.RestuarantId = restuarantId;
                db.UserRanking.Add(newUserRanking);
            }

            db.SaveChanges();

            // Return new average
            return average;
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