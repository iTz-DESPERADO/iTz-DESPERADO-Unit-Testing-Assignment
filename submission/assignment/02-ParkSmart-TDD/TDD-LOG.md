# TDD Log

Name: Gamal Khaled Mohamed

Group / ID: G4


| Commit | Test name | Why it failed (real failure) | What changed in Green | Refactor extracted |
|---|---|---|---|---|
| `red(S1)` / `green(S1)` | `Calculate_FreePeriod_ReturnsZero` | `ParkingFeeCalculator` was not implemented, so the test could not compile | Created `ParkingFeeCalculator` and returned `0m` for the free parking test | None |
| `red(S1)` / `green(S1)` | `Calculate_JustAfterFifteenMinutes_IsNotFree` | Expected fee to be greater than `0m` but found `0m` | Added the 15-minute check and returned `1m` after the free period | None |
| `red(S2)` / `green(S2)` | `Calculate_FirstStartedHour_ReturnsTen` | Expected fee to be `10m` but found `1m` | Changed the fee after the free period from `1m` to `10m` | None |
| `red(S2)` / `green(S2)` | `Calculate_JustBeyondFirstHour_ReturnsTwenty` | Expected fee to be `20m` but found `10m` | Added the branch after the first hour and returned `20m` | None |
| `red(S2)` / `green(S2)` | `Calculate_MultipleStartedHours_ReturnsCorrectFee` | Expected fee to be `40m` but found `20m` | Replaced the fixed fee with `Math.Ceiling` so each started hour costs `10m` | None |
| `red(S3)` / `green(S3)` | `Calculate_ExitBeforeEntry_ThrowsArgumentException` | Expected `ArgumentException`, but no exception was thrown | Added validation before calculating the duration | None |
| `red(S4)` / `green(S4)` | `Calculate_TenHours_ReturnsDailyCap` | Expected fee to be `80m` but found `100m` | Added the daily cap of `80m` | Extracted CalculateFee() |
| `red(S4)` / `green(S4)` | `Calculate_MultipleDays_ReturnsCorrectFee` | Expected `140m`, `160m`, and `170m` but found `80m` | Counted each full 24-hour block at `80m` and charged the remaining time | None |
| `red(S5)` / `green(S5)` | `Calculate_FirstStartedHourForMotoCycle_ReturnsFive` | Expected `5m` but found `10m` | Used the Motorcycle hourly rate of `5m` | None |
| `red(S5)` / `green(S5)` | `Calculate_TenHoursForMotorCycle_ReturnsDailyCap` | Expected `40m` but found `50m` | Used the Motorcycle daily cap of `40m` | None |
