using System;
using System.Collections.Generic;
using System.Globalization;
using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;
using DriveNow.Domain.ValueObjects;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace DriveNow.UnitTests.Task5;

public class PricingServiceRulesReferenceTests
{
    [Theory]
    [InlineData("2026-11-12", 1, 0)]     // Thursday
    [InlineData("2026-11-13", 1, 120)]   // Friday
    [InlineData("2026-11-14", 1, 120)]   // Saturday
    // AI suggestion: non-weekend day and rental days that reach a weekend.
    [InlineData("2026-11-15", 1, 0)]     // Sunday
    [InlineData("2026-11-12", 2, 120)]   // Thursday + Friday
    [InlineData("2026-11-13", 2, 240)]   // Friday + Saturday
    [InlineData("2026-11-14", 2, 120)]   // Saturday + Sunday
    [InlineData("2026-11-12", 10, 480)]  // Two weekends
    [Trait("Rule", "BR-31")]
    public void Calculate_WeekendDayStarts_ReturnsCorrectWeekendSurcharge(string pickupDate, int days, int expected)
    {
        // Arrange
        var pickup = DateTime.ParseExact(pickupDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(10);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(days), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.WeekendSurcharge.Should().Be(expected);
    }

    [Theory]
    // AI suggestion: an extra rental day from BR-03 can start on a weekend day.
    [InlineData("2026-11-12", 25, 1, 120)] // Thursday + 25h 1m: Friday is day two
    [InlineData("2026-11-13", 25, 1, 240)] // Friday + 25h 1m: Friday and Saturday
    public void Calculate_GracePeriodExtraDay_ReturnsCorrectWeekendSurcharge(
        string pickupDate, int hours, int minutes, decimal expected)
    {
        // Arrange
        var pickup = DateTime.ParseExact(pickupDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(10);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddHours(hours).AddMinutes(minutes), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.WeekendSurcharge.Should().Be(expected);
    }

    [Theory]
    [InlineData(21, 90)]
    [InlineData(25, 0)]
    // AI suggestion: both remaining young-driver age boundaries.
    [InlineData(20, 0)]
    [InlineData(24, 90)]
    [Trait("Rule", "BR-32")]
    public void Calculate_DriverAge_ReturnsCorrectYoungDriverSurcharge(int age, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = pickup.Date.AddYears(-age), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.YoungDriverSurcharge.Should().Be(expected);
    }

    [Theory]
    // AI suggestion: age is calculated on pickup, including a birthday on pickup or during the rental.
    [InlineData("2005-11-08", 1, 90)] // Turns 21 on pickup
    [InlineData("2005-11-09", 2, 0)]  // Turns 21 during the rental
    [Trait("Rule", "BR-32")]
    public void Calculate_DriverBirthday_ReturnsSurchargeFromPickupAge(string birthDate, int rentalDays, decimal expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = DateTime.ParseExact(birthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(rentalDays), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.YoungDriverSurcharge.Should().Be(expected);
    }

    [Theory]
    [InlineData(7, 420)]
    [InlineData(14, 1260)]
    // AI suggestion: values immediately before the 7-day and 14-day thresholds.
    [InlineData(6, 0)]
    [InlineData(13, 780)]
    [InlineData(15, 1350)]
    [Trait("Rule", "BR-33")]
    public void Calculate_RentalDays_ReturnsCorrectDurationDiscount(int days, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(days), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.DurationDiscount.Should().Be(expected);
    }

    [Theory]
    [InlineData(LoyaltyTier.None, 0)]
    [InlineData(LoyaltyTier.Silver, 30)]
    [InlineData(LoyaltyTier.Gold, 60)]
    [InlineData(LoyaltyTier.Platinum, 90)]
    [Trait("Rule", "BR-34")]
    public void Calculate_LoyaltyTier_ReturnsCorrectLoyaltyDiscount(LoyaltyTier tier, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = tier };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.LoyaltyDiscount.Should().Be(expected);
    }

    [Theory]
    [InlineData(InsurancePlan.Basic, VehicleCategory.Economy, 0)]
    [InlineData(InsurancePlan.Standard, VehicleCategory.Economy, 150)]
    [InlineData(InsurancePlan.Premium, VehicleCategory.Luxury, 600)]
    // AI suggestion: remaining insurance plans and the Luxury multiplier.
    [InlineData(InsurancePlan.Basic, VehicleCategory.Luxury, 0)]
    [InlineData(InsurancePlan.Standard, VehicleCategory.Luxury, 300)]
    [InlineData(InsurancePlan.Premium, VehicleCategory.Economy, 300)]
    [Trait("Rule", "BR-38")]
    public void Calculate_InsurancePlanAndCategory_ReturnsCorrectInsuranceCost(InsurancePlan plan, VehicleCategory category, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = category };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = plan };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.InsuranceCost.Should().Be(expected);
    }

