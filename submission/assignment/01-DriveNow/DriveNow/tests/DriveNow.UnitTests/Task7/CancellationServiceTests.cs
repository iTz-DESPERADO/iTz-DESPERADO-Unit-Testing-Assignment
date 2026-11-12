using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Domain.Interfaces;
using DriveNow.UnitTests.Task6;
using FluentAssertions;

namespace DriveNow.UnitTests.Task7;

public class CancellationServiceTests
{
    [Fact]
    [Trait("Rule", "BR-50")]
    public void Cancel_BookingDoesNotExist_ThrowsBookingNotFoundException()
    {
        // AI suggestion: test the missing booking validation.
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var bookings = new FakeBookingRepository();
        var service = new CancellationService(
            new FixedBookingTimeProvider(new DateTimeOffset(now, TimeSpan.Zero)),
            new StubPromoRepository(),
            bookings,
            new StubVehicleRepository(new Vehicle()),
            new StubCustomerRepository(CancellationTestFactory.Customer()),
            new SpyReturnPaymentGateway(),
            new SpySmsSender(),
            new SpyEmailSender(),
            new SpyAuditLog());

        // Act
        Action act = () => service.Cancel(99);

        // Assert
        act.Should().Throw<BookingNotFoundException>()
            .WithMessage("Booking #99 not found");
    }

    [Theory]
    [InlineData(BookingStatus.Active)]
    [InlineData(BookingStatus.Completed)]
    [Trait("Rule", "BR-50")]
    public void Cancel_BookingIsNotReserved_ThrowsInvalidBookingStateException(BookingStatus status)
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now.AddDays(3), status: status);
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now);

        // Act
        Action act = () => service.Cancel(1);

        // Assert
        act.Should().Throw<InvalidBookingStateException>()
            .WithMessage("Only reserved bookings can be cancelled");
    }

    [Fact]
    [Trait("Rule", "BR-50")]
    public void Cancel_PickupTimeHasArrived_ThrowsInvalidBookingStateException()
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now);
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now);

        // Act
        Action act = () => service.Cancel(1);

        // Assert
        act.Should().Throw<InvalidBookingStateException>()
            .WithMessage("Booking has already started");
    }

    [Theory]
    [InlineData(48, LoyaltyTier.None, 0)]
    [InlineData(47.999, LoyaltyTier.None, 300)]
    [InlineData(24, LoyaltyTier.None, 300)]
    [InlineData(23.999, LoyaltyTier.None, 600)]
    [InlineData(1, LoyaltyTier.Platinum, 0)]
    [Trait("Rule", "BR-51")]
    public void Cancel_HoursUntilPickup_ReturnsCorrectFee(double hoursUntilPickup, LoyaltyTier tier, decimal expectedFee)
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now.AddHours(hoursUntilPickup), deposit: 1000m);
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(tier: tier), now);

        // Act
        var fee = service.Cancel(1);

        // Assert
        fee.Should().Be(expectedFee);
    }

    [Fact]
    [Trait("Rule", "BR-52")]
    public void Cancel_DepositIsMoreThanFee_RefundsTheDifference()
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now.AddHours(24), deposit: 1000m);
        var payments = new SpyReturnPaymentGateway();
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now, payments: payments);

        // Act
        var fee = service.Cancel(1);

        // Assert
        fee.Should().Be(300m);
        payments.Refunds.Should().ContainSingle().Which.Should().Be((10, 700m, "Deposit refund DN-20261114-CN01"));
        payments.Charges.Should().BeEmpty();
    }

    [Fact]
    [Trait("Rule", "BR-52")]
    public void Cancel_DepositEqualsFee_DoesNotChargeOrRefund()
    {
        // AI suggestion: check the equality boundary between deposit and fee.
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now.AddHours(24), deposit: 300m);
        var payments = new SpyReturnPaymentGateway();
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now, payments: payments);

        // Act
        var fee = service.Cancel(1);

        // Assert
        fee.Should().Be(300m);
        payments.Refunds.Should().BeEmpty();
        payments.Charges.Should().BeEmpty();
    }

    [Fact]
    [Trait("Rule", "BR-52")]
    public void Cancel_FeeIsMoreThanDeposit_ChargesTheDifference()
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var booking = CancellationTestFactory.Booking(now.AddHours(23), deposit: 100m);
        var payments = new SpyReturnPaymentGateway();
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now, payments: payments);

        // Act
        var fee = service.Cancel(1);

        // Assert
        fee.Should().Be(600m);
        payments.Refunds.Should().BeEmpty();
        payments.Charges.Should().ContainSingle().Which.Should().Be((10, 500m, "Cancellation DN-20261114-CN01"));
    }

    [Fact]
    [Trait("Rule", "BR-53")]
    public void Cancel_ValidBooking_CancelsAndSendsNotifications()
    {
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var promo = new PromoCode { Code = "SAVE10", UsedCount = 1 };
        var promos = new StubPromoRepository(promo);
        var booking = CancellationTestFactory.Booking(now.AddDays(3), promoCode: "SAVE10");
        var sms = new SpySmsSender();
        var email = new SpyEmailSender();
        var audit = new SpyAuditLog();
        var service = CancellationTestFactory.CreateService(
            booking,
            CancellationTestFactory.Customer(),
            now,
            promos: promos,
            sms: sms,
            email: email,
            audit: audit);

        // Act
        var fee = service.Cancel(1);

        // Assert
        fee.Should().Be(0m);
        booking.Status.Should().Be(BookingStatus.Cancelled);
        promo.UsedCount.Should().Be(0);
        promos.UpdateCalls.Should().Be(1);
        sms.Calls.Should().ContainSingle().Which.Should().Be(("5550100", "Booking DN-20261114-CN01 cancelled"));
        email.Calls.Should().ContainSingle().Which.Should().Be(("gamal@example.com", "Booking DN-20261114-CN01 cancelled", "Booking DN-20261114-CN01 cancelled"));
        audit.Messages.Should().ContainSingle().Which.Should().Be("Booking DN-20261114-CN01 cancelled");
    }

    [Fact]
    [Trait("Rule", "BR-53")]
    public void Cancel_BookingWithoutPromo_DoesNotUpdatePromo()
    {
        // AI suggestion: cancellation without a promo must not change promo data.
        // Arrange
        var now = new DateTime(2026, 11, 12, 10, 0, 0);
        var promos = new StubPromoRepository(new PromoCode { Code = "SAVE10", UsedCount = 4 });
        var booking = CancellationTestFactory.Booking(now.AddDays(3));
        var service = CancellationTestFactory.CreateService(booking, CancellationTestFactory.Customer(), now, promos: promos);

        // Act
        service.Cancel(1);

        // Assert
        promos.Promo!.UsedCount.Should().Be(4);
        promos.UpdateCalls.Should().Be(0);
    }
}

