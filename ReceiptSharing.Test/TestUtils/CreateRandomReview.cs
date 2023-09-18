using System.Security.Claims;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CreateRandomReview
    {
        public static Review CreateReview()
        {
            var review = new Review
            {
                ReviewText = Constants.Review.ReviewText,
                ReceiptId = Constants.Review.ReceiptId,
            };
            
            return review;

        }
    }
}