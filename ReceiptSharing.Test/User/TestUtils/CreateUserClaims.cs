using System.Security.Claims;
using ReceiptSharing.Test.TestUtils.Constants;  // Import the outer namespace

namespace ReceiptSharing.Test.CreateUser.TestUtils
{
    public static class CreateUserClaims
    {
        public static Claim[] CreateClaims()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Constants.Claim.Name),
                new Claim(ClaimTypes.Email, Constants.Claim.Email),
                new Claim(ClaimTypes.NameIdentifier, Constants.Claim.NameIdentifier),
                new Claim("image", Constants.Claim.AvatarHash),
                new Claim("urn:discord:avatar:hash", Constants.Claim.Image)
            };

            return claims;
        }
    }
}