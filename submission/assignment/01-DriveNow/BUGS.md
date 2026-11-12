# Bugs

Name: Gamal Khaled Mohamed
Group / ID: G4

| # | Rule | Failing test | Expected | Actual | Fix commit |
|---|---|---|---|---|---|
| 1 | BR-03 / BR-30 | `CalculateRentalDays_WithinGracePeriod_ReturnsOneDay` - 25 hours; `CalculateRentalDays_GracePeriodAfterTwoDays_ReturnsCorrectDays` - 49 hours; `Calculate_RentalDuration_ReturnsCorrectDaysAndBaseAmount` - 25 hours | 1 day; 2 days; 1 day and 600 base amount | 2 days; 3 days; 2 days | `ee36768` |
| 2 | BR-33 | `Calculate_RentalDays_ReturnsCorrectDurationDiscount` - 7 days | 420 | 0 | `7cbd404` |
| 3 | BR-37 | `Calculate_DiscountCap_ReturnsCorrectPromoDiscount` - 7 days, Gold, Promo 15% | 210 | 630 | `97ab4c1` |
| 4 | BR-39 | `Calculate_WorkedExample_ReturnsWholePriceBreakdown` - Example B, Silver, extra driver | 700 | 0 | `7f5cbc9` |
| 5 | BR-16 | `CreateBooking_LuxuryDriverIsExactlyTwentyFiveWithThreeLicenseYears_CreatesBooking` | booking is created | `BookingRejectedException` | — |
