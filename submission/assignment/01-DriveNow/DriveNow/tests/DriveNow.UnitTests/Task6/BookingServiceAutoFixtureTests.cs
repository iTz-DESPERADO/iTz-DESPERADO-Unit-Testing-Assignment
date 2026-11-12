using AutoFixture;
using AutoFixture.Xunit2;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Exceptions;
using DriveNow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace DriveNow.UnitTests.Task6;

public class BookingServiceAutoFixtureTests
{
    [Theory, AutoMoqData]
    [Trait("Rule", "BR-26")]
    public void CreateBooking_AutoMoqBuildsService_PlatinumGatewayIsNeverCalled(
        [Frozen] Mock<TimeProvider> clock,
        [Frozen] Mock<ICustomerRepository> customers,
        [Frozen] Mock<IVehicleRepository> vehicles,
        [Frozen] Mock<IBookingRepository> bookings,
        [Frozen] Mock<IBookingReferenceGenerator> references,
        [Frozen] Mock<IPaymentGateway> payments,
        BookingService service)
    {
        // Arrange
        ConfigureClock(clock);
        customers.Setup(repository => repository.GetById(10))
            .Returns(BookingTestFactory.Customer(tier: LoyaltyTier.Platinum));
        vehicles.Setup(repository => repository.GetById(20)).Returns(BookingTestFactory.Vehicle());
        bookings.Setup(repository => repository.GetOpenBookingsForVehicle(20)).Returns([]);
        bookings.Setup(repository => repository.CountOpenBookingsForCustomer(10)).Returns(0);
        references.Setup(generator => generator.Generate(It.IsAny<DateTime>())).Returns("DN-20261112-AF01");

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(
            pickup: BookingTestFactory.Now.DateTime.AddHours(3),
            returnAt: BookingTestFactory.Now.DateTime.AddHours(27)));

        // Assert
        booking.Deposit.Should().Be(0m);
        payments.Verify(gateway => gateway.Charge(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_AnonymousCustomer_IsNotAdult()
    {
        // Arrange
        var fixture = new Fixture();
        var anonymousCustomer = fixture.Create<Customer>();
        var pickup = BookingTestFactory.Now.DateTime.AddHours(3);
        var age = DriveNow.Domain.Services.RentalMathService.CalculateAge(anonymousCustomer.DateOfBirth, pickup);

        // Act
        var isValidAdult = age >= 21;

        // Assert
        isValidAdult.Should().BeFalse("the anonymous date of birth is not a valid booking customer");
    }

    [Fact]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_ValidCustomerCustomization_CreatesAdultCustomer()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(new ValidAdultCustomerCustomization());
        var customer = fixture.Create<Customer>();
        var pickup = BookingTestFactory.Now.DateTime.AddHours(3);

        // Act
        var age = DriveNow.Domain.Services.RentalMathService.CalculateAge(customer.DateOfBirth, pickup);

        // Assert
        age.Should().BeGreaterThanOrEqualTo(21);
        DriveNow.Domain.Services.RentalMathService.CalculateLicenseYears(customer.LicenseIssueDate, pickup)
            .Should().BeGreaterThanOrEqualTo(1);
    }

    [Theory, InlineAutoMoqData(20)]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_InlineAutoDataAgeTwenty_RejectsBooking(
        int age,
        [Frozen] Mock<TimeProvider> clock,
        [Frozen] Mock<ICustomerRepository> customers,
        [Frozen] Mock<IVehicleRepository> vehicles,
        [Frozen] Mock<IBookingRepository> bookings,
        BookingService service)
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(3);
        ConfigureClock(clock);
        ConfigureValidBooking(customers, vehicles, bookings, pickup, age);

        // Act
        Action act = () => service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        act.Should().Throw<BookingRejectedException>()
            .WithMessage("Driver must be at least 21");
    }

    [Theory, InlineAutoMoqData(21), InlineAutoMoqData(25)]
    [Trait("Rule", "BR-12")]
    public void CreateBooking_InlineAutoDataAdultAges_CreateBooking(
        int age,
        [Frozen] Mock<TimeProvider> clock,
        [Frozen] Mock<ICustomerRepository> customers,
        [Frozen] Mock<IVehicleRepository> vehicles,
        [Frozen] Mock<IBookingRepository> bookings,
        BookingService service)
    {
        // Arrange
        var pickup = BookingTestFactory.Now.DateTime.AddHours(3);
        ConfigureClock(clock);
        ConfigureValidBooking(customers, vehicles, bookings, pickup, age);

        // Act
        var booking = service.CreateBooking(BookingTestFactory.Request(pickup: pickup));

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
    }

    static void ConfigureValidBooking(
        Mock<ICustomerRepository> customers,
        Mock<IVehicleRepository> vehicles,
        Mock<IBookingRepository> bookings,
        DateTime pickup,
        int age)
    {
        customers.Setup(repository => repository.GetById(10)).Returns(BookingTestFactory.Customer(
            dateOfBirth: pickup.AddYears(-age),
            licenseIssued: pickup.AddYears(-3)));
        vehicles.Setup(repository => repository.GetById(20)).Returns(BookingTestFactory.Vehicle());
        bookings.Setup(repository => repository.GetOpenBookingsForVehicle(20)).Returns([]);
        bookings.Setup(repository => repository.CountOpenBookingsForCustomer(10)).Returns(0);
    }

    static void ConfigureClock(Mock<TimeProvider> clock)
    {
        clock.Setup(provider => provider.GetUtcNow()).Returns(BookingTestFactory.Now);
        clock.SetupGet(provider => provider.LocalTimeZone).Returns(TimeZoneInfo.Utc);
    }
}
