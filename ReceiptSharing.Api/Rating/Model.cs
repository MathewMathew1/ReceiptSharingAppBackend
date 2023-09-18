using System.ComponentModel.DataAnnotations;

namespace ReceiptSharing.Api.Models
{
    public class Rating
    {
        public int UserId { get; set; } // User who is giving the rating
        public int ReceiptId { get; set; } // The receipt being rated

        [Range(1, 5)]
        public int Value { get; set; } // The rating value (1 to 5)

        public User? User { get; set; } // Navigation property to the user giving the rating
        public Receipt? Receipt { get; set; } // Navigation property to the receipt being rated
    }

    public class RatingDto
    {
        public int UserId { get; set; } // User who is giving the rating
        public int ReceiptId { get; set; } // The receipt being rated

        [Range(1, 5)]
        public int Value { get; set; } // The rating value (1 to 5)
    }

    public class CreateRateCommand {
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }
    }

        public class ResponseCreateRatingMessage
        {
            public required string message { get; set; }
            public required Rating rating { get; set; }
        }
}