using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public interface IReceiptRepository
    {
        Task<Receipt> CreateReceiptAsync(Receipt receipt);
        Task<bool> DeleteReceiptAsync(int receiptId);
        Task<Receipt?> GetReceiptByIdAsync(int receiptId);
        Task UpdateReceiptAsync(Receipt receipt, UpdateReceiptCommand newReceipt);
        Task<(List<Receipt> receipts, bool isThereNextPage)> GetNewestReceiptsAsync(int limit, int skip);
        Task<(List<Receipt> receipts, int totalCount)> GetNewestSubscribedReceiptsAsync(List<int> subscribedToUserIds, int limit, int skip);
        Task<(List<Receipt> receipts, bool isThereNextPage)> GetReceiptsWithBayesianRatingAsync(int limit, int skip);
        Task<(List<Receipt> receipts, bool isThereNextPage)> GetReceiptsSortedByNewSubscriptionsAsync(int limit, int skip);
    }
}