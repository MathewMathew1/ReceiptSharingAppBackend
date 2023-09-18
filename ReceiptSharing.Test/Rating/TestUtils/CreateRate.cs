using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Test.TestUtils
{
    public static class CreateRating
    {
        public static  CreateRateCommand CreateRate(int ratingValue)
        {
            var rating = new CreateRateCommand { Rate = ratingValue };
            return rating;
        }
    }
}