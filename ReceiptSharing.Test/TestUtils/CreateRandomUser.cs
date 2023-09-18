using System.Security.Claims;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants; 

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CreateRandomUser
    {
        public static User CreateUser()
        {
            var user = new User
            {        
                Name = Constants.User.Name,
                Username = Constants.User.UserName,
                Email = Constants.User.Email,
                Image = Constants.User.Image,
                Id = 1
            };
            
            
            return user;
        }
    }
}






