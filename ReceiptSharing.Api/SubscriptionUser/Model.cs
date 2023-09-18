using System.ComponentModel.DataAnnotations.Schema;

namespace ReceiptSharing.Api.Models
{
    public class SubscriptionUser
    {
        [ForeignKey("UserId")]
        public int UserId { get; set; } // User who is subscribing
        [ForeignKey("UserSubscribedToId")]
        public int UserSubscribedToId { get; set; } // Receipt being subscribed to
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SubscriptionStart { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public User? User { get; set; } // Navigation property to User
        [ForeignKey("UserSubscribedToId")]
        public User? UserSubscribedTo { get; set; }
        
    }

    public class SubscriptionUserDto
    {
        public int UserId { get; set; } // User who is subscribing
        public int UserSubscribedToId { get; set; } // Receipt being subscribed to
        public DateTime SubscriptionStart { get; set; } = DateTime.UtcNow;
        public OtherUserDto? User { get; set; } // Navigation property to User
        public OtherUserDto? UserSubscribedTo { get; set; }
    }
}