using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Clothe
    {
        public Clothe()
        {
            ImgUrls = new HashSet<ImgUrl>();
            Idpeople = new HashSet<Person>();
        }

        public int Id { get; set; }
        public int? Idseller { get; set; }
        public int? IdCategory { get; set; }
        public string? Des { get; set; }
        public string? Name { get; set; }

        public virtual Category? IdCategoryNavigation { get; set; }
        public virtual Person? IdsellerNavigation { get; set; }
        public virtual ClothesProperty? ClothesProperty { get; set; }
        public virtual ICollection<ImgUrl> ImgUrls { get; set; }

        public virtual ICollection<Person> Idpeople { get; set; }
    }
}
