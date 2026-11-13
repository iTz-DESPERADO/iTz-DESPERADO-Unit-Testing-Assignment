using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Domain.Interfaces;
using DriveNow.Domain.ValueObjects;
using DriveNow.UnitTests.Task6;
using FluentAssertions;

namespace DriveNow.UnitTests.Task7;

public class ReturnServiceTests
{
    [Theory]
    [InlineData(BookingStatus.Reserved)]
    [InlineData(BookingStatus.Completed)]
    [Trait("Rule", "BR-41")]
    public void ProcessReturn_BookingIsNotActive_ThrowsInvalidBookingStateException(BookingStatus status)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking(status: status);
        var service = ReturnTestFactory.CreateService(booking, ReturnTestFactory.Vehicle(), ReturnTestFactory.Customer(), booking.ReturnAt);

        // Act
        Action act = () => service.ProcessReturn(1, 1000, 4, 0m);

        // Assert
        act.Should().Throw<InvalidBookingStateException>()
            .WithMessage("Only active bookings can be returned");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    [Trait("Rule", "BR-43")]
    public void ProcessReturn_FuelQuartersOutsideRange_ThrowsArgumentException(int fuelQuarters)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking();
        var service = ReturnTestFactory.CreateService(booking, ReturnTestFactory.Vehicle(), ReturnTestFactory.Customer(), booking.ReturnAt);

        // Act
        Action act = () => service.ProcessReturn(1, 1000, fuelQuarters, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Fuel quarters must be between 0 and 4");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(59, 0)]
    [InlineData(60, 60)]
    [InlineData(61, 120)]
    // AI suggestion: check rounding within the hourly-fee range.
    [InlineData(119, 120)]
    [InlineData(120, 120)]
    [InlineData(121, 180)]
    [InlineData(239, 240)]
    [InlineData(240, 240)]
    [InlineData(241, 600)]
    [InlineData(1440, 600)]
    [InlineData(1441, 1200)]
    [Trait("Rule", "BR-41")]
    public void ProcessReturn_LateMinutes_ReturnsCorrectLateFee(int lateMinutes, decimal expectedFee)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking(returnAt: new DateTime(2026, 11, 12, 10, 0, 0));
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(dailyRate: 600m),
            ReturnTestFactory.Customer(),
            booking.ReturnAt.AddMinutes(lateMinutes));

        // Act
        var report = service.ProcessReturn(1, 1000, 4, 0m);

