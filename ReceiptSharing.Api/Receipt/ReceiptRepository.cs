using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories.Utils;

namespace ReceiptSharing.Api.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _dbContext;

        public ReceiptRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;  
        }

        public async Task<Receipt?> GetReceiptByIdAsync(int receiptId)
        {
            var receipt = await _dbContext.Receipts
                .Where(r => r.Id == receiptId)
                .Include(r => r.Ratings)
                .Include(r => r.Reviews)
                .Include(r => r.User)
                .Include(r => r.SubscriptionsReceipt)
                .FirstOrDefaultAsync();

            return receipt;
        }

        public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
        {
            _dbContext.Receipts.Add(receipt);
            await _dbContext.SaveChangesAsync();
            return receipt;
        }

        public async Task<bool> DeleteReceiptAsync(int receiptId)
        {
            var receipt = await _dbContext.Receipts.FindAsync(receiptId);

            if (receipt == null)
            {
                return false; // Receipt not found
            }

            _dbContext.Receipts.Remove(receipt);
            await _dbContext.SaveChangesAsync();
            return true; // Deleted successfully
        }

        public async Task UpdateReceiptAsync(Receipt receipt, UpdateReceiptCommand newReceipt)
        {
            _dbContext.Entry(receipt).CurrentValues.SetValues(newReceipt);

            await _dbContext.SaveChangesAsync();

        }

        public async Task<(List<Receipt> receipts, bool isThereNextPage)> GetNewestReceiptsAsync(int limit, int skip)
        {
            var receipts = await Task.FromResult(_dbContext.Receipts.OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(limit + 1)
                .Include(r => r.Ratings)
                .Include(r => r.User)
                .Include(r => r.Reviews)
                .Include(r => r.SubscriptionsReceipt)
                .ToList());

            var isThereNextPage = receipts.Count > limit;
            if (isThereNextPage)
            {
                receipts.RemoveAt(receipts.Count - 1);
            }

            return (receipts, isThereNextPage);
        }

        public async Task<(List<Receipt> receipts, int totalCount)> GetNewestSubscribedReceiptsAsync(List<int> subscribedToUserIds, int limit, int skip)
        {      
            var count = _dbContext.SubscriptionsReceipt.Count(r => subscribedToUserIds.Contains(r.UserId));

            var subscribedReceiptsQuery = await _dbContext.Receipts
                .Where(r => subscribedToUserIds.Contains(r.UserId))
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(limit + 1)
                .Include(r => r.Ratings)
                .Include(r => r.User)
                .Include(r => r.SubscriptionsReceipt)
                .Include(r => r.Reviews)
                .ToListAsync();

            return (subscribedReceiptsQuery, count);
        }

        public async Task<(List<Receipt> receipts, bool isThereNextPage)> GetReceiptsWithBayesianRatingAsync(int limit, int skip)
        {
            var rankedReceipts = await _dbContext.Receipts
                .Include(r => r.Ratings)
                .Select(r => new
                {
                    Receipt = r,
                    AvgRating = r.Ratings.Count > 0
                        ? r.Ratings.AsEnumerable().Average(rt => rt.Value)
                        : 0
                })
                .OrderByDescending(r => 
                    (Sort.minCount * Sort.minRating + r.Receipt.Ratings!.Count * r.AvgRating) / 
                    (Sort.minCount + r.Receipt.Ratings!.Count))
                .Skip(skip)
                .Take(limit + 1)
                .Select(r => r.Receipt)
                .Include(r => r.User)
                .Include(r => r.SubscriptionsReceipt)
                .Include(r => r.Reviews)
                .AsSplitQuery()
                .ToListAsync();    

            var isThereNextPage = rankedReceipts.Count > limit;
            if (isThereNextPage)
            {
                rankedReceipts.RemoveAt(rankedReceipts.Count - 1);
            }

            return (rankedReceipts, isThereNextPage);
        }

        public async Task<(List<Receipt> receipts, bool isThereNextPage)> GetReceiptsSortedByNewSubscriptionsAsync(int limit, int skip)
        {
            var lastWeekendStart = DateTimeOffset.UtcNow.AddDays(-6);
            var lastWeekendEnd = lastWeekendStart.AddDays(7);

            var rankedReceipts = await _dbContext.Receipts 
                .Include(r => r.SubscriptionsReceipt)
                .Select(r => new
                {
                    Receipt = r,
                    NewSubscriptionsCount = _dbContext.SubscriptionsReceipt
                        .Count(sr => sr.ReceiptId == r.Id && 
                                    sr.SubscriptionStart >= lastWeekendStart && 
                                    sr.SubscriptionStart < lastWeekendEnd)
                })
                .OrderByDescending(r => r.NewSubscriptionsCount)
                .Skip(skip)
                .Take(limit + 1)
                .Select(r => r.Receipt)
                .Include(r => r.Ratings)
                .Include(r => r.User)
                
                .Include(r => r.Reviews)
                .ToListAsync();

            var isThereNextPage = rankedReceipts.Count() > limit;
            if (isThereNextPage)
            {
                rankedReceipts.RemoveAt(rankedReceipts.Count() - 1);
            }

            return (rankedReceipts, isThereNextPage);
        }


    }
}