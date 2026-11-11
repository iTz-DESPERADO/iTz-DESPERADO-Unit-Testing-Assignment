using DriveNow.Application.Models;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.Interfaces;

namespace DriveNow.UnitTests.Task6;

internal sealed class FixedBookingTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}

internal sealed class FixedReferenceGenerator(string reference) : IBookingReferenceGenerator
{
    public string Generate(DateTime pickupAt) => reference;
}

internal sealed class SpyReferenceGenerator(string reference) : IBookingReferenceGenerator
{
    public List<DateTime> PickupDates { get; } = [];
    public string Generate(DateTime pickupAt)
    {
        PickupDates.Add(pickupAt);
        return reference;
    }
}

internal sealed class FakeBookingRepository : IBookingRepository
{
    public List<Booking> Items { get; } = [];

    public Booking? GetById(int id) => Items.SingleOrDefault(booking => booking.Id == id);
    public List<Booking> GetAll() => Items.ToList();
    public void Add(Booking booking) => Items.Add(booking);
    public void Update(Booking booking) { }
    public List<Booking> GetOpenBookingsForVehicle(int vehicleId) =>
        Items.Where(booking => booking.VehicleId == vehicleId && booking.Status is BookingStatus.Reserved or BookingStatus.Active).ToList();
    public int CountOpenBookingsForCustomer(int customerId) =>
        Items.Count(booking => booking.CustomerId == customerId && booking.Status is BookingStatus.Reserved or BookingStatus.Active);
}

internal sealed class StubCustomerRepository(Customer? customer) : ICustomerRepository
{
    public Customer? GetById(int id) => customer;
    public List<Customer> GetAll() => customer is null ? [] : [customer];
    public void Update(Customer customer) { }
}

internal sealed class StubVehicleRepository(Vehicle? vehicle) : IVehicleRepository
{
    public Vehicle? GetById(int id) => vehicle;
    public List<Vehicle> GetAll() => vehicle is null ? [] : [vehicle];
    public void Update(Vehicle vehicle) { }
}

internal sealed class StubPromoRepository(PromoCode? promo = null) : IPromoCodeRepository
{
    public PromoCode? Promo { get; } = promo;
    public int UpdateCalls { get; private set; }

    public PromoCode? GetByCode(string code) => Promo;
    public void Update(PromoCode promo) => UpdateCalls++;
}

internal sealed class SpySmsSender : ISmsSender
{
    public List<(string Phone, string Text)> Calls { get; } = [];
    public void Send(string phone, string text) => Calls.Add((phone, text));
}

internal sealed class SpyEmailSender : IEmailSender
{
    public List<(string To, string Subject, string Body)> Calls { get; } = [];
    public void Send(string to, string subject, string body) => Calls.Add((to, subject, body));
}

internal sealed class HandWrittenPaymentMock : IPaymentGateway
{
    public List<(int CustomerId, decimal Amount, string Description)> Charges { get; } = [];
    public string Charge(int customerId, decimal amount, string description)
    {
        Charges.Add((customerId, amount, description));
        return "payment-1";
    }

    public void Refund(int customerId, decimal amount, string description) { }
}

internal sealed class DummyAuditLog : IAuditLog
{
    public void Write(string message) { }
}

internal sealed class SpyAuditLog : IAuditLog
{
    public List<string> Messages { get; } = [];
    public void Write(string message) => Messages.Add(message);
}

internal static class BookingTestFactory
{
    public static readonly DateTimeOffset Now = new(2026, 11, 12, 9, 0, 0, TimeSpan.Zero);

    public static Customer Customer(
        int id = 10,
        DateTime? dateOfBirth = null,
        DateTime? licenseIssued = null,
        LoyaltyTier tier = LoyaltyTier.None,
        bool blacklisted = false) => new()
    {
        Id = id,
        FullName = "Gamal Khaled",
        Email = "gamal@example.com",
        Phone = "5550100",
        DateOfBirth = dateOfBirth ?? new DateTime(1990, 1, 1),
        LicenseIssueDate = licenseIssued ?? new DateTime(2020, 1, 1),
        LoyaltyTier = tier,
        IsBlacklisted = blacklisted
    };

    public static Vehicle Vehicle(
        int id = 20,
        VehicleCategory category = VehicleCategory.Economy,
        bool available = true,
        int mileage = 1234) => new()
    {
        Id = id,
        Category = category,
        DailyRate = 600m,
        IsAvailable = available,
        Mileage = mileage,
        PlateNumber = "DN-1",
        Model = "Test car"
    };

    public static BookingRequest Request(
        DateTime? pickup = null,
        DateTime? returnAt = null,
        int additionalDrivers = 0,
        InsurancePlan insurance = InsurancePlan.Basic,
        string? promoCode = null) => new()
    {
        CustomerId = 10,
        VehicleId = 20,
        PickupAt = pickup ?? Now.DateTime.AddHours(2),
        ReturnAt = returnAt ?? Now.DateTime.AddHours(26),
        AdditionalDrivers = additionalDrivers,
        InsurancePlan = insurance,
        PromoCode = promoCode
    };

    public static BookingService CreateService(
        Customer? customer = null,
        Vehicle? vehicle = null,
        bool customerExists = true,
        bool vehicleExists = true,
        FakeBookingRepository? bookings = null,
        StubPromoRepository? promos = null,
        IPaymentGateway? payments = null,
        ISmsSender? sms = null,
        IEmailSender? email = null,
        IAuditLog? audit = null,
        IBookingReferenceGenerator? referenceGenerator = null,
        string reference = "DN-20261112-AB12") => new(
        new PricingService(new FixedBookingTimeProvider(Now), promos ?? new StubPromoRepository()),
        new FixedBookingTimeProvider(Now),
        referenceGenerator ?? new FixedReferenceGenerator(reference),
        promos ?? new StubPromoRepository(),
        bookings ?? new FakeBookingRepository(),
        new StubVehicleRepository(vehicleExists ? vehicle ?? Vehicle() : null),
        new StubCustomerRepository(customerExists ? customer ?? Customer() : null),
        payments ?? new HandWrittenPaymentMock(),
        sms ?? new SpySmsSender(),
        email ?? new SpyEmailSender(),
        audit ?? new DummyAuditLog());
}