        // Assert
        report.LateMinutes.Should().Be(lateMinutes);
        report.LateFee.Should().Be(expectedFee);
    }

    [Fact]
    [Trait("Rule", "BR-41")]
    public void ProcessReturn_FiftyNineMinutesAndFiftyNineSecondsLate_ReturnsNoLateFee()
    {
        // AI suggestion: late minutes are whole minutes, so seconds are truncated.
        // Arrange
        var booking = ReturnTestFactory.Booking(returnAt: new DateTime(2026, 11, 12, 10, 0, 0));
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(),
            ReturnTestFactory.Customer(),
            booking.ReturnAt.AddMinutes(59).AddSeconds(59));

        // Act
        var report = service.ProcessReturn(1, 1000, 4, 0m);

        // Assert
        report.LateMinutes.Should().Be(59);
        report.LateFee.Should().Be(0m);
    }

    [Theory]
    [InlineData(VehicleCategory.Economy, 500, 0, 0)]
    [InlineData(VehicleCategory.Economy, 501, 1, 0.75)]
    [InlineData(VehicleCategory.Luxury, 600, 100, 150)]
    // AI suggestion: verify the Luxury rate at one kilometre over the allowance.
    [InlineData(VehicleCategory.Luxury, 501, 1, 1.50)]
    [Trait("Rule", "BR-42")]
    public void ProcessReturn_DistanceOverAllowance_ReturnsCorrectMileageFee(
        VehicleCategory category,
        int driven,
        int expectedExtraKm,
        decimal expectedFee)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking(rentalDays: 2);
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(category: category),
            ReturnTestFactory.Customer(),
            booking.ReturnAt);

        // Act
        var report = service.ProcessReturn(1, booking.StartMileage + driven, 4, 0m);

        // Assert
        report.AllowedKm.Should().Be(500);
        report.ExtraKm.Should().Be(expectedExtraKm);
        report.MileageFee.Should().Be(expectedFee);
    }

    [Theory]
    [InlineData(VehicleCategory.Economy, 4, 0)]
    [InlineData(VehicleCategory.Economy, 3, 200)]
    [InlineData(VehicleCategory.SUV, 2, 600)]
    // AI suggestion: cover the remaining fuel values and Van independently.
    [InlineData(VehicleCategory.Economy, 0, 800)]
    [InlineData(VehicleCategory.Van, 1, 900)]
    [Trait("Rule", "BR-43")]
    public void ProcessReturn_MissingFuelQuarters_ReturnsCorrectFuelFee(
        VehicleCategory category,
        int fuelQuarters,
        decimal expectedFee)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking();
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(category: category),
            ReturnTestFactory.Customer(),
            booking.ReturnAt);

        // Act
        var report = service.ProcessReturn(1, 1000, fuelQuarters, 0m);

        // Assert
        report.MissingFuelQuarters.Should().Be(4 - fuelQuarters);
        report.FuelFee.Should().Be(expectedFee);
    }

    [Theory]
    [InlineData(InsurancePlan.Basic, 12000, 10000)]
    [InlineData(InsurancePlan.Standard, 2000, 2000)]
    [InlineData(InsurancePlan.Standard, 4000, 3000)]
    [InlineData(InsurancePlan.Premium, 1000, 0)]
    // AI suggestion: test the deductible boundary and zero reported damage.
    [InlineData(InsurancePlan.Basic, 0, 0)]
    [InlineData(InsurancePlan.Basic, 10000, 10000)]
    [Trait("Rule", "BR-44")]
    public void ProcessReturn_DamageAndInsurance_ReturnsCorrectDamageCharge(
        InsurancePlan insurance,
        decimal damage,
        decimal expectedCharge)
    {
        // Arrange
        var booking = ReturnTestFactory.Booking(insurance: insurance);
        var service = ReturnTestFactory.CreateService(booking, ReturnTestFactory.Vehicle(), ReturnTestFactory.Customer(), booking.ReturnAt);

        // Act
        var report = service.ProcessReturn(1, 1000, 4, damage);

        // Assert
        report.DamageCharge.Should().Be(expectedCharge);
    }

    [Fact]
    [Trait("Rule", "BR-45")]
    public void ProcessReturn_EarlyReturn_DoesNotRefundUnusedRentalDays()
    {
        // Arrange
        var booking = ReturnTestFactory.Booking(returnAt: new DateTime(2026, 11, 14, 10, 0, 0), rentalDays: 4);
        booking.Price.BaseAmount = 2400m;
        var payments = new SpyReturnPaymentGateway();
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(),
            ReturnTestFactory.Customer(),
            new DateTime(2026, 11, 12, 10, 0, 0),
            payments);

        // Act
        var report = service.ProcessReturn(1, 1000, 4, 0m);

        // Assert
        report.LateMinutes.Should().Be(0);
        booking.Price.BaseAmount.Should().Be(2400m);
        payments.Refunds.Should().BeEmpty();
    }

    [Fact]
    [Trait("Rule", "BR-46")]
    public void ProcessReturn_MultipleExtras_ChargesTheirCombinedAmountOnce()
    {
        // AI suggestion: combine late fee, mileage, fuel and damage in one return.
        // Arrange
        var booking = ReturnTestFactory.Booking(returnAt: new DateTime(2026, 11, 12, 10, 0, 0), rentalDays: 1);
        var payments = new SpyReturnPaymentGateway();
        var service = ReturnTestFactory.CreateService(
            booking,
            ReturnTestFactory.Vehicle(category: VehicleCategory.SUV),
            ReturnTestFactory.Customer(),
            booking.ReturnAt.AddMinutes(60),
            payments);

        // Act
        var report = service.ProcessReturn(1, 1251, 3, 12000m);

        // Assert
        report.TotalExtra.Should().Be(10360.75m);
        payments.Charges.Should().ContainSingle().Which.Should().Be((10, 10360.75m, "Extras DN-20261110-RT01"));
    }

    [Fact]
    [Trait("Rule", "BR-46")]
    public void ProcessReturn_ValidReturn_CompletesRentalAndHandlesPayments()
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = ReturnTestFactory.Booking(deposit: 1000m, reference: "DN-20261110-RT01");
        var vehicle = ReturnTestFactory.Vehicle(mileage: 1000);
        var customer = ReturnTestFactory.Customer(completedRentals: 4);
        var payments = new SpyReturnPaymentGateway();
        var email = new SpyEmailSender();
        var audit = new SpyAuditLog();
        var service = ReturnTestFactory.CreateService(booking, vehicle, customer, now, payments, email, audit);

        // Act
        var report = service.ProcessReturn(1, 1100, 4, 0m);

        // Assert
        booking.Status.Should().Be(BookingStatus.Completed);
        booking.ActualReturnAt.Should().Be(now);
        booking.EndMileage.Should().Be(1100);
        booking.ExtraCharges.Should().Be(report.TotalExtra);
        vehicle.Mileage.Should().Be(1100);
        customer.CompletedRentals.Should().Be(5);
        customer.LoyaltyTier.Should().Be(LoyaltyTier.Silver);
        payments.Refunds.Should().ContainSingle().Which.Should().Be((10, 1000m, "Deposit refund DN-20261110-RT01"));
        payments.Charges.Should().BeEmpty();
        email.Calls.Should().ContainSingle().Which.To.Should().Be("gamal@example.com");
        audit.Messages.Should().ContainSingle().Which.Should().Be("Booking DN-20261110-RT01 returned");
    }
}

