using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DriveNow.UnitTests.Task5;

public class BogusFakerUsageTests
{
    [Fact]
    [Trait("Rule", "BR-32")]
    public void Calculate_BogusFixedAge_ReturnsExpectedYoungSurcharge()
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var customer = new CustomerFaker(12345)
            .WithAge(23, pickup).WithTier(LoyaltyTier.None).Generate();
        var vehicle = new VehicleFaker(12345).WithDailyRate(600m).Generate();
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic };
        var pricing = new PricingService(TimeProvider.System, new StubEmptyPromoCodeRepository());

        // Act
        var result = pricing.Calculate(customer, vehicle, request);

        // Assert
        result.YoungDriverSurcharge.Should().Be(90m);
    }

    [Fact]
    [Trait("Rule", "BR-30")]
    public void Calculate_BogusRandomName_DoesNotChangeBaseAmount()
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 10, 0, 0);
        var customer = new CustomerFaker(4455).Generate();
        var vehicle = new VehicleFaker(5533).WithDailyRate(600m).Generate();

        // Act
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic };
        var pricing = new PricingService(TimeProvider.System, new StubEmptyPromoCodeRepository());
        var result = pricing.Calculate(customer, vehicle, request);

        // Assert
        customer.FullName.Should().NotBeNullOrWhiteSpace();
        result.BaseAmount.Should().Be(600m);
    }
}

internal sealed class StubEmptyPromoCodeRepository : IPromoCodeRepository
{
    public PromoCode? GetByCode(string code) => null;
    public void Update(PromoCode promo) { }
}
