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
        // TODO:: add image

        public Location Location { get; set; }
        public Cuisine Cuisine { get; set; }
    }
}