using FluentAssertions;
using ParkSmart.Application;
using ParkSmart.Domain;
using Xunit;

namespace ParkSmart.UnitTests;

public class ParkingFeeCalculatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    public void Calculate_FreePeriod_ReturnsZero(int minutes)
    {
        // Arrange
        var entry = new DateTime(2026, 11, 9, 10, 0, 0);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = entry.AddMinutes(minutes),
            Vehicle = VehicleType.Car
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(0m);
    }
}
