using System.Security.Claims;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CreateRandomIngredient
    {
        public static Ingredient CreateIngredient(int id)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Constants.Claim.Name),
                new Claim(ClaimTypes.Email, Constants.Claim.Email),
                new Claim(ClaimTypes.NameIdentifier, Constants.Claim.NameIdentifier),
                new Claim("image", Constants.Claim.AvatarHash),
                new Claim("urn:discord:avatar:hash", Constants.Claim.Image)
            };
            var ingredient = new Ingredient
            {
                Name = $"{Constants.Ingredient.Name} ${id}",
                Quantity = Constants.Ingredient.Quantity,
                Unit = Constants.Ingredient.Unit,
                Optional = Constants.Ingredient.Optional,
            };

            
            return ingredient;
        }
    }
}