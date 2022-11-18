namespace EcommerceAPI.Models.Models_Request
{
    public class PersonReq
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? Psw { get; set; }
        public string? PhoneNum { get; set; }
        public int? Role { get; set; }
        public string? ImgUrl { get; set; }
        public string? Address { get; set; }
    }
}
