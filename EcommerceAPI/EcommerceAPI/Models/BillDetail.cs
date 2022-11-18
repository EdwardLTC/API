using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class BillDetail
    {
        public int Idbill { get; set; }
        public int Idclothes { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }

        public virtual Bill IdbillNavigation { get; set; } = null!;
        public virtual ClothesProperty IdclothesNavigation { get; set; } = null!;
    }
}
