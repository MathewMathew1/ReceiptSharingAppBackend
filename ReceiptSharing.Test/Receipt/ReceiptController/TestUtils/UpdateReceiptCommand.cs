using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class UpdateRandomReceiptCommand
    {
        public static UpdateReceiptCommand CreateUpdateReceipt()
        {
            var receipt = new UpdateReceiptCommand
            {
                Title = Constants.Receipt.Title,
                Description = Constants.Receipt.Description,
                Steps = new string[] { $"{Constants.Receipt.Step}1", $"{Constants.Receipt.Step}2", $"{Constants.Receipt.Step}3" },

                MinCookDuration = Constants.Receipt.MinCookDuration,
                MaxCookDuration = Constants.Receipt.MaxCookDuration,
                VideoLink = Constants.Receipt.VideoLink,
                Ingredients = new List<Ingredient> {CreateRandomIngredient.CreateIngredient(1), CreateRandomIngredient.CreateIngredient(2), CreateRandomIngredient.CreateIngredient(3)}
            };
             
            return receipt;

        }

    }
}