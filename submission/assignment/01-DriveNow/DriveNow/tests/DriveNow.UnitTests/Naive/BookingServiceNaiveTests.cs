using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace DriveNow.UnitTests.Naive;

public sealed class BookingServiceNaiveTests
{
    [Fact]
    [Trait("Rule", "BR-24")]
    [Trait("Rule", "BR-27")]
    public void CreateBooking_ValidRequest_CreatesReservedBooking()
    {
        // Arrange
        var pickup = DateTime.Today.AddDays(10);
        var request = new BookingRequest
        {
            CustomerId = 2, // Eligible, not blacklisted
            VehicleId = 1,  // Available Economy vehicle
            PickupAt = pickup,
            ReturnAt = pickup.AddDays(5),
            InsurancePlan = InsurancePlan.Basic,
            AdditionalDrivers = 0,
            PromoCode = null
        };
        var sut = new BookingService();

        // Act
        var booking = sut.CreateBooking(request);

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
        booking.CustomerId.Should().Be(request.CustomerId);
        booking.VehicleId.Should().Be(request.VehicleId);
        booking.Id.Should().BeGreaterThan(0);

        var savedBooking = new BookingRepository().GetById(booking.Id);
        savedBooking.Should().BeSameAs(booking);
    }

    [Fact]
    [Trait("Rule", "BR-26")]
    public void CreateBooking_PlatinumCustomer_HasZeroDeposit()
    {
        // Arrange
        // This overlaps with the Happy Path test on vehicle #1 ON PURPOSE.
        // Each request is valid against the original seed state, but both
        // successful bookings cannot coexist in the shared static list.
        var pickup = DateTime.Today.AddDays(12);
        var request = new BookingRequest
        {
            CustomerId = 5, // Seeded Platinum customer
            VehicleId = 1,  // Same vehicle as Happy Path
            PickupAt = pickup,
            ReturnAt = pickup.AddDays(2),
            InsurancePlan = InsurancePlan.Basic,
            AdditionalDrivers = 0,
            PromoCode = null
        };
        var sut = new BookingService();

        // Act
        var booking = sut.CreateBooking(request);

        // Assert
        booking.Deposit.Should().Be(0m);
        // This does NOT prove PaymentGateway.Charge() was never called
        // The current implementation does not allow a payment spy/mock injection
    }

    [Fact]
    [Trait("Rule", "BR-21")]
    public void CreateBooking_OverlappingReservation_ThrowsBookingRejectedException()
    {
        // Arrange
        // The seeded reservation for vehicle #5 runs from today+3 at 10:00
        // to today+6 at 10:00. Our request genuinely overlaps it
        var pickup = DateTime.Today.AddDays(4).AddHours(10);
        var request = new BookingRequest
        {
            CustomerId = 2,
            VehicleId = 5,
            PickupAt = pickup,
            ReturnAt = pickup.AddDays(1),
            InsurancePlan = InsurancePlan.Basic,
            AdditionalDrivers = 0,
            PromoCode = null
        };
        var sut = new BookingService();

        // Act
        Action act = () => sut.CreateBooking(request);

        // Assert
        act.Should()
            .Throw<BookingRejectedException>()
            .WithMessage("Vehicle is already booked for these dates");
    }
}
