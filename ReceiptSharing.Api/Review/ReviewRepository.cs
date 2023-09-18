using Microsoft.EntityFrameworkCore;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _dbContext;

        public ReviewRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Review?> CreateReviewAsync(Review review)
        {
            try{
                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync();
                return review;
            }catch(DbUpdateException){
                return null;
            }
        }

        public async Task<List<Review>> GetReviewsAsync(int userId)
        {
            var reviews = await _dbContext.Reviews.Where(r => r.UserId == userId).ToListAsync();
            return reviews;
        }

        public async Task<bool> DeleteReviewAsync(int receiptId, int userId)
        {
            var review = await _dbContext.Reviews.FindAsync(userId, receiptId);

            if (review == null)
            {
                return false; // Review not found
            }

            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
            return true; // Deleted successfully
        }

        public async Task<bool> UpdateReviewAsync(int userId, int receiptId, UpdateReviewCommand updatedReview)
        {
            var review = await _dbContext.Reviews.FindAsync(userId, receiptId);

            if (review == null)
            {
                return false; // Review not found
            }

            review.ReviewText = updatedReview.ReviewText;
            await _dbContext.SaveChangesAsync();
            return true; // Deleted successfully
        }

    }
}