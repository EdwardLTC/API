using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class ClothesProperty
    {
        public ClothesProperty()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public int Id { get; set; }
        public int? Idclothes { get; set; }
        public string? Size { get; set; }
        public int? Quantily { get; set; }
        public double? Price { get; set; }

        public virtual Clothe IdNavigation { get; set; } = null!;
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
