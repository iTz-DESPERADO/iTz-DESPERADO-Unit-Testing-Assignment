using System;
using System.Linq;
using System.Threading.Tasks;
using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DriveNow.UnitTests.Task5;

public class PricingRentalAndSnapshotTests
{
    [Theory]
    [InlineData(0, 180, 1, 600)]
    // AI suggestion: exact full-day and just-inside grace-period boundaries.
    [InlineData(1, 0, 1, 600)]
    [InlineData(1, 59, 1, 600)]
    [InlineData(1, 60, 1, 600)]
    [InlineData(1, 61, 2, 1200)]
    // AI suggestion: verify full-day counting beyond the first day.
    [InlineData(2, 0, 2, 1200)]
    [InlineData(2, 60, 2, 1200)]
    [InlineData(2, 61, 3, 1800)]
    [Trait("Rule", "BR-30")]
    public void Calculate_RentalDuration_ReturnsCorrectDaysAndBaseAmount(int days, int extraMinutes, int expectedDays, int expectedBase)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new StubRentalPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(days).AddMinutes(extraMinutes), InsurancePlan = InsurancePlan.Basic };

        // Act
        var actual = pricing.Calculate(customer, vehicle, request);

        // Assert
        actual.RentalDays.Should().Be(expectedDays);
        actual.BaseAmount.Should().Be(expectedBase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Rule", "BR-30")]
    public void Calculate_InvalidRentalSpan_ThrowsArgumentException(int offset)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new StubRentalPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddMinutes(offset), InsurancePlan = InsurancePlan.Basic };

        // Act
        Action act = () => pricing.Calculate(customer, vehicle, request);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Return must be after pickup");
    }

    [Fact]
    [Trait("Rule", "BR-35")]
    public void Calculate_ValidPromo_ReturnsPercentOfBase()
    {
        // Arrange
        var promo = new PromoCode { Code = "SAVE15", DiscountPercent = 15,
            ValidFrom = new DateTime(2026, 11, 1), ValidUntil = new DateTime(2026, 12, 1),
            MinDays = 1, MaxUses = 100 };
        var pricing = new PricingService(new RentalPromoTimeProvider(), new StubRentalPromoRepository(promo));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic, PromoCode = "save15" };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.PromoDiscount.Should().Be(90m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [Trait("Rule", "BR-40")]
    public async Task Calculate_WorkedExample_SnapshotMatchesApproved(int exampleIndex)
    {
        // Arrange
        var data = PricingExamples.Cases().ElementAt(exampleIndex);
        var customer = (Customer)data[0];
        var vehicle = (Vehicle)data[1];
        var request = (BookingRequest)data[2];
        var promo = (PromoCode?)data[3];
        var pricing = new PricingService(new RentalPromoTimeProvider(), new StubRentalPromoRepository(promo));

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        await Verifier.Verify(price).UseParameters(exampleIndex);
    }
}

internal sealed class StubRentalPromoRepository(PromoCode? promo) : IPromoCodeRepository
{
    public PromoCode? GetByCode(string code) => string.Equals(promo?.Code, code, StringComparison.OrdinalIgnoreCase) ? promo : null;
    public void Update(PromoCode promo) { }
}

internal sealed class RentalPromoTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 12, 12, 0, 0, TimeSpan.Zero);
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
