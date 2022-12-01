namespace EdwardEcommerce.Models
{
    public partial class Category
    {
        public Category()
        {
            Clothes = new HashSet<Clothe>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Clothe> Clothes { get; set; }
    }
}
