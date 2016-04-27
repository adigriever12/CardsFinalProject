using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyCards.Models
{
    public class UserRankingData
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RestuarantId { get; set; }

        [DefaultValue(0)]
        [Range(0, 5)]
        public double rating { get; set; }
    }
}