using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Microsoft.AspNetCore.Authentication;

namespace ReceiptSharing.Test.CreateUser.TestUtils
{
    public static class AuthenticationHttpContext
    {
        public static DefaultHttpContext CreateHttpContextWithClaims(Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            var authService = Substitute.For<IAuthenticationService>();
            authService.AuthenticateAsync(Arg.Any<HttpContext>(), Arg.Any<string>())
                .Returns(Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "some-scheme"))));

            var httpContext = new DefaultHttpContext
            {
                RequestServices = Substitute.For<IServiceProvider>()
            };
            httpContext.RequestServices.GetService(typeof(IAuthenticationService))
                .Returns(authService);
            
            return httpContext;
        }

        public static DefaultHttpContext CreateHttpContextWithoutClaims(){
            var authService = Substitute.For<IAuthenticationService>();
            authService.AuthenticateAsync(Arg.Any<HttpContext>(), Arg.Any<string>())
                .Returns(Task.FromResult(AuthenticateResult.NoResult()));

            var httpContext = new DefaultHttpContext
            {
                RequestServices = Substitute.For<IServiceProvider>()
            };
            httpContext.RequestServices.GetService(typeof(IAuthenticationService))
                .Returns(authService);
            
            return httpContext;
        }
    }
}