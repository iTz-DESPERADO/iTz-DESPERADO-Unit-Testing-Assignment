# TDD Log

Name: Gamal Khaled Mohamed

Group / ID: G4


| Commit | Test name | Why it failed (real failure) | What changed in Green | Refactor extracted |
|---|---|---|---|---|
| `red(S1)` / `green(S1)` | `Calculate_FreePeriod_ReturnsZero` | `ParkingFeeCalculator` was not implemented, so the test could not compile | Created `ParkingFeeCalculator` and returned `0m` for the free parking test | None |
| `red(S1)` / `green(S1)` | `Calculate_JustAfterFifteenMinutes_IsNotFree` | Expected fee to be greater than `0m` but found `0m` | Added the 15-minute check and returned `1m` after the free period | None |
