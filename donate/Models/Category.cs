namespace donate.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }=null!;
        public string CategoryIcon { get; set; } = null!;
        public ICollection<Donation>? Donations { get; set; }
    }
}
