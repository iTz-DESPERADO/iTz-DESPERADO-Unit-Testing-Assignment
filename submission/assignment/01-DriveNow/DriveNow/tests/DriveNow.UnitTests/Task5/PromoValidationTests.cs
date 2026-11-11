using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DriveNow.UnitTests.Task5;

public class PromoValidationTests
{
    [Theory]
    [ClassData(typeof(PromoValidationCases))]
    [Trait("Rule", "BR-36")]
    public void Calculate_PromoValidation_ReturnsExpectedOutcome(string scenario, PromoCode? promo, int days,
        VehicleCategory category, string? expectedMessage)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var pricing = new PricingService(new FixedPromoTimeProvider(), new PromoCodeRepository(promo));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = category };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(days), InsurancePlan = InsurancePlan.Basic, PromoCode = "SAVE15" };

        // Act
        Action act = () => pricing.Calculate(customer, vehicle, request);

        // Assert
        if (expectedMessage is null)
            act.Should().NotThrow(because: scenario);
        else
            act.Should().Throw<InvalidPromoCodeException>(because: scenario)
                .WithMessage(expectedMessage);
    }
}

internal sealed class PromoCodeRepository(PromoCode? promo) : IPromoCodeRepository
{
    public PromoCode? GetByCode(string code) => string.Equals(promo?.Code, code, StringComparison.OrdinalIgnoreCase) ? promo : null;
    public void Update(PromoCode promo) { }
}

internal sealed class FixedPromoTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 12, 12, 0, 0, TimeSpan.Zero);
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
