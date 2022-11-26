namespace EcommerceAPI.Models.Models_Respone
{
    public class PersonRes
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? PhoneNum { get; set; }
        public int? Role { get; set; }
        public string? ImgUrl { get; set; }
        public string? Address { get; set; }

        public PersonRes(string? name, string? mail, string? phoneNum, int? role, string? imgUrl, string? address)
        {
            Name = name;
            Mail = mail;
            PhoneNum = phoneNum;
            Role = role;
            ImgUrl = imgUrl;
            Address = address;
        }

        public PersonRes(int id, string? name, string? mail, string? phoneNum, int? role, string? imgUrl, string? address)
        {
            Id = id;
            Name = name;
            Mail = mail;
            PhoneNum = phoneNum;
            Role = role;
            ImgUrl = imgUrl;
            Address = address;
        }

        public PersonRes()
        {
        }
    }
}
