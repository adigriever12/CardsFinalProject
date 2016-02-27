using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class ProjectDBContext// : DbContext
    {
        public DbSet<Restuarant> Restuarants { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<VisitHistory> VisitHistories { get; set; }  
    }
}