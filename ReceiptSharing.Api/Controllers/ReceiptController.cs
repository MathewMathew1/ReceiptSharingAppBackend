using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReceiptSharing.Api.MiddleWare;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;

namespace ReceiptSharing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly ISubscriptionReceiptRepository _subscriptionRepository;
        private readonly ISubscriptionUserRepository _subscriptionUserRepository;
        private readonly ILogger<ReceiptController> _logger; // Add ILogger
        private readonly IMapper _mapper;
        private readonly IImageStorage _imageStorage;

        public ReceiptController(IReceiptRepository receiptRepository, ILogger<ReceiptController> logger, IImageStorage imageStorage,
        IMapper mapper, ISubscriptionReceiptRepository subscriptionRepository, ISubscriptionUserRepository subscriptionUserRepository)
        {
            _receiptRepository = receiptRepository;
            _logger = logger; 
            _mapper = mapper;
            _imageStorage = imageStorage;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionUserRepository = subscriptionUserRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseCreateReceipt>> CreateReceipt([FromForm] CreateReceiptCommand createReceiptInput)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;

                var images = createReceiptInput.Images.ToArray(); // Convert to an array for indexing

                var tasks = images.Select(async (image, index) =>
                {
                    var link = await _imageStorage.PostImage(image);
                    return (image, link);
                });

                var results = await Task.WhenAll(tasks);

                List<string> imagesLink = new List<string>();

                foreach (var (image, link) in results)
                {
                    if (link is not null)
                    {
                        imagesLink.Add(link);
                    }
                }

                var receipt = new Receipt
                {
                    UserId = user.Id,
                    Title = createReceiptInput.Title,
                    Description = createReceiptInput.Description,
                    Steps = createReceiptInput.Steps.ToArray(),
                    ImageLinks = imagesLink.ToArray(),
                    VideoLink = createReceiptInput.VideoLink,
                    MinCookDuration = createReceiptInput.MinCookDuration,
                    MaxCookDuration = createReceiptInput.MaxCookDuration,
                    Ingredients = createReceiptInput.Ingredients
                };

                var receiptCreated = await _receiptRepository.CreateReceiptAsync(receipt);
               
                var receiptDto = _mapper.Map<ReceiptDto>(receiptCreated);
                return Ok(new ResponseCreateReceipt { message = "Receipt created successfully.", receipt = receiptDto });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An error occurred while creating receipt");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseGetReceipt>> GetReceiptById(int id)
        {
            try
            {
                var receipt = await _receiptRepository.GetReceiptByIdAsync(id);

                if (receipt == null)
                {
                    return NotFound(new { error = "Receipt not found." });
                }

                var receiptDto = _mapper.Map<ReceiptDto>(receipt);

                return Ok( new ResponseGetReceipt{receipt = receiptDto});
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An error occurred while fetching receipt by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceipt(int id)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;

                var receipt = await _receiptRepository.GetReceiptByIdAsync(id);

                if (receipt == null)
                {
                    return NotFound(new { error = "Receipt not found." });
                }

       
                if (receipt.UserId != user.Id)
                {
                    return Forbid("Only the author can update this receipt.");
                }

                var deleted = await _receiptRepository.DeleteReceiptAsync(id);

                if(deleted){
                    return Ok(new { message = "Receipt deleted successfully." });
                }

                return NotFound(new { error = "Receipt not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceipt(int id, [FromForm] UpdateReceiptCommand updatedReceipt)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                var receipt = await _receiptRepository.GetReceiptByIdAsync(id);

                if (receipt == null)
                {
                    return NotFound(new { error = "Receipt not found." });
                }

       
                if (receipt.UserId != user.Id)
                {
                    return Forbid("Only the author can update this receipt.");
                }

                await _receiptRepository.UpdateReceiptAsync(receipt, updatedReceipt);

                return Ok(new { message = "Receipt updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }


        [Authorize]
        [HttpGet("subscribed")]
        public async Task<ActionResult<ReceiptDtoListResponseWithCount>> GetSubscribedReceipts(int limit = 10, int skip = 0)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                int userId = user.Id;

                var result = await _subscriptionRepository.GetSubscribedReceiptsAsync(userId, limit, skip);
                var receiptDtos = _mapper.Map<List<ReceiptDto>>(result.receipts);

                var responseDto = new ReceiptDtoListResponseWithCount
                {
                    receipts = receiptDtos,
                    count = result.count
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }


        [HttpGet("newest/subscribed")]
        public async Task<ActionResult<ReceiptDtoListResponseWithCount>> GetNewestUserSubscribedReceipts(int limit = 10, int skip = 0)
        {
            try
            {
                User user = (User)Request.HttpContext.Items["User"]!;
                int userId = user.Id;

                var userSubscribedTo = await _subscriptionUserRepository.GetUserSubscriptionsAsync(userId);
                var userSubscribedToIds = userSubscribedTo.Select(x =>x.UserSubscribedToId).ToList();

                var (subscribedReceipts, totalCount) = await _receiptRepository.GetNewestSubscribedReceiptsAsync(userSubscribedToIds, limit, skip);
                var receiptDtos = _mapper.Map<List<ReceiptDto>>(subscribedReceipts);

                return Ok(new ReceiptDtoListResponseWithCount { receipts = receiptDtos, count = totalCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting receipt.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("")]
        public async Task<ActionResult<ReceiptDtoListResponse>> GetSortedReceipts(string sortType = "newest", int limit = 10, int skip = 0, string searchQuery = "")
        {
            try
            {
                var sortedReceipts = new List<Receipt>(); 
                var isThereNextPage = false;

                if (sortType == "bestRated")
                {
                    (sortedReceipts, isThereNextPage) = await _receiptRepository.GetReceiptsWithBayesianRatingAsync(limit, skip, searchQuery);
                }
                else if(sortType == "hot")
                {
                    (sortedReceipts, isThereNextPage) = await _receiptRepository.GetReceiptsSortedByNewSubscriptionsAsync(limit, skip, searchQuery);
                }else
                {
                    (sortedReceipts, isThereNextPage) = await _receiptRepository.GetNewestReceiptsAsync(limit, skip, searchQuery);
                }


                var receiptDtos = _mapper.Map<List<ReceiptDto>>(sortedReceipts);

                var responseDto = new ReceiptDtoListResponse
                {
                    receipts = receiptDtos,
                    isThereNextPage = isThereNextPage
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching sorted receipts.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }

        [HttpGet("user/receipts/{id}")]
        public async Task<ActionResult<ReceiptDtoListResponseWithCount>> GetUserReceipts(int id, int limit = 10, int skip = 0)
        {
            try
            {
                var listOfIds = new List<int> {id};
                var (receipts, count) = await _receiptRepository.GetNewestSubscribedReceiptsAsync(listOfIds, limit, skip);

                var receiptDtos = _mapper.Map<List<ReceiptDto>>(receipts);

                var responseDto = new ReceiptDtoListResponseWithCount
                {
                    receipts = receiptDtos,
                    count = count
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user receipts.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"An unexpected error occurred" });
            }
        }


        
    }
}