    [Fact]
    [Trait("Rule", "BR-38")]
    public void Calculate_PremiumLuxuryForThreeDays_ReturnsCorrectInsuranceCost()
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.None };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Luxury };
        // AI suggestion: insurance cost must also scale with rental days.
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(3), InsurancePlan = InsurancePlan.Premium };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.InsuranceCost.Should().Be(1800m);
    }

    [Theory]
    [InlineData(LoyaltyTier.None, 0, 0)]
    [InlineData(LoyaltyTier.None, 1, 100)]
    [InlineData(LoyaltyTier.Gold, 1, 0)]
    [InlineData(LoyaltyTier.Gold, 2, 100)]
    // AI suggestion: Silver is paid, Platinum receives the first free driver.
    [InlineData(LoyaltyTier.Silver, 1, 100)]
    [InlineData(LoyaltyTier.Platinum, 1, 0)]
    [InlineData(LoyaltyTier.Platinum, 3, 200)]
    [Trait("Rule", "BR-39")]
    public void Calculate_AdditionalDriversAndTier_ReturnsCorrectDriverCost(LoyaltyTier tier, int drivers, int expected)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = tier };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(1), InsurancePlan = InsurancePlan.Basic, AdditionalDrivers = drivers };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.AdditionalDriversCost.Should().Be(expected);
    }

    [Fact]
    [Trait("Rule", "BR-39")]
    public void Calculate_AdditionalDriversForMultipleDays_ReturnsCorrectDriverCost()
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 8, 10, 0, 0);
        var pricing = new PricingService(TimeProvider.System, new RulesPromoRepository(null));
        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = LoyaltyTier.Gold };
        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };
        // AI suggestion: the free driver is applied once, then cost scales with rental days.
        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(3), InsurancePlan = InsurancePlan.Basic, AdditionalDrivers = 2 };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        price.AdditionalDriversCost.Should().Be(300m);
    }

    [Theory]
    [InlineData(1, LoyaltyTier.None, 15)]   // promo only: 15%
    [InlineData(7, LoyaltyTier.Gold, 15)]   // 10% + 10% + 15% -> promo allowed 5%
    // AI suggestion: exactly 25% and no remaining promo capacity.
    [InlineData(7, LoyaltyTier.Silver, 10)]
    [InlineData(14, LoyaltyTier.Gold, 5)]
    [InlineData(7, LoyaltyTier.Silver, 5)]
    [Trait("Rule", "BR-37")]
    public void Calculate_DiscountCap_ReturnsCorrectPromoDiscount(int days, LoyaltyTier tier, int promoPercent)
    {
        // Arrange
        var pickup = new DateTime(2026, 11, 12, 9, 0, 0);

        var promo = new PromoCode { Code = "SAVE15", DiscountPercent = promoPercent,
            ValidFrom = new DateTime(2026, 11, 1), ValidUntil = new DateTime(2026, 12, 1),
            MaxUses = 100, MinDays = 1 };

        var pricing = new PricingService(new RulesPromoTimeProvider(), new RulesPromoRepository(promo));

        var customer = new Customer { DateOfBirth = new DateTime(1990, 1, 1), LoyaltyTier = tier };

        var vehicle = new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy };

        var request = new BookingRequest { PickupAt = pickup, ReturnAt = pickup.AddDays(days), InsurancePlan = InsurancePlan.Basic, PromoCode = "SAVE15" };

        // Act
        var price = pricing.Calculate(customer, vehicle, request);

        // Assert
        var expectedPercent = days >= 14 ? 15m : days >= 7 ? 10m : 0m;

        expectedPercent += tier switch { LoyaltyTier.Silver => 5m, LoyaltyTier.Gold => 10m,
            LoyaltyTier.Platinum => 15m, _ => 0m };

        var promoAllowedPercent = Math.Min(promoPercent, Math.Max(0m, 25m - expectedPercent));

        price.PromoDiscount.Should().Be(Math.Round(price.BaseAmount * promoAllowedPercent / 100m, 2,
            MidpointRounding.AwayFromZero));
    }

    [Theory]
    [MemberData(nameof(PricingExamples.Cases), MemberType = typeof(PricingExamples))]
    [Trait("Rule", "BR-40")]
    public void Calculate_WorkedExample_ReturnsWholePriceBreakdown(
        Customer customer, Vehicle vehicle, BookingRequest request, PromoCode? promo, PriceBreakdown expected)
    {
        // Arrange
        var pricing = new PricingService(new RulesPromoTimeProvider(), new RulesPromoRepository(promo));

        // Act
        var actual = pricing.Calculate(customer, vehicle, request);

        // Assert
        using (new AssertionScope())
        {
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

internal sealed class RulesPromoRepository(PromoCode? promo) : IPromoCodeRepository
{
    public PromoCode? GetByCode(string code) => string.Equals(promo?.Code, code, StringComparison.OrdinalIgnoreCase) ? promo : null;
    public void Update(PromoCode promo) { }
}

internal sealed class RulesPromoTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 12, 12, 0, 0, TimeSpan.Zero);
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}