internal sealed class SpyReturnPaymentGateway : IPaymentGateway
{
    public List<(int CustomerId, decimal Amount, string Description)> Charges { get; } = [];
    public List<(int CustomerId, decimal Amount, string Description)> Refunds { get; } = [];
    public string Charge(int customerId, decimal amount, string description)
    {
        Charges.Add((customerId, amount, description));
        return "payment-1";
    }

    public void Refund(int customerId, decimal amount, string description) => Refunds.Add((customerId, amount, description));
}

internal static class ReturnTestFactory
{
    public static Booking Booking(
        BookingStatus status = BookingStatus.Active,
        DateTime? returnAt = null,
        int rentalDays = 2,
        InsurancePlan insurance = InsurancePlan.Basic,
        decimal deposit = 0m,
        string reference = "DN-20261110-RT01") => new()
    {
        Id = 1,
        CustomerId = 10,
        VehicleId = 20,
        Reference = reference,
        PickupAt = new DateTime(2026, 11, 10, 10, 0, 0),
        ReturnAt = returnAt ?? new DateTime(2026, 11, 12, 10, 0, 0),
        StartMileage = 1000,
        Status = status,
        Deposit = deposit,
        InsurancePlan = insurance,
        Price = new PriceBreakdown { RentalDays = rentalDays }
    };

    public static Vehicle Vehicle(VehicleCategory category = VehicleCategory.Economy, decimal dailyRate = 600m, int mileage = 1000) => new()
    {
        Id = 20,
        Category = category,
        DailyRate = dailyRate,
        Mileage = mileage,
        IsAvailable = true
    };

    public static Customer Customer(int completedRentals = 0) => new()
    {
        Id = 10,
        Email = "gamal@example.com",
        CompletedRentals = completedRentals,
        LoyaltyTier = LoyaltyTier.None
    };

    public static ReturnService CreateService(
        Booking booking,
        Vehicle vehicle,
        Customer customer,
        DateTime now,
        IPaymentGateway? payments = null,
        IEmailSender? email = null,
        IAuditLog? audit = null)
    {
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(booking);
        return new ReturnService(
            new FixedBookingTimeProvider(new DateTimeOffset(now, TimeSpan.Zero)),
            bookings,
            new StubVehicleRepository(vehicle),
            new StubCustomerRepository(customer),
            payments ?? new SpyReturnPaymentGateway(),
            email ?? new SpyEmailSender(),
            audit ?? new SpyAuditLog());
    }
}
