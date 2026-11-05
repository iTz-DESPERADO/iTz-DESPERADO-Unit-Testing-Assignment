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
        var result = RentalMath.CalculateAge(birthDate, calculationDate);

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
        var result = RentalMath.CalculateLicenseYears(issue, calculationDate);

        // Assert
        result.Should().Be(expected);
    }
}
