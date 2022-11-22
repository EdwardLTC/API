namespace EcommerceAPI.Models.Models_Request
{
    public class BillReq
    {
        public int Id { get; set; }
        public int? Iduser { get; set; }
        public int? Idvoucher { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateReceived { get; set; }
        public string? Status { get; set; }
        public List<BillDetailReq> ListBillDetailReq { get; set; }
       
    }
}
