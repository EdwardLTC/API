namespace EdwardEcommerce.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Bills = new HashSet<Bill>();
        }

        public int Id { get; set; }
        public int? Idseller { get; set; }
        public int? Ratio { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
