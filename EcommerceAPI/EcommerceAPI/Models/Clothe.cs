using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public partial class Clothe
    {
        public Clothe()
        {
            Favorites = new HashSet<Favorite>();
            ImgUrls = new HashSet<ImgUrl>();
        }

        public int Id { get; set; }
        public int? Idseller { get; set; }
        public int? IdCategory { get; set; }
        public string? Des { get; set; }
        public string? Name { get; set; }

        public virtual Category? IdCategoryNavigation { get; set; }
        public virtual Person? IdsellerNavigation { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<ImgUrl> ImgUrls { get; set; }
    }
}
