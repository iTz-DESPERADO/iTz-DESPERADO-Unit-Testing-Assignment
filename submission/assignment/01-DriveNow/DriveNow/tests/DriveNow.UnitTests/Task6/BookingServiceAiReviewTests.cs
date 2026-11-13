using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using FluentAssertions;

namespace DriveNow.UnitTests.Task6;

public class BookingServiceAiReviewTests
{
    [Fact]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_DayBeforeTwentyFirstBirthday_RejectsBooking()
    {
        // AI suggestion: test immediately before the age boundary.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(dateOfBirth: pickup.AddYears(-21).AddDays(1));
        var service = BookingTestFactory.CreateService(customer: customer);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Driver must be at least 21");
    }

    [Fact]
    [Trait("Rule", "BR-13")]
    public void CreateBooking_LicenseIsExactlyOneYearOld_CreatesBooking()
    {
        // AI suggestion: test the exact first license anniversary.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(licenseIssued: pickup.AddYears(-1));
        var service = BookingTestFactory.CreateService(customer: customer);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-16")]
    public void CreateBooking_LuxuryDriverIsTwentyFour_RejectsBooking()
    {
        // AI suggestion: test the Luxury age restriction separately.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(
            dateOfBirth: pickup.AddYears(-24),
            licenseIssued: pickup.AddYears(-3));
        var service = BookingTestFactory.CreateService(
            customer: customer,
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Luxury));

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Luxury vehicles require age 25+ and 3 years of license");
    }

    [Fact]
    [Trait("Rule", "BR-16")]
    public void CreateBooking_LuxuryDriverHasTwoLicenseYears_RejectsBooking()
    {
        // AI suggestion: test the Luxury license restriction separately.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(
            dateOfBirth: pickup.AddYears(-25),
            licenseIssued: pickup.AddYears(-2));
        var service = BookingTestFactory.CreateService(
            customer: customer,
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Luxury));

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Luxury vehicles require age 25+ and 3 years of license");
    }

    [Fact]
    [Trait("Rule", "BR-17")]
    public void CreateBooking_VanDriverHasOneLicenseYear_RejectsBooking()
    {
        // AI suggestion: the Van rule must be tested separately from the general license rule.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var customer = BookingTestFactory.Customer(licenseIssued: pickup.AddYears(-1));
        var service = BookingTestFactory.CreateService(
            customer: customer,
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Van));

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Vans require 2 years of license");
    }

    [Fact]
    [Trait("Rule", "BR-18")]
    public void CreateBooking_OneTickBeforeTwoHourMinimum_RejectsBooking()
    {
        // AI suggestion: test immediately before the pickup boundary.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2).AddTicks(-1);
        var service = BookingTestFactory.CreateService();

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(1)));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Pickup must be at least 2 hours from now");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [Trait("Rule", "BR-19")]
    public void CreateBooking_ReturnIsNotAfterPickup_RejectsBooking(int ticksFromPickup)
    {
        // AI suggestion: test both equal and immediately-before return times.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var service = BookingTestFactory.CreateService();
        var request = BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddTicks(ticksFromPickup));

        // Act
        Action act = () => service.CreateBooking(request);

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Rental period is invalid");
    }

    [Fact]
    [Trait("Rule", "BR-19")]
    public void CreateBooking_ReturnIsOneTickAfterPickup_CreatesBooking()
    {
        // AI suggestion: test the smallest valid rental period.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var service = BookingTestFactory.CreateService();

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddTicks(1)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-19")]
    public void CreateBooking_RentalPeriodIsExactlyThirtyDays_CreatesBooking()
    {
        // AI suggestion: test the maximum rental span exactly.
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var service = BookingTestFactory.CreateService();

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(30)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [Trait("Rule", "BR-20")]
    public void CreateBooking_AdditionalDriversAtAllowedBoundaries_CreatesBooking(int additionalDrivers)
    {
        // AI suggestion: test both valid ends of the additional-driver range.
        // Arrange
        var service = BookingTestFactory.CreateService();

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(additionalDrivers: additionalDrivers));

        // Assert
        booking.AdditionalDrivers.Should().Be(additionalDrivers);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(2, 4)]
    [InlineData(1, 2)]
    [InlineData(0, 4)]
    [Trait("Rule", "BR-21")]
    public void CreateBooking_OverlapsExistingBooking_RejectsBooking(int requestedStartHours, int requestedEndHours)
    {
        // AI suggestion: reject partial and complete overlap patterns.
        // Arrange
        var baseTime = BookingTestFactory.Now.DateTime.AddHours(2);
        var bookings = CreateRepositoryWithExistingBooking(baseTime.AddHours(1), baseTime.AddHours(3));
        var service = BookingTestFactory.CreateService(bookings: bookings);
        var request = BookingTestFactory.Request(
            pickup: baseTime.AddHours(requestedStartHours),
            returnAt: baseTime.AddHours(requestedEndHours));

        // Act
        Action act = () => service.CreateBooking(request);

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Vehicle is already booked for these dates");
    }

    [Fact]
    [Trait("Rule", "BR-21")]
    public void CreateBooking_ReturnEqualsExistingPickup_CreatesBooking()
    {
        // AI suggestion: test the second allowed back-to-back boundary.
        // Arrange
        var baseTime = BookingTestFactory.Now.DateTime.AddHours(2);
        var bookings = CreateRepositoryWithExistingBooking(baseTime.AddHours(1), baseTime.AddHours(3));
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(
            pickup: baseTime,
            returnAt: baseTime.AddHours(1)));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-22")]
    public void CreateBooking_OneOpenBookingAndOneCompletedBooking_CreatesBooking()
    {
        // AI suggestion: closed bookings must not count toward the limit.
        // Arrange
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(new Booking { CustomerId = 10, VehicleId = 50, Status = BookingStatus.Reserved });
        bookings.Items.Add(new Booking { CustomerId = 10, VehicleId = 51, Status = BookingStatus.Completed });
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    [Fact]
    [Trait("Rule", "BR-25")]
    public void CreateBooking_PickupIsInDifferentMonth_PassesPickupDateToReferenceGenerator()
    {
        // AI suggestion: the reference must use pickup date, not created-at date.
        // Arrange
        var pickup = new DateTime(2026, 12, 1, 9, 0, 0);
        var referenceGenerator = new SpyReferenceGenerator("DN-20261201-RF01");
        var service = BookingTestFactory.CreateService(referenceGenerator: referenceGenerator);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup, returnAt: pickup.AddDays(1)));

        // Assert
        booking.Reference.Should().Be("DN-20261201-RF01");
        referenceGenerator.PickupDates.Should().ContainSingle().Which.Should().Be(pickup);
    }

    [Fact]
    [Trait("Rule", "BR-27")]
    public void CreateBooking_RejectedBooking_DoesNotStoreOrSendConfirmation()
    {
        // AI suggestion: invalid bookings must not have successful-booking side effects.
        // Arrange
        var promo = new PromoCode
        {
            Code = "SAVE10",
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
            customer: BookingTestFactory.Customer(blacklisted: true),
            bookings: bookings,
            promos: promos,
            sms: sms,
            email: email,
            audit: audit);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(promoCode: "SAVE10"));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Customer is blacklisted");
        promo.UsedCount.Should().Be(0);
        bookings.Items.Should().BeEmpty();
        sms.Calls.Should().BeEmpty();
        email.Calls.Should().BeEmpty();
        audit.Messages.Should().BeEmpty();
    }

    static FakeBookingRepository CreateRepositoryWithExistingBooking(DateTime pickup, DateTime returnAt)
    {
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(new Booking
        {
            CustomerId = 99,
            VehicleId = 20,
            PickupAt = pickup,
            ReturnAt = returnAt,
            Status = BookingStatus.Reserved
        });
        return bookings;
    }
}
