using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace DriveNow.UnitTests.Task6;

public class BookingServiceMoqTests
{
    [Fact]
    [Trait("Rule", "BR-10")]
    public void CreateBooking_MoqCustomerRepositoryReturnsNull_ThrowsCustomerNotFoundException()
    {
        // Arrange
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(repository => repository.GetById(10)).Returns((Customer?)null);
        var service = CreateService(customers: customers.Object);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request());

        // Assert
        act.Should().Throw<CustomerNotFoundException>()
            .WithMessage("Customer #10 not found");
        customers.Verify(repository => repository.GetById(10), Times.Once);
    }

    [Fact]
    [Trait("Rule", "BR-21")]
    public void CreateBooking_MoqBookingRepositoryReturnsOverlap_RejectsWithoutSaving()
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(2);
        var bookings = new Mock<IBookingRepository>();
        bookings.Setup(repository => repository.GetOpenBookingsForVehicle(20)).Returns([
            new Booking
            {
                VehicleId = 20,
                PickupAt = pickup.AddHours(1),
                ReturnAt = pickup.AddHours(5),
                Status = BookingStatus.Reserved
            }
        ]);
        var service = CreateService(bookings: bookings.Object);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Vehicle is already booked for these dates");
        bookings.Verify(repository => repository.Add(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    [Trait("Rule", "BR-26")]
    public void CreateBooking_PlatinumCustomer_MoqPaymentGatewayIsNeverCalled()
    {
        // Arrange
        var payments = new Mock<IPaymentGateway>();
        var service = CreateService(
            customer: BookingTestFactory.Customer(tier: LoyaltyTier.Platinum),
            payments: payments.Object);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request());

        // Assert
        booking.Deposit.Should().Be(0m);
        payments.Verify(gateway => gateway.Charge(
            It.IsAny<int>(),
            It.IsAny<decimal>(),
            It.IsAny<string>()), Times.Never);
    }

    static BookingService CreateService(
        Customer? customer = null,
        ICustomerRepository? customers = null,
        IBookingRepository? bookings = null,
        IPaymentGateway? payments = null)
    {
        var promoRepository = new Mock<IPromoCodeRepository>();
        var vehicleRepository = new Mock<IVehicleRepository>();
        vehicleRepository.Setup(repository => repository.GetById(20)).Returns(BookingTestFactory.Vehicle());

        var customerRepository = customers ?? new Mock<ICustomerRepository>().Object;
        if (customers is null)
        {
            var configuredCustomers = Mock.Get(customerRepository);
            configuredCustomers.Setup(repository => repository.GetById(10)).Returns(customer ?? BookingTestFactory.Customer());
        }

        var bookingRepository = bookings ?? new Mock<IBookingRepository>().Object;
        if (bookings is null)
        {
            var configuredBookings = Mock.Get(bookingRepository);
            configuredBookings.Setup(repository => repository.GetOpenBookingsForVehicle(20)).Returns([]);
            configuredBookings.Setup(repository => repository.CountOpenBookingsForCustomer(10)).Returns(0);
        }

        return new BookingService(
            new PricingService(new FixedBookingTimeProvider(BookingTestFactory.Now), promoRepository.Object),
            new FixedBookingTimeProvider(BookingTestFactory.Now),
            new StubReferenceGenerator("DN-20261112-AB12"),
            promoRepository.Object,
            bookingRepository,
            vehicleRepository.Object,
            customerRepository,
            payments ?? new Mock<IPaymentGateway>().Object,
            new Mock<ISmsSender>().Object,
            new Mock<IEmailSender>().Object,
            new Mock<IAuditLog>().Object);
    }
}
