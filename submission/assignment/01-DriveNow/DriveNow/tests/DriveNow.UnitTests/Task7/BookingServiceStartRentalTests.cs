using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.UnitTests.Task6;
using FluentAssertions;

namespace DriveNow.UnitTests.Task7;

public class BookingServiceStartRentalTests
{
    [Fact]
    [Trait("Rule", "BR-28")]
    public void StartRental_BookingDoesNotExist_ThrowsBookingNotFoundException()
    {
        // Arrange
        var service = BookingTestFactory.CreateService();

        // Act
        Action act = () => service.StartRental(99);

        // Assert
        act.Should().Throw<BookingNotFoundException>()
            .WithMessage("Booking #99 not found");
    }

    [Theory]
    [InlineData(BookingStatus.Active)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Cancelled)]
    [Trait("Rule", "BR-28")]
    public void StartRental_BookingIsNotReserved_ThrowsInvalidBookingStateException(BookingStatus status)
    {
        // Arrange
        var bookings = RepositoryWithBooking(BookingTestFactory.Now.DateTime.AddHours(1), status);
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        Action act = () => service.StartRental(1);

        // Assert
        act.Should().Throw<InvalidBookingStateException>()
            .WithMessage("Only reserved bookings can be started");
    }

    [Fact]
    [Trait("Rule", "BR-28")]
    public void StartRental_OneTickBeforeAllowedTime_ThrowsInvalidBookingStateException()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(1).AddTicks(1);
        var bookings = RepositoryWithBooking(pickup, BookingStatus.Reserved);
        var service = BookingTestFactory.CreateService(bookings: bookings);

        // Act
        Action act = () => service.StartRental(1);

        // Assert
        act.Should().Throw<InvalidBookingStateException>()
            .WithMessage("Too early to start this rental");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Rule", "BR-28")]
    public void StartRental_AtOrAfterAllowedTime_MarksBookingActive(int pickupOffsetMinutes)
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(1).AddMinutes(pickupOffsetMinutes);
        var bookings = RepositoryWithBooking(pickup, BookingStatus.Reserved);
        var audit = new SpyAuditLog();
        var service = BookingTestFactory.CreateService(bookings: bookings, audit: audit);

        // Act
        service.StartRental(1);

        // Assert
        bookings.Items.Single().Status.Should().Be(BookingStatus.Active);
        audit.Messages.Should().ContainSingle().Which.Should().Be("Booking DN-20261112-ST01 started");
    }

    static FakeBookingRepository RepositoryWithBooking(DateTime pickup, BookingStatus status)
    {
        var bookings = new FakeBookingRepository();
        bookings.Items.Add(new Booking
        {
            Id = 1,
            Reference = "DN-20261112-ST01",
            PickupAt = pickup,
            Status = status
        });
        return bookings;
    }
}
