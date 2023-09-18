using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CreateRandomReviewCommand
    {
        public static CreateReviewCommand CreateReview()
        {
            var review = new CreateReviewCommand
            {
                ReviewText = Constants.Review.ReviewText,
            };
            
            return review;

        }

        public static UpdateReviewCommand UpdateReview()
        {
            var review = new UpdateReviewCommand
            {
                ReviewText = Constants.Review.ReviewText,
            };
            
            return review;

        }
    }
}