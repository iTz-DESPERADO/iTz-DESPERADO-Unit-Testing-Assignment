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

    [Theory]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 12:00:01", 20)]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 12:01:00", 20)]
    public void Calculate_JustBeyondFirstHour_ReturnsTwenty(string entryDate, string exitDate, decimal expected)
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


    [Theory]
    [InlineData("2026-11-12 07:00:00", "2026-11-12 10:30:00", 40)]
    public void Calculate_MultipleStartedHours_ReturnsCorrectFee(string entryDate, string exitDate, decimal expected)
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


    [Fact]
    public void Calculate_ExitBeforeEntry_ThrowsArgumentException()
    {
        // Arrange
        var entry = DateTime.Parse("2026-11-13 07:00:00");

        var exit = DateTime.Parse("2026-11-13 06:59:00");

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car
        };

        var calc = new ParkingFeeCalculator();

        // Act
        var feeResult = () => calc.Calculate(session);

        // Assert
        feeResult.Should().Throw<ArgumentException>().WithMessage("Exit time must be after entry time.");
    }

    [Fact]
    public void Calculate_TenHours_ReturnsDailyCap()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 13, 1, 0, 0);
        var exit = entry.AddHours(10);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(80m);
    }


}
