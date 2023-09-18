using AutoMapper;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Mappings;

public class UserRatingOnReviewResolver : IValueResolver<Receipt, ReceiptDto, List<ReviewInReceipt>>
{
    private readonly IMapper _mapper;

    public UserRatingOnReviewResolver(IMapper mapper)
    {
        _mapper = mapper;
    }

    public List<ReviewInReceipt> Resolve(Receipt source, ReceiptDto destination, List<ReviewInReceipt> member, ResolutionContext context)
    {
        

        if (source.Reviews == null || source.Reviews.Count == 0 || source.Ratings == null || source.Ratings.Count == 0)
        {
            return null; // If there are no reviews or ratings, return null.
        }

        var reviewsWithRating = new List<ReviewInReceipt>();

        foreach (var review in source.Reviews)
        {
            var userRating = source.Ratings.FirstOrDefault(r => r.UserId == review.UserId);

            reviewsWithRating.Add(new ReviewInReceipt
            {
                // Map other review properties
                UserId = review.UserId,
                ReceiptId = review.ReceiptId,
                ReviewText = review.ReviewText,
                User = _mapper.Map<OtherUserDto>(review.User),
                RatingValue = userRating != null ? userRating.Value : (int?)null,
                CreatedAt = review.CreatedAt,
            });
            
        }

        return reviewsWithRating;
    }
}