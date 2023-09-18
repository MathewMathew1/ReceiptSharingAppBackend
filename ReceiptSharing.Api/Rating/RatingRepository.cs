using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _dbContext;

        public RatingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Rating?> AddRatingAsync(Rating rating)
        {
            try
            {
                _dbContext.Ratings.Add(rating);
                await _dbContext.SaveChangesAsync();
                return rating;
            }
            catch (DbUpdateException)
            {
                return null; 
            }
        }

        public async Task<List<Rating>>? GetAllUserRatingsAsync(int userId)
        {
            var ratings = await _dbContext.Ratings.Where(r => r.UserId == userId).ToListAsync();
            return ratings;
        }

        public async Task<bool> UpdateRatingAsync(Rating rating)
        {
            try
            {
                var existingRating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.ReceiptId == rating.ReceiptId && r.UserId == rating.UserId);
                if (existingRating != null)
                {
                    existingRating.Value = rating.Value; // Update the rating value if needed          
                }else{
                    _dbContext.Ratings.Add(rating);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }
    }

}