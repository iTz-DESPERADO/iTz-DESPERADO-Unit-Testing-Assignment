# Bugs

Name: Gamal Khaled Mohamed
Group / ID: G4

| # | Rule | Failing test | Expected | Actual | Fix commit |
|---|---|---|---|---|---|
| 1 | BR-03 / BR-30 | `CalculateRentalDays_WithinGracePeriod_ReturnsOneDay` - 25 hours; `CalculateRentalDays_GracePeriodAfterTwoDays_ReturnsCorrectDays` - 49 hours; `Calculate_RentalDuration_ReturnsCorrectDaysAndBaseAmount` - 25 hours | 1 day; 2 days; 1 day and 600 base amount | 2 days; 3 days; 2 days | `ee36768` |
| 2 | BR-33 | `Calculate_RentalDays_ReturnsCorrectDurationDiscount` - 7 days | 420 | 0 | — |
