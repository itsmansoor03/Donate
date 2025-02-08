using System.ComponentModel.DataAnnotations.Schema;

namespace donate.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        //navigation
        public string? DonationStatus { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }


        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Request>? Requests { get; set; }

    }
}
