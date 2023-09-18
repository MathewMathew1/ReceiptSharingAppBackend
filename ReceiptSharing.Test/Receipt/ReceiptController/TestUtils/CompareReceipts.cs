
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Test.CreateObject.Utils
{
    public static class CompareReceipts
    {
        public static bool AreReceiptFieldsMatching(Receipt expected, ReceiptDto actual)
        {
            // Compare each field of the two Receipt objects
            return expected.UserId == actual.UserId &&
                expected.Title == actual.Title &&
                expected.Description == actual.Description &&
                expected.Steps.Length == actual.Steps.Length &&
                expected.ImageLinks.Length == actual.ImageLinks.Length &&
                expected.VideoLink == actual.VideoLink &&
                expected.MinCookDuration == actual.MinCookDuration &&
                expected.MaxCookDuration == actual.MaxCookDuration &&
               expected.Ingredients.Count== actual.Ingredients.Count;
        }

    }


}