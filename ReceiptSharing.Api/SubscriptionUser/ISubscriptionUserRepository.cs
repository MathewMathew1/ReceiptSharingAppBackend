using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface ISubscriptionUserRepository
    {
        Task<IEnumerable<SubscriptionUser>> GetUserSubscriptionsAsync(int userId);
        Task<SubscriptionUser?> SubscribeToUserAsync(SubscriptionUser subscription);
        Task<bool> UnsubscribeFromUserAsync(int userId, int userSubscribedTo);
    }
}