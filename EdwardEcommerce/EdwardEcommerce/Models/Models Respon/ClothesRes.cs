namespace EcommerceAPI.Models.Models_Respon
{
    public class ClothesRes
    {
        public int Id { get; set; }
        public int? Idseller { get; set; }
        public int? IdCategory { get; set; }
        public string? Des { get; set; }
        public string? Name { get; set; }
        public List<String>? imgsUrl { get; set; }
        public int? quantily { get; set; }
        public string? maxPrice { get; set; }
        public string CategoryName { get; set; }
        public string size { get; set; }
    }
}
