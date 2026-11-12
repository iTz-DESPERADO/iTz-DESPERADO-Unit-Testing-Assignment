using ParkSmart.Application;
using ParkSmart.Domain;
using System.Globalization;

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


    [Fact]
    public void Calculate_JustAfterFifteenMinutes_IsNotFree()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 12, 10, 0, 0);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = entry.AddMinutes(15).AddSeconds(1),
            Vehicle = VehicleType.Car
        };

        // Act
        var fee = new ParkingFeeCalculator().Calculate(session);

        // Assert
        fee.Should().BeGreaterThan(0m);
    }


    [Theory]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 11:16:00", 10)]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 12:00:00", 10)]
    public void Calculate_FirstStartedHour_ReturnsTen(string entryDate, string exitDate, decimal expected)
    {
        // Arrange
        var entry = DateTime.ParseExact(entryDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var exit = DateTime.ParseExact(exitDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(expected);
    }

}
