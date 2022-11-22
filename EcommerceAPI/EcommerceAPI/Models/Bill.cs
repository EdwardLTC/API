using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public int Id { get; set; }
        public int? Iduser { get; set; }
        public int? Idvoucher { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateReceived { get; set; }
        public string? Status { get; set; }

        public virtual Voucher? IdvoucherNavigation { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
