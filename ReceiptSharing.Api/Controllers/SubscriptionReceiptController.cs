using Microsoft.AspNetCore.Mvc;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.MiddleWare;

namespace ReceiptSharing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionReceiptController : ControllerBase
    {
        private readonly ISubscriptionReceiptRepository _subscriptionRepository;
        private readonly ILogger<SubscriptionReceiptController> _logger; // Add ILogger

        public SubscriptionReceiptController(ISubscriptionReceiptRepository subscriptionRepository, ILogger<SubscriptionReceiptController> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _logger = logger; // Inject the logger
        }

        [Authorize]
        [HttpPost("subscribe/{receiptId}")]
        public async Task<IActionResult> SubscribeToReceiptAsync(int receiptId)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                var subscription = new SubscriptionReceipt
                {
                    UserId = user.Id,
                    ReceiptId = receiptId,
                };
                
                var result = await _subscriptionRepository.SubscribeToReceiptAsync(subscription);

                if (result)
                {
                    return Ok( new {message = "Successfully subscribed"});
                }
                
                return NotFound(new { error = "Invalid receipt ID." });
                           
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while subscribing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> SubscribeToReceiptIdsAsync()
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;

                
                var result = await _subscriptionRepository.GetSubscribedReceiptsIdsAsync(user.Id);
          
                return Ok(new { receiptIds = result, message = "success" });
                           
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while subscribing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPost("unsubscribe/{receiptId}")]
        public async Task<IActionResult> UnsubscribeFromReceiptAsync(int receiptId)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                var result = await _subscriptionRepository.UnsubscribeFromReceiptAsync(user.Id, receiptId);
                
                if (result)
                {
                    return Ok( new {message = "Successfully unsubscribed"});
                }
                
                return NotFound(new { error = "Invalid receipt ID." });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while unsubscribing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }
    }
}