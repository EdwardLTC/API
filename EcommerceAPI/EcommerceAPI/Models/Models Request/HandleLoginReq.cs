namespace EcommerceAPI.Models.Models_Respone
{
    public class HandleLoginReq
    {
        public String _Email { get; set; }
        public String _password { get; set; }

        public HandleLoginReq(String mail, string pasaword)
        {
            _Email = mail;
            _password = pasaword;
        }
    }
}
