using System.Security.Claims;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CreateRandomReceipt
    {
        public static Receipt CreateReceipt()
        {
            var receipt = new Receipt
            {
                UserId = Constants.Receipt.UserId,
                Title = Constants.Receipt.Title,
                Description = Constants.Receipt.Description,
                Steps = new string[] { $"{Constants.Receipt.Step}1", $"{Constants.Receipt.Step}2", $"{Constants.Receipt.Step}3" },
                ImageLinks = new string[] { $"{Constants.Receipt.ImageLink}1", $"{Constants.Receipt.ImageLink}2", $"{Constants.Receipt.ImageLink}3" },
                MinCookDuration = Constants.Receipt.MinCookDuration,
                MaxCookDuration = Constants.Receipt.MaxCookDuration,
                Ingredients = new Ingredient[] {CreateRandomIngredient.CreateIngredient(1), CreateRandomIngredient.CreateIngredient(2), CreateRandomIngredient.CreateIngredient(3)}
            };
            return receipt;

        }
    }
}