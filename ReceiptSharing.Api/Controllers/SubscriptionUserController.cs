using Microsoft.AspNetCore.Mvc;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.MiddleWare;
using AutoMapper;

namespace ReceiptSharing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionUserController : ControllerBase
    {
        private readonly ISubscriptionUserRepository _subscriptionRepository;
        private readonly ILogger<SubscriptionUserController> _logger; // Add ILogger
        private readonly IMapper _mapper;

        public SubscriptionUserController(ISubscriptionUserRepository subscriptionRepository, ILogger<SubscriptionUserController> logger, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _logger = logger; // Inject the logger
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet()]
        public async Task<ActionResult<List<SubscriptionUserDto>>> GetUserSubscriptionsAsync()
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                var subscriptions = await _subscriptionRepository.GetUserSubscriptionsAsync(user.Id);

                var subscriptionsDto = _mapper.Map<List<SubscriptionUserDto>>(subscriptions);

                return Ok( new {subscriptions = subscriptionsDto});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching subscriptions.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPost("subscribe/{userId}")]
        public async Task<IActionResult> SubscribeToUserAsync(int userId)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;

                if(userId == user.Id){
                    return BadRequest(new {error = "Cannot subscribe to yourself"});
                }
                var subscription = new SubscriptionUser
                {
                    UserId = user.Id,
                    UserSubscribedToId = userId,
                };
                
                var result = await _subscriptionRepository.SubscribeToUserAsync(subscription);

                if (result is null)
                {
                    return NotFound(new { error = "Invalid author ID." });
                }
                
                var subscriptionDto = _mapper.Map<SubscriptionUserDto>(result);
                return Ok( new {message = "Successfully subscribed", subscription = subscriptionDto});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while subscribing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPost("unsubscribe/{userId}")]
        public async Task<IActionResult> UnsubscribeFromUserAsync(int userId)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;

                if(userId == user.Id){
                    return BadRequest(new {error = "Cannot subscribe to yourself"});
                }
                var result = await _subscriptionRepository.UnsubscribeFromUserAsync(user.Id, userId);
                if (result)
                {
                    return Ok( new {message = "Successfully unSubscribed"});
                }
                
                return NotFound(new { error = "Invalid author ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while unsubscribing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }
    }
}