public static class PricingExamples
{
    public static IEnumerable<object[]> Cases()
    {
        var sun = new DateTime(2026, 11, 8, 10, 0, 0);
        yield return new object[]
        {
            new Customer { DateOfBirth = sun.AddYears(-34), LoyaltyTier = LoyaltyTier.Gold },
            new Vehicle { DailyRate = 600m, Category = VehicleCategory.Economy },
            new BookingRequest { PickupAt = sun, ReturnAt = sun.AddDays(3), InsurancePlan = InsurancePlan.Standard },
            null!,
            new PriceBreakdown
            {
                RentalDays = 3, BaseAmount = 1800m, WeekendSurcharge = 0m,
                YoungDriverSurcharge = 0m, DurationDiscount = 0m, LoyaltyDiscount = 180m,
                PromoDiscount = 0m, InsuranceCost = 450m, AdditionalDriversCost = 0m,
                Total = 2070m
            }
        };
        var thu = new DateTime(2026, 11, 12, 9, 0, 0);
        var req = new BookingRequest { PickupAt = thu, ReturnAt = thu.AddDays(7).AddMinutes(30), InsurancePlan = InsurancePlan.Premium, AdditionalDrivers = 1, PromoCode = "SAVE15" };
        yield return new object[]
        {
            new Customer { DateOfBirth = thu.AddYears(-23), LoyaltyTier = LoyaltyTier.Silver },
            new Vehicle { DailyRate = 1500m, Category = VehicleCategory.SUV },
            req,
            new PromoCode { Code = "SAVE15", DiscountPercent = 15, ValidFrom = new DateTime(2026, 1, 1),
                ValidUntil = new DateTime(2027, 1, 1), MinDays = 1, MaxUses = 100, UsedCount = 0 },
            new PriceBreakdown
            {
                RentalDays = 7, BaseAmount = 10500m, WeekendSurcharge = 600m,
                YoungDriverSurcharge = 1575m, DurationDiscount = 1050m, LoyaltyDiscount = 525m,
                PromoDiscount = 1050m, InsuranceCost = 2100m, AdditionalDriversCost = 700m,
                Total = 12850m
            }
        };
    }
}
