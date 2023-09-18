

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Helpers;
using System.Security.Claims;

namespace ReceiptSharing.Api.MiddleWare{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IUserRepository userService)
        {
            try{
                var token = context.Request.Cookies["jwt"];

                if (token is not null) await AttachUserToContext(context, token, userService);

                await next(context);
            }
            catch(Exception ex){
                Console.Write(ex);
            }
            
        }

        private async Task AttachUserToContext(HttpContext context, string token, IUserRepository userService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                string secret = configuration.GetSection("Jwt:Key").Value!;
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                   
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
           
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!;
           
                var userId = userIdClaim.Value;       
                
                context.Items["User"] = await userService.GetUserByIdAsync(int.Parse(userId));

            }
            catch (Exception e){
                Console.Write(e);
            }
        }
    }
}