using DriveNow.Application.Models;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using FluentAssertions;

namespace DriveNow.UnitTests.Task6;

public class BookingServiceRulesTests
{
    [Fact]
    [Trait("Rule", "BR-10")]
    public void CreateBooking_CustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(customerExists: false);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<CustomerNotFoundException>()
            .WithMessage("Customer #10 not found");
    }

    [Fact]
    [Trait("Rule", "BR-11")]
    public void CreateBooking_CustomerIsBlacklisted_RejectsBooking()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(customer: BookingTestFactory.Customer(blacklisted: true));

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Customer is blacklisted");
    }

    [Fact]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_DriverIsTwenty_RejectsBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(dateOfBirth: pickup.AddYears(-20).AddDays(1));
        var service = BookingTestFactory.CreateService(customer: customer);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Driver must be at least 21");
    }

    [Fact]
    [Trait("Rule", "BR-13")]
    public void CreateBooking_LicenseIsLessThanOneYearOld_RejectsBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(licenseIssued: pickup.AddYears(-1).AddDays(1));
        var service = BookingTestFactory.CreateService(customer: customer);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("License must be held for at least 1 year");
    }

    [Fact]
    [Trait("Rule", "BR-14")]
    public void CreateBooking_VehicleDoesNotExist_ThrowsVehicleNotFoundException()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(vehicleExists: false);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<VehicleNotFoundException>()
            .WithMessage("Vehicle #20 not found");
    }

    [Fact]
    [Trait("Rule", "BR-15")]
    public void CreateBooking_VehicleIsUnavailable_RejectsBooking()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(vehicle: BookingTestFactory.Vehicle(available: false));

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Vehicle is not available");
    }

    [Fact]
    [Trait("Rule", "BR-16")]
    public void CreateBooking_LuxuryDriverIsExactlyTwentyFiveWithThreeLicenseYears_CreatesBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(
            dateOfBirth: pickup.AddYears(-25),
            licenseIssued: pickup.AddYears(-3));
        var service = BookingTestFactory.CreateService(
            customer: customer,
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Luxury));

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(1)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-17")]
    public void CreateBooking_VanDriverHasExactlyTwoLicenseYears_CreatesBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(licenseIssued: pickup.AddYears(-2));
        var service = BookingTestFactory.CreateService(
            customer: customer,
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Van));

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(1)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-18")]
    public void CreateBooking_PickupIsExactlyTwoHoursFromNow_CreatesBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var service = BookingTestFactory.CreateService();

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        booking.PickupAt.Should().Be(pickup);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(30 * 24 + 1)]
    [Trait("Rule", "BR-19")]
    public void CreateBooking_InvalidRentalPeriod_RejectsBooking(int returnOffsetHours)
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var service = BookingTestFactory.CreateService();
        var request = BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddHours(returnOffsetHours));

        // Act
        Action act = () => service.CreateBooking(request);

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Rental period is invalid");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [Trait("Rule", "BR-20")]
    public void CreateBooking_AdditionalDriversOutsideRange_RejectsBooking(int additionalDrivers)
    {
        // Arrange
        var service = BookingTestFactory.CreateService();

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(additionalDrivers: additionalDrivers));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Up to 3 additional drivers allowed");
    }

    [Fact]
    [Trait("Rule", "BR-21")]
    public void CreateBooking_PickupEqualsExistingReturn_CreatesBooking()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(26);
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(new Booking
        {
            CustomerId = 99,
            VehicleId = 20,
            PickupAt = BookingTestFactory.Now.DateTime.AddHours(2),
            ReturnAt = pickup,
            Status = BookingStatus.Reserved
        });
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(1)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-22")]
    public void CreateBooking_CustomerHasTwoOpenBookings_RejectsBooking()
    {
        // Arrange
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(new Booking { CustomerId = 10, VehicleId = 50, Status = BookingStatus.Reserved });
        bookings.Items.Add(new Booking { CustomerId = 10, VehicleId = 51, Status = BookingStatus.Active });
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Customer already has 2 active bookings");
    }

    [Fact]
    [Trait("Rule", "BR-23")]
    public void CreateBooking_ValidRequest_CalculatesPrice()
    {
        // Arrange
        var service = BookingTestFactory.CreateService();

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Price.RentalDays.Should().Be(1);
        booking.Price.BaseAmount.Should().Be(600m);
    }

    [Fact]
    [Trait("Rule", "BR-24")]
    public void CreateBooking_ValidRequest_SetsReservedStateAndBookingData()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(vehicle: BookingTestFactory.Vehicle(mileage: 4321));

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
        booking.CreatedAt.Should().Be(BookingTestFactory.Now.DateTime);
        booking.StartMileage.Should().Be(4321);
    }

    [Fact]
    [Trait("Rule", "BR-25")]
    public void CreateBooking_ValidRequest_UsesGeneratedReference()
    {
        // Arrange
        var service = BookingTestFactory.CreateService(reference: "DN-20261112-AB12");

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Reference.Should().Be("DN-20261112-AB12");
    }

    [Theory]
    [InlineData(VehicleCategory.Economy, InsurancePlan.Basic, 1000)]
    [InlineData(VehicleCategory.Compact, InsurancePlan.Basic, 1000)]
    [InlineData(VehicleCategory.SUV, InsurancePlan.Basic, 2500)]
    [InlineData(VehicleCategory.Van, InsurancePlan.Basic, 2500)]
    [InlineData(VehicleCategory.Luxury, InsurancePlan.Basic, 5000)]
    [InlineData(VehicleCategory.Luxury, InsurancePlan.Premium, 2500)]
    [Trait("Rule", "BR-26")]
    public void CreateBooking_NonPlatinumCustomer_ChargesCorrectDeposit(
        VehicleCategory category,
        InsurancePlan insurance,
        decimal expectedDeposit)
    {
        // Arrange
        var payments = new HandWrittenPaymentMock();
        var service = BookingTestFactory.CreateService(
            vehicle: BookingTestFactory.Vehicle(category: category),
            payments: payments);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(insurance: insurance));

        // Assert
        booking.Deposit.Should().Be(expectedDeposit);
        payments.Charges.Should().ContainSingle()
            .Which.Should().Be((10, expectedDeposit, $"Deposit {booking.Reference}"));
    }

    [Fact]
    [Trait("Rule", "BR-26")]
    public void CreateBooking_PlatinumCustomer_DoesNotChargeDeposit()
    {
        // Arrange
        var payments = new HandWrittenPaymentMock();
        var service = BookingTestFactory.CreateService(
            customer: BookingTestFactory.Customer(tier: LoyaltyTier.Platinum),
            payments: payments);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Deposit.Should().Be(0m);
        payments.Charges.Should().BeEmpty();
    }

    [Fact]
    [Trait("Rule", "BR-27")]
    public void CreateBooking_ValidPromo_StoresBookingAndSendsConfirmation()
    {
        // Arrange
        var promo = new PromoCode
        {
            Code = "SAVE10",
            DiscountPercent = 10,
            ValidFrom = new DateTime(2026, 1, 1),
            ValidUntil = new DateTime(2026, 12, 31),
            MinDays = 1,
            MaxUses = 10
        };
        var promos = new StubPromoRepository(promo);
        var bookings = new FakeBookingRepository();
        var sms = new SpySmsSender();
        var email = new SpyEmailSender();
        var audit = new SpyAuditLog();
        var service = BookingTestFactory.CreateService(
            bookings: bookings,
            promos: promos,
            sms: sms,
            email: email,
            audit: audit);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(promoCode: "SAVE10"));

        // Assert
        promo.UsedCount.Should().Be(1);
        promos.UpdateCalls.Should().Be(1);
        bookings.Items.Should().ContainSingle().Which.Should().BeSameAs(booking);
        sms.Calls.Should().ContainSingle().Which.Should().Be(("5550100", $"Booking {booking.Reference} confirmed"));
        email.Calls.Should().ContainSingle().Which.Should().Be(("gamal@example.com", $"Booking {booking.Reference} confirmed", $"Booking {booking.Reference} confirmed"));
        audit.Messages.Should().ContainSingle().Which.Should().Be($"Booking {booking.Reference} created");
    }
}
