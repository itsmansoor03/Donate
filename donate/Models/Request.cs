using System.ComponentModel.DataAnnotations.Schema;

namespace donate.Models
{
    public class Request
    {
        public int Id { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? RequestStatus { get; set; }
        public string? Requestdetils { get; set; }

        //navigation
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Donation))]
        public int DonationId { get; set; }

        public Donation? Donation { get; set; }
        public User? User { get; set; }
        
    }
}
