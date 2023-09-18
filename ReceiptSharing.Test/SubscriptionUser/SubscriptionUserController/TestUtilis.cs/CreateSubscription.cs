using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.CreateObject.Utils;

namespace ReceiptSharing.Test.TestUtils
{
    public static class CreateRandomSubscription
    {
        public static SubscriptionUser CreateSubscription()
        {
            var user = CreateRandomUser.CreateUser();
            var userSubscribed = CreateRandomUser.CreateUser();
            var subscription = new SubscriptionUser
            {
                UserId = user.Id,
                User = user,
                UserSubscribedToId = userSubscribed .Id,
                SubscriptionStart = DateTime.UtcNow,
                UserSubscribedTo = userSubscribed 
            };
            return subscription;
        }
    }
}