internal static class CancellationTestFactory
{
    public static Booking Booking(DateTime pickupAt, BookingStatus status = BookingStatus.Reserved, decimal deposit = 0m, string? promoCode = null) => new()
    {
        Id = 1,
        CustomerId = 10,
        VehicleId = 20,
        Reference = "DN-20261114-CN01",
        PickupAt = pickupAt,
        ReturnAt = pickupAt.AddDays(1),
        Status = status,
        Deposit = deposit,
        PromoCode = promoCode
    };

    public static Customer Customer(LoyaltyTier tier = LoyaltyTier.None) => new()
    {
        Id = 10,
        Email = "gamal@example.com",
        Phone = "5550100",
        LoyaltyTier = tier
    };

    public static CancellationService CreateService(
        Booking booking,
        Customer customer,
        DateTime now,
        StubPromoRepository? promos = null,
        IPaymentGateway? payments = null,
        ISmsSender? sms = null,
        IEmailSender? email = null,
        IAuditLog? audit = null)
    {
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(booking);
        return new CancellationService(
            new FixedBookingTimeProvider(new DateTimeOffset(now, TimeSpan.Zero)),
            promos ?? new StubPromoRepository(),
            bookings,
            new StubVehicleRepository(new Vehicle { Id = 20, DailyRate = 600m }),
            new StubCustomerRepository(customer),
            payments ?? new SpyReturnPaymentGateway(),
            sms ?? new SpySmsSender(),
            email ?? new SpyEmailSender(),
            audit ?? new SpyAuditLog());
    }
}
