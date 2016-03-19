﻿using System;
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

        public string Address { get; set; }
        public Cuisine Cuisine { get; set; }

        public Category Category { get; set; }

        public string Kosher { get; set; }
        public string Phone { get; set; }
        public bool HandicapAccessibility { get; set; }
        public byte[] Image { get; set; }
        public string OpeningHours { get; set; }
    
    }
}