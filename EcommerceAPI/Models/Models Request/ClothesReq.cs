namespace EcommerceAPI.Models.Models_Request
{
    public class ClothesReq
    {
        public int Id { get; set; }
        public int? Idseller { get; set; }
        public int? IdCategory { get; set; }
        public string? Des { get; set; }
        public string? Name { get; set; }
        public List<string> imgUrls { get; set; }
    }
}
