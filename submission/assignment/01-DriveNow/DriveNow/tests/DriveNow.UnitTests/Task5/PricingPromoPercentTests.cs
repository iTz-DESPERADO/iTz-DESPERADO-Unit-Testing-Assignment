using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DriveNow.UnitTests.Task5;

public class PricingPromoPercentTests
{
    [Theory]
    // AI suggestion: zero and normal promo percentages.
    [InlineData(0, 0)]
    [InlineData(5, 30)]
    [InlineData(15, 90)]
    [Trait("Rule", "BR-35")]
    public void Calculate_PromoPercent_ReturnsPercentageOfBase(int percent, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var promo = new PromoCode
        {
            Code = "SAMPLE", DiscountPercent = percent, MinDays = 1,
            ValidFrom = new DateTime(2026, 11, 1),
            ValidUntil = new DateTime(2026, 12, 1),
            MaxUses = 100
        };
        var sut = new PricingService(new PromoPercentTimeProvider(), new PromoPercentRepository(promo));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic, PromoCode = "sample" };

        // Act
        var result = sut.Calculate(customer, vehicle, request);

        // Assert
        result.PromoDiscount.Should().Be(expected);
    }

    [Fact]
    [Trait("Rule", "BR-35")]
    public void Calculate_FractionalBase_ReturnsCorrectPromoDiscount()
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var promo = new PromoCode { Code = "SAVE5", DiscountPercent = 5, MinDays = 1, ValidFrom = new DateTime(2026, 11, 1), ValidUntil = new DateTime(2026, 12, 1), MaxUses = 100 };
        var sut = new PricingService(new PromoPercentTimeProvider(), new PromoPercentRepository(promo));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 100.005m, Category = VehicleCategory.Economy };
        // AI suggestion: verify promo arithmetic when BaseAmount is rounded from a fractional rate.
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic, PromoCode = "SAVE5" };

        // Act
        var result = sut.Calculate(customer, vehicle, request);

        // Assert
        result.BaseAmount.Should().Be(100.01m);
        result.PromoDiscount.Should().Be(5.00m);
    }

}

internal sealed class PromoPercentRepository(PromoCode? promo) : IPromoCodeRepository
{
    public PromoCode? GetByCode(string code) => string.Equals(promo?.Code, code, StringComparison.OrdinalIgnoreCase) ? promo : null;
    public void Update(PromoCode promo) { }
}

internal sealed class PromoPercentTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 12, 12, 0, 0, TimeSpan.Zero);
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
