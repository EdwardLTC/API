namespace EcommerceAPI.Models.Models_Respon
{
    public class BillRes
    {
        public int Id { get; set; }
        public int? Iduser { get; set; }
        public int? Idvoucher { get; set; }
        public DateTime? DateCreate { get; set; }
        public string? Status { get; set; }
    }
}
