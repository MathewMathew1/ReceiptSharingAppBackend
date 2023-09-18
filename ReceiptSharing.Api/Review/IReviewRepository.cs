using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface IReviewRepository
    {
        Task<Review?> CreateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int receiptId, int userId);
        Task<bool> UpdateReviewAsync(int userId, int receiptId, UpdateReviewCommand updatedReview);
        Task<List<Review>> GetReviewsAsync(int userId);
    }
}