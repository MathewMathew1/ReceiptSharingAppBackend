using System.Threading.Tasks;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface IRatingRepository
    {
        Task<Rating?> AddRatingAsync(Rating rating);
        Task<bool> UpdateRatingAsync(Rating rating);
        Task<List<Rating>>? GetAllUserRatingsAsync(int userId);
    }
}