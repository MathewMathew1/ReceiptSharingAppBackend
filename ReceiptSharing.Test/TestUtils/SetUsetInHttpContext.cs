using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants; 

namespace ReceiptSharing.Test.TestUtils;
    
public static class UserInHttpContext{

    public static void SetUserInHttpContext(User user, HttpContext httpContext)
    {
        httpContext.Items["User"] = user;
    }
}
