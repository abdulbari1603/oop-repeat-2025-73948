using Xunit;
using CarServMgmt.Domain;

namespace CarServMgmt.Tests;

public class ServiceRecordTests
{
    [Fact]
    public void GetTotalCost_RoundsUpHoursAndCalculatesCorrectly()
    {
        // Arrange
        var record = new ServiceRecord { Hours = 2.5 };
        // Act
        var cost = record.GetTotalCost();
        // Assert
        Assert.Equal(225m, cost); // 2.5 rounds up to 3, 3 * 75 = 225
    }
} 