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
            var addresses = db.Restuarants.Include(a => a.Location).Take(10).ToArray();
            List<locationGmaps> addressesList = new List<locationGmaps>();

            for (int i = 0; i < addresses.Count(); i++)
            {
                // TODO : this needs to be in parseData
                var address = addresses.ElementAt(i).Location.Address.Replace("\r", "");
                var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                while (xdoc.Element("GeocodeResponse").Element("status").Value == "OVER_QUERY_LIMIT")
                {
                    address = addresses.ElementAt(i).Location.Address.Replace("\r", "");
                    requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
                    request = WebRequest.Create(requestUri);
                    response = request.GetResponse();
                    xdoc = XDocument.Load(response.GetResponseStream());
                }
                if (xdoc.Element("GeocodeResponse").Element("status").Value == "OK")
                {
                    var result = xdoc.Element("GeocodeResponse").Element("result");
                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");

                    locationGmaps location = new locationGmaps();
                    location.lat = lat.Value;
                    location.lng = lng.Value;
                    location.name = addresses[i].Name;

                    addressesList.Add(location);
                }
                else if (xdoc.Element("GeocodeResponse").Element("status").Value == "ZERO_RESULTS")
                {
                   //error in address - TODO
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string addressesString = serializer.Serialize(addressesList);
            ViewBag.restaurantsData = addressesString;   
            
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
public class locationGmaps
{
    public string lat;
    public string lng;
    public string name;
}