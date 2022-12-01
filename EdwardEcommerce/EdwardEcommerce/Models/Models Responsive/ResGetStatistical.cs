using EcommerceAPI.Models.Models_Respone;
using EdwardEcommerce.Models.Models_Respon;

namespace EdwardEcommerce.Models.Models_Responsive
{
    public class ResGetStatistical
    {
        public Respon _Respon { get; set; }
        public List<ClothesStatisticalRes> ClothesStatisticalRes { get; set; }
    }
}
