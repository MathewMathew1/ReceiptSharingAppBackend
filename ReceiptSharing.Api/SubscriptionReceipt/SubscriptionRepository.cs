using Microsoft.EntityFrameworkCore;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public class SubscriptionReceiptRepository : ISubscriptionReceiptRepository
    {
        private readonly AppDbContext _dbContext;

        public SubscriptionReceiptRepository(AppDbContext dbContext)
        {
             _dbContext = dbContext;
        }
        

        public async Task<(List<Receipt> receipts, int count)> GetSubscribedReceiptsAsync(int userId, int limit, int skip)
        {
            var count = _dbContext.SubscriptionsReceipt.Count(sr => sr.UserId == userId);

            List<Receipt> receipts = await _dbContext.SubscriptionsReceipt
                .Where(sr => sr.UserId == userId)
                .Select(sr => sr.Receipt!)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            return (receipts, count);
        }

        public async Task<bool> SubscribeToReceiptAsync(SubscriptionReceipt subscription)
        {
            try{
                await _dbContext.SubscriptionsReceipt.AddAsync(subscription);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }

        public async Task<List<int>> GetSubscribedReceiptsIdsAsync(int userId)
        {
            List<int> receiptIds = await _dbContext.SubscriptionsReceipt
                .Where(sr => sr.UserId == userId)
                .Select(sr => sr.ReceiptId) 
                .ToListAsync();

            return receiptIds;
        }

        public async Task<bool> UnsubscribeFromReceiptAsync(int userId, int receiptId)
        {
             try{
                var subscription = await _dbContext.SubscriptionsReceipt
                    .SingleOrDefaultAsync(s => s.UserId == userId && s.ReceiptId == receiptId);

                if (subscription != null)
                {
                    _dbContext.SubscriptionsReceipt.Remove(subscription);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }
    }
}