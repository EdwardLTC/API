using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Person
    {
        public Person()
        {
            Bills = new HashSet<Bill>();
            Clothes = new HashSet<Clothe>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? Psw { get; set; }
        public string? PhoneNum { get; set; }
        public int? Role { get; set; }
        public string? ImgUrl { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Clothe> Clothes { get; set; }
    }
}
