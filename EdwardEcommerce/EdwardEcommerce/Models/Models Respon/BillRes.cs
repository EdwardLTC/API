namespace EcommerceAPI.Models.Models_Respon
{
    public class BillRes
    {
        public int Id { get; set; }
        public int? Iduser { get; set; }
        public int? Idseller { get; set; }
        public int? Idvoucher { get; set; }
        public string? DateCreate { get; set; }
        public string? Status { get; set; }
        public string? SellerName { get; set; }
        public string? UserAddress { get; set; }
    }
}
