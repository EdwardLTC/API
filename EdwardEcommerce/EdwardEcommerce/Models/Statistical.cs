using Microsoft.EntityFrameworkCore;

namespace EdwardEcommerce.Models
{
    [Keyless]
    public class Statistical
    {
        public int Quantity { get; set; }
        public int IDClothes { get; set; }
    }
}
