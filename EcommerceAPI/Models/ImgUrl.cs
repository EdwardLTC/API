using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class ImgUrl
    {
        public int Id { get; set; }
        public int? Idclothes { get; set; }
        public string? ImgUrl1 { get; set; }

        public virtual Clothe? IdclothesNavigation { get; set; }
    }
}
