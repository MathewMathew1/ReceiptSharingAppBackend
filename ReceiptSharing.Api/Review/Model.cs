using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceiptSharing.Api.Models
{
    public class Review
    {
        public int UserId { get; set; } // User who is giving the rating
        public int ReceiptId { get; set; } // The receipt being rated
        public required string ReviewText { get; set;}
        public User? User { get; set; } // Navigation property to the user giving the rating
        public Receipt? Receipt { get; set; } 
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
    

    public class ReviewDto
    {
        public int UserId { get; set; } // User who is giving the rating
        public int ReceiptId { get; set; } // The receipt being rated
        public required string ReviewText { get; set;}

        public User? User { get; set; } // Navigation property to the user giving the rating
        public Receipt? Receipt { get; set; } 
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public class ReviewInReceipt
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public int UserId { get; set; } // User who is giving the rating
        public int ReceiptId { get; set; } // The receipt being rated
        public required string ReviewText { get; set;}
        public int? RatingValue { get; set; }
        public OtherUserDto? User { get; set; } // Navigation property to the user giving the rating
    }

    public class CreateReviewCommand
    {
        [MinLength(3)]
        [StringLength(1048)]
        [Required]
        public required string ReviewText {get; set;}
    } 

    public class UpdateReviewCommand
    {
        [MinLength(3)]
        [StringLength(1048)]
        [Required]
        public required string ReviewText {get; set;}
    } 

    public class ResponseCreateReviewMessage
    {
        public required string message { get; set; }
        public required ReviewDto review { get; set; }
    }

}