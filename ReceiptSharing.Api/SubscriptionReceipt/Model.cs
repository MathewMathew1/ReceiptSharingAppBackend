using System.ComponentModel.DataAnnotations.Schema;

namespace ReceiptSharing.Api.Models
{
    public class SubscriptionReceipt
    {
        [ForeignKey("User")]
        public int UserId { get; set; } // User who is subscribing
        [ForeignKey("Receipt")]
        public int ReceiptId { get; set; } // Receipt being subscribed to
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SubscriptionStart { get; set; } = DateTime.UtcNow;
        public User? User { get; set; } // Navigation property to User
        public Receipt? Receipt { get; set; }
    }

    public class SubscriptionReceiptDto
    {
        [ForeignKey("User")]
        public int UserId { get; set; } // User who is subscribing
        [ForeignKey("Receipt")]
        public int ReceiptId { get; set; } // Receipt being subscribed to
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SubscriptionStart { get; set; } = DateTime.UtcNow;
        public User? User { get; set; } // Navigation property to User
        public Receipt? Receipt { get; set; }
    }
}