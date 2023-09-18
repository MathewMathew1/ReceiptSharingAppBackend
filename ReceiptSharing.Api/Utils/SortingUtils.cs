
namespace ReceiptSharing.Api.Repositories.Utils;

public static class Sort
{
    public static int minCount = 5;
    public static double minRating = 1.0;
    public static double maxRating = 5.0;
    public static double BayesianEstimate(double average, int count)
    {
        // Bayesian estimate formula
        double estimate = (minCount * minRating + count * average) / (minCount + count);

        // Ensure the estimate is within the valid rating range
        return Math.Max(minRating, Math.Min(maxRating, estimate));
    }
}
