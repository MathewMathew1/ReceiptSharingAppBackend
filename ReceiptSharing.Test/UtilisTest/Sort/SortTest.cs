using ReceiptSharing.Api.Repositories.Utils;

public class SortTests
{
    public class Item
    {
        public double Average { get; set; }
        public int Count { get; set; }
    }

    [Fact]
    public void BayesianEstimate_ReturnsExpectedSorting()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Average = 2.0, Count = 3000 },
            new Item { Average = 4.0, Count = 4 },
            new Item { Average = 3.5, Count = 15 },
            // Add more items with different average and count values
        };

        // Calculate Bayesian estimates and sort the items
        var sortedItems = items.OrderByDescending(item => Sort.BayesianEstimate(item.Average, item.Count)).ToList();

        // Assert
        Assert.Equal(3.5, sortedItems[0].Average, precision: 10); // Highest Bayesian estimate
        Assert.Equal(4, sortedItems[1].Average, precision: 10); // Second highest Bayesian estimate
        Assert.Equal(2.0, sortedItems[2].Average, precision: 10); // Lowest Bayesian estimate
    }
}