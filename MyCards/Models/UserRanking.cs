using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class UserRanking
    {
        public int UserRankingId { get; set; }

        [Key]
        [Column(Order = 1)]
        public ApplicationUser ApplicationUser { get; set; }

        [Key]
        [Column(Order = 2)]
        public Restuarant Restuarant { get; set; }

        [DefaultValue(0)]
        [Range(0, 5)]
        public double Rating { get; set; }

        //public Restaurants_Type Ty{get;set;}
    }

    public enum Restaurants_Type
    {
        Hever = 0,
        Groupun = 1,
        Laumi = 2,
        American = 3
    }
}