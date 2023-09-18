using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceiptSharing.Api.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // You might want to include an ID property as well

        public string Email { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;

        public string? Username { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public string Image { get; set; } = string.Empty;

        public List<SubscriptionReceipt>? SubscriptionsReceipt { get; set; } // Navigation property

        public List<Receipt>? Receipts { get; set; } // Navigation property

        public List<Rating>? Ratings { get; set; } // Navigation property

        public List<Review>? Reviews { get; set; } // Navigation property
        
        public List<SubscriptionUser>? SubscribedTo {get; set;} // Navigation property
        
        public List<SubscriptionUser>? Subscribers {get; set;} // Navigation property
    }

    public class UserDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // You might want to include an ID property as well

        public string Email { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;

        public string? Username { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; }

        public string Image { get; set; } = string.Empty;

        public List<SubscriptionReceipt>? SubscriptionsReceipt { get; set; }  // Navigation property

        public List<Receipt>? Receipts { get; set; } // Navigation property

        public List<Rating>? Ratings { get; set; } // Navigation property

        public List<Review>? Reviews { get; set; } // Navigation property
        
        public List<SubscriptionUser>? SubscribedTo {get; set;} // Navigation property
        
        public List<SubscriptionUser>? Subscribers {get; set;} // Navigation property
    }

    public class OtherUserDto
    {
        public int Id { get; set; } // You might want to include an ID property as well

        public string Name { get; set; } = string.Empty;

        public string? Username { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; }

        public string Image { get; set; } = string.Empty;
    }

    public class UserProfileDto
    {
        public int Id { get; set; } // You might want to include an ID property as well

        public string Name { get; set; } = string.Empty;

        public string? Username { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; }

        public string Image { get; set; } = string.Empty;

        public int NumberOfRatings { get; set; }

        public int NumberOfReceipts { get; set; }
        
        public int NumberOfSubscriptions { get; set; }
        
        public int NumberOfReviews { get; set; }
    }

    public class ChangeUsernameCommand {
        public required string Username {get; set;}
    }
}