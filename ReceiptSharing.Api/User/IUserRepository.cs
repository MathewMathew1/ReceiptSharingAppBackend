using System.Threading.Tasks;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        string GenerateJwtToken(User user);
        Task<User?> GetOtherUserById(int id);
        Task ChangeUsername(string username, int userId);
    }
}
