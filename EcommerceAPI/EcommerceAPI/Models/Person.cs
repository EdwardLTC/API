using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Person
    {
        public Person()
        {
            Favorites = new HashSet<Favorite>();
            Vouchers = new HashSet<Voucher>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Role { get; set; }
        public string? Img { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
