using AutoMapper;
using ReceiptSharing.Api.MiddleWare;
using Microsoft.AspNetCore.Mvc;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;

namespace ReceiptSharing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewController> _logger;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, ILogger<ReviewController> logger, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult<ResponseCreateReviewMessage>> CreateReviewAsync(int id, [FromBody] CreateReviewCommand createReview)
        {
            try
            {
                
                User user = (User)Request.HttpContext.Items["User"]!;
                
                var review = new Review
                {
                    UserId = user.Id,
                    ReceiptId = id,
                    ReviewText = createReview.ReviewText
                };
                
                var addedReview = await _reviewRepository.CreateReviewAsync(review);
                if (addedReview != null)
                {
                    var reviewDto = _mapper.Map<ReviewDto>(addedReview);
                    var response = new ResponseCreateReviewMessage { message = "Rating added successfully.", review = reviewDto };
                    return Ok(response);
                }
                
                return NotFound(new { error = "Invalid receipt ID." });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpGet()]
        public async Task<ActionResult> GetUserReviewsAsync()
        {
            try
            {
                
                User user = (User)Request.HttpContext.Items["User"]!;
                
                var reviews = await _reviewRepository.GetReviewsAsync(user.Id);
                var reviewsDtos = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(new { message = "success", review = reviews });          
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewAsync(int id, [FromBody] UpdateReviewCommand updateReview)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                // You might want to add additional validation here, such as checking if the user has already rated the receipt.

                var updated = await _reviewRepository.UpdateReviewAsync(user.Id, id, updateReview);

                if(!updated) return NotFound(new { error = "Invalid receipt ID." });
                
                return Ok(new { message = "Review updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                // You might want to add additional validation here, such as checking if the user has already rated the receipt.

                var deleted = await _reviewRepository.DeleteReviewAsync(id, user.Id);

                if(!deleted) return NotFound(new { error = "Invalid receipt ID." });
                
                return Ok(new { message = "Review deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }
    }
}