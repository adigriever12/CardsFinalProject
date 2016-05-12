using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class Groupun_Restuarant
    {
        public int Groupun_RestuarantId { get; set; }
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string CopunDescription { get; set; }

        public Location Location { get; set; }

        public Category Category { get; set; }

        public string Kosher { get; set; }

        [DataType(DataType.MultilineText)]
        public string Expiration { get; set; }

        [DataType(DataType.MultilineText)]
        public string Hours { get; set; }

        [DataType(DataType.MultilineText)]
        public string PhoneAndContent { get; set; }

        public byte[] Image { get; set; }

        public int RankingsSum { get; set; }

        public int RankningUsersSum { get; set; }

    }
}