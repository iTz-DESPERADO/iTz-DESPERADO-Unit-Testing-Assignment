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


    [Theory]
    [InlineData("2026-11-13 00:00:00", "2026-11-14 06:00:00", 140)]
    [InlineData("2026-11-13 00:00:00", "2026-11-15 00:00:00", 160)]
    [InlineData("2026-11-13 00:00:00", "2026-11-15 00:10:00", 170)]
    public void Calculate_MultipleDays_ReturnsCorrectFee(string entryDate, string exitDate, decimal expected)
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
    [InlineData("2026-11-12 11:00:00", "2026-11-12 11:15:01", 5)]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 11:16:00", 5)]
    public void Calculate_FirstStartedHourForMotoCycle_ReturnsFive(string entryDate, string exitDate, decimal expected)
    {
        // Arrange
        var entry = DateTime.ParseExact(entryDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var exit = DateTime.ParseExact(exitDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Motorcycle
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(expected);
    }


    [Fact]
    public void Calculate_TenHoursForMotorCycle_ReturnsDailyCap()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 13, 3, 0, 0);
        var exit = entry.AddHours(10);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Motorcycle
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(40m);
    }

    [Theory]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 11:15:01", 25)]
    [InlineData("2026-11-12 11:00:00", "2026-11-12 11:16:00", 25)]
    public void Calculate_FirstStartedHourForTruck_ReturnsTwintyFive(string entryDate, string exitDate, decimal expected)
    {
        // Arrange
        var entry = DateTime.ParseExact(entryDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var exit = DateTime.ParseExact(exitDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Truck
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(expected);
    }


    [Fact]
    public void Calculate_TenHoursForTruck_ReturnsDailyCap()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 13, 3, 0, 0);
        var exit = entry.AddHours(10);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Truck
        };

        // Act
        var feeResult = new ParkingFeeCalculator().Calculate(session);

        // Assert
        feeResult.Should().Be(200m);
    }



    [Theory]
    [InlineData(VehicleType.Car, 30)]
    [InlineData(VehicleType.Motorcycle, 15)]
    [InlineData(VehicleType.Truck, 75)]
    public void Calculate_EligibleNight_ReturnsNightFlatRate(VehicleType vehicle,decimal expected)
    {
        // Arrange
        var entry = new DateTime(2026, 11, 16, 22, 0, 0);
        var exit = new DateTime(2026, 11, 17, 8, 0, 0);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = vehicle
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(VehicleType.Car, 150)]
    [InlineData(VehicleType.Motorcycle, 75)]
    [InlineData(VehicleType.Truck, 300)]
    public void Calculate_LostTicket_ReturnsCorrectFees(
        VehicleType vehicle,
        decimal expected)
    {
        // Arrange
        var entry = new DateTime(2026, 11, 16, 10, 0, 0);
        var exit = entry.AddHours(2);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = vehicle,
            TicketLost = true
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(VehicleType.Car, 48, 160)]
    [InlineData(VehicleType.Motorcycle, 48, 80)]
    [InlineData(VehicleType.Truck, 30, 350)]
    public void Calculate_LostTicket_WhenNormalFeeIsHigher_ReturnsHigherFee(VehicleType vehicle,int hours,decimal expected)
    {
        // Arrange
        var entry = new DateTime(2026, 11, 16, 10, 0, 0);
        var exit = entry.AddHours(hours);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = vehicle,
            TicketLost = true
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Calculate_FridayParkingUpToTwoHours_ReturnsZero()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 13, 10, 0, 0);
        var exit = entry.AddHours(2);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void Calculate_FridayParkingJustAfterTwoHours_ReturnsThirty()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 13, 10, 0, 0);
        var exit = entry.AddHours(2).AddMinutes(1);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(30m);
    }

    [Fact]
    public void Calculate_SubscriberWithinWeekdayFreeWindow_ReturnsZero()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 16, 9, 0, 0);
        var exit = new DateTime(2026, 11, 16, 17, 0, 0);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car,
            IsSubscriber = true
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void Calculate_SubscriberOutsideFreeWindow_ReturnsHalfFee()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 16, 18, 0, 0);
        var exit = new DateTime(2026, 11, 16, 20, 0, 0);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car,
            IsSubscriber = true
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(10m);
    }

    [Fact]
    public void Calculate_SubscriberOnSaturday_ReturnsHalfFee()
    {
        // Arrange
        var entry = new DateTime(2026, 11, 14, 10, 0, 0);
        var exit = entry.AddHours(3);

        var session = new ParkingSession
        {
            EntryTime = entry,
            ExitTime = exit,
            Vehicle = VehicleType.Car,
            IsSubscriber = true
        };

        // Act
        var result = new ParkingFeeCalculator().Calculate(session);

        // Assert
        result.Should().Be(15m);
    }

}
