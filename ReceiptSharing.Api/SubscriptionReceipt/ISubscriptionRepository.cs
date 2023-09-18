using System.Collections.Generic;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface ISubscriptionReceiptRepository
    {
        Task<(List<Receipt> receipts, int count)> GetSubscribedReceiptsAsync(int userId, int limit, int skip);
        Task<bool> SubscribeToReceiptAsync(SubscriptionReceipt subscription);
        Task<bool> UnsubscribeFromReceiptAsync(int userId, int receiptId);
        Task<List<int>> GetSubscribedReceiptsIdsAsync(int userId);
    }
}