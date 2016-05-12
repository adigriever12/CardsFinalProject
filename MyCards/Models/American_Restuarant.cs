using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class American_Restuarant
    {
        public int American_RestuarantId { get; set; }

        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Perks { get; set; }

        public Location Location { get; set; }

        public string Phone { get; set; }

        public string Expiration { get; set; }

        public byte[] Image { get; set; }

        public int RankingsSum { get; set; }

        public int RankningUsersSum { get; set; }
    }
}