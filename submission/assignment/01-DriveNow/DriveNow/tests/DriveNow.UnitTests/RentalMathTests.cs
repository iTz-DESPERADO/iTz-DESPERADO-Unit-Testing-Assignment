using Xunit;
using FluentAssertions;
using DriveNow.Domain.Services;
using System.Globalization;

namespace DriveNow.UnitTests;

public class RentalMathTests
{
    [Theory]
    [InlineData("2000-03-29", "2024-03-28", 23)]
    [InlineData("2000-03-29", "2024-03-29", 24)]
    [InlineData("2000-03-29", "2024-03-30", 24)]

    [InlineData("2000-02-29", "2024-02-28", 23)]
    [InlineData("2000-02-29", "2024-02-29", 24)]
    [InlineData("2000-02-29", "2024-03-01", 24)]

    [InlineData("2000-02-29", "2025-02-27", 24)]
    [InlineData("2000-02-29", "2025-02-28", 25)]
    [InlineData("2000-02-29", "2025-03-01", 25)]

    [InlineData("2000-03-29", "2000-03-29", 0)]

    [Trait("Rule", "BR-01")]
    public void CalculateAge_ValidDate_ReturnsCorrectAge(string dateOfBirth, string onDate, int expected)
    {
        // Arrange
        var birthDate = DateTime.ParseExact(
            dateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var calculationDate = DateTime.ParseExact(
            onDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateAge(birthDate, calculationDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2020-05-15", "2025-05-15", 5)]
    [InlineData("2020-05-15", "2025-05-16", 5)]

    [InlineData("2020-02-29", "2025-02-28", 5)]
    [InlineData("2020-02-29", "2025-03-01", 5)]

    [InlineData("2020-02-29", "2024-02-29", 4)]
    [InlineData("2020-02-29", "2024-03-01", 4)]

    // Before anniversary
    [InlineData("2020-05-15", "2025-05-14", 4)]

    // Leap day in a non-leap year
    [InlineData("2020-02-29", "2025-02-27", 4)]

    // Before leap-day anniversary in a leap year
    [InlineData("2020-02-29", "2024-02-28", 3)]

    // Same issue date
    [InlineData("2025-05-15", "2025-05-15", 0)]

    // Less than one complete year
    [InlineData("2025-05-15", "2026-05-14", 0)]

    // Calendar-year boundary
    [InlineData("2020-12-31", "2025-01-01", 4)]

    [Trait("Rule", "BR-02")]
    public void CalculateLicenseYears_GivenIssueDates_ReturnsCorrectLicenseYears(string issueDate, string onDate, int expected)
    {
        // Arrange
        var issue = DateTime.ParseExact(
            issueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var calculationDate = DateTime.ParseExact(
            onDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateLicenseYears(issue, calculationDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2026-11-04 10:00:00", "2026-11-05 10:00:00", 1)]
    [InlineData("2026-11-04 10:00:00", "2026-11-05 11:00:00", 1)]
    // AI suggestion: one minute before the grace period ends.
    [InlineData("2026-11-04 10:00:00", "2026-11-05 10:59:00", 1)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_WithinGracePeriod_ReturnsOneDay(string pickup, string returnAt, int expected)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2026-11-04 10:00:00", "2026-11-05 11:01:00", 2)]
    // AI suggestion: exceeding the grace period by one second adds a day.
    [InlineData("2026-11-04 10:00:00", "2026-11-05 11:00:01", 2)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_BeyondGracePeriod_ReturnsTwoDays(string pickup, string returnAt, int expected)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2026-11-04 10:00:00", "2026-11-04 10:00:01", 1)]
    // AI suggestion: short rental example and just before the first full day.
    [InlineData("2026-11-04 10:00:00", "2026-11-04 13:00:00", 1)]
    [InlineData("2026-11-04 10:00:00", "2026-11-05 09:59:00", 1)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_MinimumDuration_ReturnsOneDay(string pickup, string returnAt, int expected)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // AI suggestion: count multiple complete 24-hour blocks.
    [InlineData("2026-11-04 10:00:00", "2026-11-06 10:00:00", 2)]
    [InlineData("2026-11-04 10:00:00", "2026-11-07 10:00:00", 3)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_MultipleFullDays_ReturnsFullDayCount(string pickup, string returnAt, int expected)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // AI suggestion: grace boundaries after two full days (59 / 60 / 61 minutes).
    [InlineData("2026-11-04 10:00:00", "2026-11-06 10:59:00", 2)]
    [InlineData("2026-11-04 10:00:00", "2026-11-06 11:00:00", 2)]
    [InlineData("2026-11-04 10:00:00", "2026-11-06 11:01:00", 3)]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_GracePeriodAfterTwoDays_ReturnsCorrectDays(string pickup, string returnAt, int expected)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // Act
        var result = RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2026-11-04 10:00:00", "2026-11-04 10:00:00")]
    [InlineData("2026-11-04 10:00:00", "2026-11-04 09:59:59")]
    [Trait("Rule", "BR-03")]
    public void CalculateRentalDays_InvalidReturnTime_ThrowsArgumentException(string pickup, string returnAt)
    {
        // Arrange
        var pickupDate = DateTime.ParseExact(
            pickup,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        var returnDate = DateTime.ParseExact(
            returnAt,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        // Act
        Action act = () => RentalMathService.CalculateRentalDays(pickupDate, returnDate);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Return must be after pickup");
    }
}
