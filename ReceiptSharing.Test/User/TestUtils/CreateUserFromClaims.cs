using System.Collections.Generic;
using System.Security.Claims;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Test.CreateUser.TestUtils
{
    public static class UserUtils
    {
        public static User CreateUserFromClaims(IEnumerable<Claim> claims)
        {
            var user = new User();
            user.CreatedAt = DateTime.UtcNow;
            user.Username = null;

            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Name:
                        user.Name = claim.Value;
                        break;
                    case ClaimTypes.Email:
                        user.Email = claim.Value;
                        break;
                    case "urn:discord:avatar:hash":
                        user.Image = claim.Value;
                        break;
                    // Add more cases for other claims if needed
                }
            }

            return user;
        }
    }
}