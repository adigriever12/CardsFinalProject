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
        public int UserRankingId { get; set; } //key
        
        public ApplicationUser ApplicationUser { get; set; }

        public int RestuarantId { get; set; }

        [DefaultValue(0)]
        [Range(0, 5)]
        public int Ranking { get; set; }

        public Restaurants_Type Type {get;set;}
    }

    public enum Restaurants_Type
    {
        Hever = 0,
        Groupun = 1,
        Laumi = 2,
        American = 3
    }
}