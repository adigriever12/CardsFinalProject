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
            // save card type to view
            ViewBag.cardType = Restaurants_Type.Hever;

            // get new recommeneded restaurants for current user
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Hever);

            string curUser = User.Identity.GetUserId();

            // get all user rankings for current card type
            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Hever).ToDictionary(x => x.RestuarantId);
  
            var addresses = db.Restuarants.Include(a => a.Location).Include(b => b.Cuisine).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            // prepare data to home view
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

                // list is ordered by descending score, and markers are colored by descending score also 
                data.score = data.ratingAvg;

                addressesList.Add(data);

                // add name and image to new recommended restaurant
                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            // sorting restaurant list by score(average)
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
            // save card type to view
            ViewBag.cardType = Restaurants_Type.Groupun;

            // get new recommeneded restaurants for current user
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Groupun);
            
            string curUser = User.Identity.GetUserId();

            // get all user rankings for current card type
            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Groupun).ToDictionary(x => x.RestuarantId);

            var addresses = db.Groupun_Restuarants.Include(a => a.Location).Include(c => c.Category).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            // prepare data to home view
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

                // list is ordered by descending score, and markers are colored by descending score also 
                data.score = data.ratingAvg;

                addressesList.Add(data);

                // add name and image to new recommended restaurant
                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            // sorting restaurant list by score(average)
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
            // save card type to view
            ViewBag.cardType = Restaurants_Type.Laumi;

            // get new recommeneded restaurants for current user
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.Laumi);

            string curUser = User.Identity.GetUserId();

            // get all user rankings for current card type
            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                 a.Type == Restaurants_Type.Laumi).ToDictionary(x => x.RestuarantId);

            var addresses = db.Laumi_Restuarants.Include(a => a.Location).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            // prepare data to home view
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

                // list is ordered by descending score, and markers are colored by descending score also 
                data.score = data.ratingAvg;

                addressesList.Add(data);

                // add name and image to new recommended restaurant
                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            // sorting restaurant list by score(average)
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
            // save card type to view
            ViewBag.cardType = Restaurants_Type.American;

            // get new recommeneded restaurants for current user
            List<int> recommendedIds = SlopeOneCalcDB(Restaurants_Type.American);

            string curUser = User.Identity.GetUserId();

            // get all user rankings for current card type
            var resturantsUserRanked = db.UserRanking.Where(a => a.ApplicationUser.Id == curUser &&
                                                                  a.Type == Restaurants_Type.American).ToDictionary(x => x.RestuarantId);

            var addresses = db.American_Restuarants.Include(a => a.Location).OrderBy(n => n.Name).ToArray();

            List<RestaurantData> addressesList = new List<RestaurantData>();
            List<RecommendedData> recommended = new List<RecommendedData>();

            // prepare data to home view
            foreach (American_Restuarant item in addresses)
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

                // list is ordered by descending score, and markers are colored by descending score also 
                data.score = data.ratingAvg;

                addressesList.Add(data);

                // add name and image to new recommended restaurant
                if (recommendedIds.Contains(data.id))
                {
                    recommended.Add(new RecommendedData(data.id, item.Name, item.Image));
                }
            }

            // sorting restaurant list by score(average)
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
                    cmd.Parameters.AddWithValue("@type", type);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // get the recommended restaurant's Id
                            recommendedIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            
            return recommendedIds;
        }

        [HttpGet]
        public string GetImage(int id, Restaurants_Type restaurantType)
        {
            byte[] image = null;

            if (restaurantType == Restaurants_Type.Hever)
            {
                image = db.Restuarants.Find(id).Image;
            }
            else if (restaurantType == Restaurants_Type.Groupun)
            {
                image = db.Groupun_Restuarants.Find(id).Image;
            }
            else if (restaurantType == Restaurants_Type.Laumi)
            {
                image = db.Laumi_Restuarants.Find(id).Image;
            }
            else if (restaurantType == Restaurants_Type.American)
            {
                image = db.American_Restuarants.Find(id).Image;
            }
            

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
                var row = db.American_Restuarants.Find(restuarantId);
                row.RankningUsersSum++;
                row.RankingsSum += rank;
                db.Entry(row).State = EntityState.Modified;
                return row.RankingsSum / row.RankningUsersSum;
            }

            return 0;
        }

        [HttpPost]
        public int UpdateRank(int rank, int restuarantId, int restaurantType)
        {
            // get user id
            string curUser = User.Identity.GetUserId();

            // updaing restaurants userSum and RankingSum, then calculating the new average
            int average = updateRestaurantsRank((Restaurants_Type)restaurantType, restuarantId, rank);

            // get the user ranking for current restaurant for current card type
            var userRankingList = db.UserRanking.Where(a => a.RestuarantId == restuarantId &&
                                                            a.ApplicationUser.Id == curUser &&
                                                            a.Type == (Restaurants_Type)restaurantType);

            // user already ranked this restaurant, thus update is nedded
            if (userRankingList.Count() > 0)
            {
                UserRanking userRanking = userRankingList.First();
                userRanking.Ranking = rank;
                  
                db.Entry(userRanking).State = EntityState.Modified;
            }
            // first time the user is ranking the restaurant, thus create is needed
            else
            {
                UserRanking newUserRanking = new UserRanking();
                newUserRanking.Ranking = rank;
                newUserRanking.Type = (Restaurants_Type)restaurantType;
                newUserRanking.ApplicationUser = db.Users.Find(curUser);
                newUserRanking.RestuarantId = restuarantId;
                db.UserRanking.Add(newUserRanking);
            }

            // save changes to DB
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