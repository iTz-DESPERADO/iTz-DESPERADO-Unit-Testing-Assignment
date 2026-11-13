# Code Coverage Report

Name: Gamal Khaled Mohamed
Group / ID: G4

## Baseline Coverage

Measured before Task 1 using the 5 legacy tests

| Project | Line Coverage | Branch Coverage |
|---|---:|---:|
| DriveNow.Application | 7.80% | 3.22% |
| DriveNow.Domain | 3.96% | 0% |
| DriveNow.Infrastructure | 0% | 0% |

Tests: 5 passed, 0 failed

Command:

first i download package coverlet.collector then make this command

dotnet test --collect:"XPlat Code Coverage"

## After Task 1

قست التغطيه بعد اضافة اختبارات RentalMath وحالات مراجعة AI باستخدام الامر ده من مجلد DriveNow

```powershell
dotnet test DriveNow.sln --collect:"XPlat Code Coverage"
```

النتيجه دي مجمعه من تقريري LegacyTests وUnitTests مش من تقرير UnitTests لوحده. حسبت الاسطر المغطاه بدون تكرارها بين التقريرين والنسب مقربه لخانتين

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 1 | DriveNow.Application | 34 / 401 | 8.48% | 6 / 186 | 3.23% |
| Task 1 | DriveNow.Domain | 28 / 114 | 24.56% | 12 / 18 | 66.67% |
| Task 1 | DriveNow.Infrastructure | 0 / 218 | 0% | 0 / 20 | 0% |

تقرير LegacyTests غطى 4 اسطر في Domain وتقرير UnitTests غطى 24 سطر مختلفين فالمجموع 28 سطر. الفروع المغطاه في Domain كلها من اختبارات UnitTests

اخر تشغيل UnitTests اللي سجلته كان 37 حاله: 35 نجحت وحالتين فشلوا بسبب نفس خطأ BR-03 المسجل في BUGS.md. ملفات التغطيه لوحدها ما بتثبتش عدد الاختبارات الناجحه والفاشله في تشغيل الحل كامل


احتفظت بارقام Baseline زي ما سجلتها وقتها. لاحظت ان تقرير LegacyTests الحالي لوحده مختلف عنها: Application فيه 34/401 سطر وDomain فيه 4/114 سطر. سبب الفرق محتاج مراجعة تقرير Baseline الاصلي ومش هغير الارقام القديمه من غير دليل

## After Task 5

قست التغطية بعد اختبارات PricingService. شغلت UnitTests و LegacyTests كل واحد لوحده ثم دمجت تقريري Cobertura في ReportGenerator.

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 5 | DriveNow.Application | 125 / 422 | 29.62% | 53 / 184 | 28.80% |
| Task 5 | DriveNow.Domain | 65 / 114 | 57.02% | 12 / 18 | 66.67% |

Commands from `DriveNow`:

```powershell
dotnet test tests/DriveNow.UnitTests/DriveNow.UnitTests.csproj --collect:"XPlat Code Coverage"
dotnet test tests/DriveNow.LegacyTests/DriveNow.LegacyTests.csproj --collect:"XPlat Code Coverage"
```

UnitTests: 109 passed, 9 failed. LegacyTests: 5 passed, 0 failed الاخطاء الموجودة هي bugs معروفة ومسجلة في `BUGS.md` لكن تقرير التغطية يتولد حتى لو الاختبارات فشلت

`PricingService` لوحده: 84 / 85 سطر (98.8%) و 47 / 48 فرع (97.9%). هذا لا يعني انه صحيح: الاختبارات ما زالت كاشفة اخطاء BR-03 وBR-33 وBR-37 وBR-39

## After Task 6

قست التغطية بعد اختبارات BookingService وحالات مراجعة AI. شغلت UnitTests وLegacyTests ثم دمجت التقريرين في ReportGenerator.

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 6 | DriveNow.Application | 230 / 422 | 54.50% | 107 / 184 | 58.15% |
| Task 6 | DriveNow.Domain | 89 / 114 | 78.07% | 12 / 18 | 66.67% |

UnitTests في التقرير الكامل: 162 passed, 12 failed. LegacyTests: 5 passed, 0 failed. من 12 فشل، 3 تخص Task 6: BR-16 وعند طرفين من back-to-back في BR-21. الباقي bugs قديمة من Task 1 وTask 5 ومسجلة في `BUGS.md`.

`BookingService` لوحده: 87 / 97 سطر (89.7%) التغطية زادت لكن ده مش معناه ان BR-16 وBR-21 صح لان الحالات الحدية كشفت اخطاء موجودة في الكود

## After Task 7

قست التغطية بعد اختبارات pickup وreturn وcancellation وحالات مراجعة AI. شغلت UnitTests وLegacyTests ثم دمجت التقريرين في ReportGenerator.

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 7 | DriveNow.Application | 419 / 422 | 99.29% | 175 / 184 | 95.11% |
| Task 7 | DriveNow.Domain | 112 / 114 | 98.25% | 16 / 18 | 88.89% |

UnitTests في التقرير الكامل: 216 passed, 14 failed. LegacyTests: 5 passed, 0 failed. من 14 فشل، 2 تخص Task 7: BR-41 عند 240 دقيقة وBR-51 عند 48 ساعة. الباقي bugs قديمة ومسجلة في `BUGS.md`.

## After Task 8

قست التغطية بعد استبدال legacy mocks باختبار receipt حقيقي وVerify. شغلت UnitTests وLegacyTests ثم دمجت التقريرين في ReportGenerator.

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 8 | DriveNow.Application | 421 / 424 | 99.29% | 175 / 184 | 95.11% |
| Task 8 | DriveNow.Domain | 112 / 114 | 98.25% | 16 / 18 | 88.89% |

UnitTests في التقرير الكامل: 216 passed, 14 failed. LegacyTests: 1 passed, 0 failed. الـ14 failures هي bugs قديمة ومسجلة في `BUGS.md`.

`ReceiptService` وclasses الخاصة بالـreceipt وصلت 100%. الاختبار الجديد شغّل VAT والformatting والنص الكامل بدل ما يختبر calls للـmocks فقط.

## Task 9 - Coverage is not correctness

### R5 - 100% coverage مع وجود bug

`RentalMathService` وصل 100% line وbranch coverage مع ذلك اختبار BR-03 عند 25 ساعة بالضبط ما زال يفشل المتوقع يوم واحد لكن الناتج يومين كل سطر اتنفذ لكن الشرط استخدم `>= 60` بدل `> 60`. لذلك coverage يثبت ان الكود اتشغل لا يثبت ان النتيجة صح عند boundary

### R6 - branch اخترت ما اختبرها

في `LoyaltyService.UpgradeTier` ما اختبرت branch ترقية Platinum عند `CompletedRentals >= 30` تقرير Task 8 يبين ان line الشرط اتنفذ لكن true branch لم يتنفذ اخترت اتركه لان اختباراتنا الحالية في BR-46 تحتاج حالة 4 الى 5 وترقية Silver فقط وما عندي سيناريو في التاسكات الحالية يحتاج 30 rental. ما استبعدتش الكود من coverage علشان الرقم يبان اعلى الفرع ظاهر في التقرير كغير مغطى

