using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class Restuarant
    {
        public int RestuarantId { get; set; }
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public Location Location { get; set; }
        public Cuisine Cuisine { get; set; }

        public Category Category { get; set; }

        public int RankingsSum { get; set; }

        public int RankningUsersSum { get; set; }

        public string Kosher { get; set; }
        public string Phone { get; set; }
        public bool HandicapAccessibility { get; set; }
        public byte[] Image { get; set; }
        public string OpeningHours { get; set; }
    }
}