using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories
{
    public class SubscriptionUserRepository : ISubscriptionUserRepository
    {
        private readonly AppDbContext _dbContext;

        public SubscriptionUserRepository(AppDbContext dbContext)
        {
             _dbContext = dbContext;
        }
        

        public async Task<IEnumerable<SubscriptionUser>> GetUserSubscriptionsAsync(int userId)
        {
            return await _dbContext.SubscriptionsUser
                .Where(s => s.UserId== userId)
                .Include(s => s.UserSubscribedTo)
                .ToListAsync();
        }

        public async Task<SubscriptionUser?> SubscribeToUserAsync(SubscriptionUser subscription)
        {
            try
            {
                await _dbContext.SubscriptionsUser.AddAsync(subscription);
                _dbContext.SaveChanges();
                return subscription ;
            }catch(DbUpdateException){
                return null;
            }
            
        }

        public async Task<bool> UnsubscribeFromUserAsync(int userId, int userSubscribedTo)
        {
            try
            {
                var subscription = await _dbContext.SubscriptionsUser
                    .SingleOrDefaultAsync(s => s.UserId == userId && s.UserSubscribedToId == userSubscribedTo);

                if (subscription != null)
                {
                     _dbContext.SubscriptionsUser.Remove(subscription);
                    _dbContext.SaveChanges();
                }
                return true;
            }catch(DbUpdateException)
            {
                return false;
            }
        }
    }

}