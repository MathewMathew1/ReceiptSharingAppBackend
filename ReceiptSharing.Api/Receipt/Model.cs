using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReceiptSharing.Api.Repositories.Utils;

namespace ReceiptSharing.Api.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; } 
        public required string Title {get; set;}
        public required string Description {get; set;}
        public required string[] Steps { get; set; }
        public required string[] ImageLinks { get; set; }
        public string? VideoLink { get; set; }
        public int MinCookDuration { get; set; }
        public int MaxCookDuration { get; set; }
        public required ICollection<Ingredient> Ingredients { get; set; }
        public List<SubscriptionReceipt>? SubscriptionsReceipt { get; set; }
        public List<Rating>? Ratings { get; set; } 
        public List<Review>? Reviews { get; set; } 
        public User? User { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public class ReceiptDto
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public required string Title {get; set;}
        public required string Description {get; set;}
        public required string[] Steps { get; set; }
        public required string[] ImageLinks { get; set; }
        public required string VideoLink { get; set; }
        public int MinCookDuration { get; set; }
        public int MaxCookDuration { get; set; }
        public required ICollection<Ingredient> Ingredients { get; set; } 
        public int NumberOfSubscriptions { get; set; } 
        public double AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
        public List<ReviewInReceipt>? Reviews { get; set; } 
        public OtherUserDto? User { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CreateReceiptCommand
    {
        [StringLength(64)]
        [Required]
        public required string Title { get; set; }

        [StringLength(1048)]
        [Required]
        public required string Description { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        [Required]
        public required List<string> Steps { get; set; }

        [MinLength(1)]
        [MaxLength(5)]
        [Required]
        public required List<IFormFile> Images { get; set; }

        public string? VideoLink { get; set; }  

        [Range(1,1000)] 
        [Required]
        public int MinCookDuration { get; set; }

        [Range(1,1000)] 
        [Required]
        public int MaxCookDuration { get; set; }

        [Required]
        public required List<Ingredient> Ingredients { get; set; }
    }

    public class UpdateReceiptCommand
    {
        [StringLength(64)]
        [Required]
        public required string Title { get; set; }

        [StringLength(1048)]
        [Required]
        public required string Description { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public required string[] Steps { get; set; }

        public string? VideoLink { get; set; }  

        [Range(1,1000)] 
        [Required]
        public int MinCookDuration { get; set; }

        [Range(1,1000)] 
        [Required]
        public int MaxCookDuration { get; set; }

        [Required]
        public required List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        [StringLength(64)]
        public required string Name { get; set; }
        public double Quantity { get; set; }
        [StringLength(8)]
        public required string Unit { get; set; }
        public bool Optional { get; set; }
    }

    public class ReceiptDtoListResponse
    {
        public required List<ReceiptDto> receipts { get; set; }
        public bool isThereNextPage { get; set; }
    }

    public class ReceiptDtoListResponseWithCount
    {
        public required List<ReceiptDto> receipts { get; set; }
        public int count { get; set; }
    }

    public class ResponseCreateReceipt
    {
        public required string message { get; set; }
        public required ReceiptDto receipt { get; set; }
    }

    public class ResponseGetReceipt
    {
        public required ReceiptDto receipt { get; set; }
    }


}