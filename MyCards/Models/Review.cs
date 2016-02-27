using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public Restuarant Restuarant { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        public ApplicationUser User { get; set; }
    }
}