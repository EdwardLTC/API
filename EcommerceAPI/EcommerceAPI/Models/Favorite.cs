﻿using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Favorite
    {
        public int Id { get; set; }
        public int? Idclothes { get; set; }
        public int? Iduser { get; set; }

        public virtual Clothe? IdclothesNavigation { get; set; }
    }
}