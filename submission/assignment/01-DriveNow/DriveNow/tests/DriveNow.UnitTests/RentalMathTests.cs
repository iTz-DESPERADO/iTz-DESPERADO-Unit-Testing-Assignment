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
}
