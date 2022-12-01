namespace EdwardEcommerce.Models
{
    public partial class Favorite
    {
        public int Id { get; set; }
        public int? Idclothes { get; set; }
        public int? Iduser { get; set; }

        public virtual Clothe? IdclothesNavigation { get; set; }
        public virtual Person? IduserNavigation { get; set; }
    }
}
