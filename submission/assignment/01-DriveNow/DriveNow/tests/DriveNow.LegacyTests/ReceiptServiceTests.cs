using DriveNow.Application.Receipts;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;
using DriveNow.Domain.ValueObjects;
using VerifyXunit;

namespace DriveNow.LegacyTests;

public class ReceiptServiceTests
{
    [Fact]
    public async Task Build_CompletedRentalWithLateFeeAndDamage_MatchesApprovedReceipt()
    {
        // Arrange
        var booking = new Booking
        {
            Reference = "DN-2026-001",
            Status = BookingStatus.Completed,
            Price = new PriceBreakdown
            {
                BaseAmount = 1200m,
                WeekendSurcharge = 120m,
                YoungDriverSurcharge = 150m,
                DurationDiscount = 60m,
                LoyaltyDiscount = 90m,
                PromoDiscount = 30m,
                InsuranceCost = 240m,
                AdditionalDriversCost = 50m
            }
        };
        var report = new ReturnReport
        {
            LateFee = 240m,
            MileageFee = 15.75m,
            FuelFee = 200m,
            DamageCharge = 1000m
        };
        var receiptService = new ReceiptService(
            new LineItemFactory(),
            new TaxCalculator(),
            new CurrencyFormatter(),
            new ReceiptFormatter());

        // Act
        var receipt = receiptService.Build(booking, report);

        // Assert
        Console.WriteLine(receipt);
        await Verifier.Verify(receipt);
    }
}
