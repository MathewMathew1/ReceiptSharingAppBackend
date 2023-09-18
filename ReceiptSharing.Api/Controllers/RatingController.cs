using ReceiptSharing.Api.MiddleWare;
using Microsoft.AspNetCore.Mvc;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;
using AutoMapper;

namespace ReceiptSharing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingController> _logger;
        private readonly IMapper _mapper;

        public RatingController(IRatingRepository ratingRepository, ILogger<RatingController> logger, IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult<ResponseCreateRatingMessage>> RateReceipt(int id, [FromBody] CreateRateCommand rating)
        {
            try
            {
                
                User user = (User)Request.HttpContext.Items["User"]!;
                
                var rate = new Rating
                {
                    UserId = user.Id,
                    ReceiptId = id,
                    Value = rating.Rate
                };
                
                var addedRating = await _ratingRepository.AddRatingAsync(rate);
                if (addedRating != null)
                {

                    return Ok(new ResponseCreateRatingMessage{ message = "Rating added successfully.", rating = addedRating });
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
        public async Task<ActionResult> GetRatings()
        {
            try
            {
                
                User user = (User)Request.HttpContext.Items["User"]!;
                               
                var allRatings = await _ratingRepository.GetAllUserRatingsAsync(user.Id);            
                
                var ratingDtos = _mapper.Map<List<RatingDto>>(allRatings);

                return Ok(new { message = "success", ratings = allRatings});
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when fetching ratings");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] CreateRateCommand rating)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                // You might want to add additional validation here, such as checking if the user has already rated the receipt.
                var rate = new Rating
                {
                    UserId = user.Id,
                    ReceiptId = id,
                    Value = rating.Rate
                };

                var updated = await _ratingRepository.UpdateRatingAsync(rate);

                if(!updated) return NotFound(new { error = "Invalid receipt ID." });
                
                return Ok(new { message = "Rating updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }
    }
}