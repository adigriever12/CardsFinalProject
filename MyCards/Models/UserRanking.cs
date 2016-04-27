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
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RestuarantId { get; set; }

        [DefaultValue(0)]
        public double rating { get; set; }
    }
}