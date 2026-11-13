using System.Threading.Tasks;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;
using VerifyXunit;

namespace DriveNow.UnitTests.Task6;

public class BookingServiceSnapshotTests
{
    [Fact]
    [Trait("Rule", "BR-24")]
    public async Task CreateBooking_FixedClockAndReference_MatchesApprovedSnapshot()
    {
        // Arrange
        var bookings = new FakeBookingRepository();
        var service = BookingTestFactory.CreateService(
            customer: BookingTestFactory.Customer(),
            vehicle: BookingTestFactory.Vehicle(category: VehicleCategory.Economy),
            bookings: bookings,
            reference: "DN-20261112-SN01");

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        await Verifier.Verify(booking);
    }
}
