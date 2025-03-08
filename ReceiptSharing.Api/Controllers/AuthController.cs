using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Models;
using System.Security.Claims;
using AutoMapper;
using ReceiptSharing.Api.MiddleWare;

namespace ReceiptSharing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;

        public AuthController(IUserRepository userRepository, ILogger<AuthController> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            try
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = $"https://{Request.Host}/api/Auth/google-response"
                };
                _logger.LogInformation("RedirectUri: " + properties.RedirectUri);

                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("discord")]
        public IActionResult DiscordLogin()
        {
            try
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = $"https://{Request.Host}/api/Auth/discord-response"
                };

                return Challenge(properties, "Discord");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating discord link");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [RequireHttps]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsIdentity = result.Principal?.Identity as ClaimsIdentity;

                if (claimsIdentity == null || result == null) return BadRequest("Authentication failed or claims are missing.");

                var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                var name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                var profileImageUrl = claimsIdentity.FindFirst("image")?.Value;


                if (email == null || name == null || profileImageUrl == null) return BadRequest("Authentication failed or claims are missing.");

                return await HandleAuthentication(email, name, profileImageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [RequireHttps]
        [HttpGet("discord-response")]
        public async Task<IActionResult> DiscordResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (result.Principal == null)
                {
                    return BadRequest("Authentication failed or claims are missing.");
                }

                var claimsIdentity = result.Principal.Identity as ClaimsIdentity;

                if (claimsIdentity == null)
                {
                    return BadRequest("Authentication failed or claims are missing.");
                }

                var name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

                var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                var nameIdentifier = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var avatarHash = claimsIdentity.FindFirst("urn:discord:avatar:hash")?.Value; // Discord Avatar Hash
                var profileImageUrl = $"https://cdn.discordapp.com/avatars/{nameIdentifier}/{avatarHash}.png";

                if (email == null || name == null) return BadRequest("Authentication failed or claims are missing.");

                return await HandleAuthentication(email, name, profileImageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        private async Task<IActionResult> HandleAuthentication(string email, string name, string profileImageUrl)
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(1), // Set cookie expiration
                    HttpOnly = true, // Cookie accessible only through HTTP
                    Secure = true, // Require HTTPS to send the cookie
                    SameSite = SameSiteMode.None, // Allow cross-site requests
                };
                string token;
                var existingUserWithThisEmail = await _userRepository.GetUserByEmailAsync(email);

                var user = existingUserWithThisEmail;
                if (existingUserWithThisEmail == null)
                {
                    var newUser = new User
                    {
                        Email = email,
                        Name = name,
                        Image = profileImageUrl,
                    };
                    user = newUser;
                    var userCreated = await _userRepository.CreateUserAsync(newUser);
                }


                token = _userRepository.GenerateJwtToken(user!);

                Response.Cookies.Append("jwt", token, cookieOptions);

                string redirectUrl;

#if DEBUG
                redirectUrl = "http://localhost:5173/";
#else
                redirectUrl = "https://receptao.netlify.app/";
#endif

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("user/profile/{id}")]
        public async Task<IActionResult> GetOtherUserProfileData(int id)
        {
            try
            {
                var user = await _userRepository.GetOtherUserById(id);
                var userDto = _mapper.Map<UserProfileDto>(user);
                return Ok(new { user = userDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserDataAsync()
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(new { user = userDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user data");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPatch("username")]
        public async Task<IActionResult> ChangeUsernameAsync(ChangeUsernameCommand usernameCommand)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                await _userRepository.ChangeUsername(usernameCommand.Username, user.Id);
                return Ok(new { message = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user data");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Append("jwt", "", new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddYears(-1), // Expire it in the past
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Path = "/",
                });
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }



        // ...
    }
}