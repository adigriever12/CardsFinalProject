using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class VisitHistory
    {
        public int VisitHistoryId { get; set; }
        public ApplicationUser User { get; set; }
        public Restuarant Restuarant { get; set; }
    }
}