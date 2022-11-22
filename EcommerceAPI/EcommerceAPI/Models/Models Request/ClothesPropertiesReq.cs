namespace EcommerceAPI.Models.Models_Request
{
    public class ClothesPropertiesReq
    {
        public int IdClothes { get; set; }
        public string? Size { get; set; }
        public int? Quantily { get; set; }
        public double? Price { get; set; }
    }
}
