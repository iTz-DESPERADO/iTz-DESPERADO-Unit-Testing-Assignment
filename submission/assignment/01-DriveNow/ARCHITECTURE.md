# Task 3 - Architecture Tests

Name: Gamal Khaled Mohamed

Group / ID: G4

## Result

15 اختبار : 11 نجحوا و4 فشلوا الفشل هنا كشف مخالفات 

| Rule | Result |
|---|---|
| R1 — Domain dependencies | نجح ومفيش اعتماد على مشروع DriveNow تاني |
| R2 — Application dependencies | فشل بسبب الاعتماد على Infrastructure |
| R3 — Infrastructure dependencies | نجح ومفيش اعتماد على ConsoleApp |
| R4 — Production naming | فشل اسم Repository واسم ومكان Service ومكان interfaces باقي الفحوص نجحت |
| R5 — Test naming | اسماء كلاسات الاختبارات ودوال Fact وTheory في UnitTests نجحت |

## المخالفات

| Rule |  reference | Why it matters for testing |
|---|---|---|
| R2 | DriveNow.Application → DriveNow.Infrastructure | بيربط منطق التطبيق بتنفيذ التخزين والخدمات الخارجيه وبيصعب عزل الاختبار |
| R2 | DriveNow.Application.Services.BookingService | بيعتمد على Infrastructure بدل اعتماديات اقدر ابدلها في الاختبار |
| R2 | DriveNow.Application.Services.PricingService | مرتبط بتنفيذ الوصول للبيانات فبيصعب التحكم في بيانات الاختبار |
| R2 | DriveNow.Application.Services.ReturnService | الاعتماد على التخزين والخدمات الخارجيه بيصعب اختبار منطق الارجاع لوحده |
| R2 | DriveNow.Application.Bookings.CancellationService | نفس مشكله الاعتماد على Infrastructure في اختبار الالغاء |
| R4 — Repository name | DriveNow.Infrastructure.Repositories.CustomerRepo | الاسم مش بينتهي بـ Repository وده بيخالف الفحص التلقائي للتسميه |
| R4 — Service name | DriveNow.Domain.Services.RentalMath | الاسم مش بينتهي بـ Service زي ما حددت في NAMING.md |
| R4 — Service namespace | DriveNow.Application.Receipts.ReceiptService | مش موجود في Services فممكن يفوت فحص بيعتمد على مكان الخدمات بس |
| R4 — Service namespace | DriveNow.Application.Bookings.CancellationService | موجود في Bookings بدل Services وده بيخالف تنظيم الخدمات المتفق عليه |
| R4 — Interface namespace | DriveNow.Application.Receipts.ICurrencyFormatter | المطلوب Application.Interfaces علشان عقود الاعتماديات تبقى واضحه وسهل نلاقيها |
| R4 — Interface namespace | DriveNow.Application.Receipts.ILineItemFactory | نفس مخالفه مكان واجهات Application |
| R4 — Interface namespace | DriveNow.Application.Receipts.IReceiptFormatter | نفس مخالفه مكان واجهات Application |
| R4 — Interface namespace | DriveNow.Application.Receipts.ITaxCalculator | نفس مخالفه مكان واجهات Application |



## Test output



```text
[xUnit.net 00:00:00.21]     DriveNow.ArchitectureTests.ProductionNamingRulesTests.ProductionServices_NamesAndNamespaces_FollowConvention [FAIL]
[xUnit.net 00:00:00.21]     DriveNow.ArchitectureTests.DependencyRulesTests.Application_Dependencies_IncludeOnlyDomain [FAIL]
[xUnit.net 00:00:00.21]     DriveNow.ArchitectureTests.ProductionNamingRulesTests.ProductionRepositories_NamesAndNamespaces_FollowConvention [FAIL]
[xUnit.net 00:00:00.22]     DriveNow.ArchitectureTests.ProductionNamingRulesTests.ProductionInterfaces_Namespaces_MatchOwningLayer [FAIL]
   Expected errors.Distinct() to be empty because service classes must end in Service and live under Services. Violations: DriveNow.Domain.Services.RentalMath; DriveNow.Application.Receipts.ReceiptService; DriveNow.Application.Bookings.CancellationService, but found at least one item {"DriveNow.Domain.Services.RentalMath"}.
   Expected assemblyReferences to be empty because Application can reference Domain, but not Infrastructure or ConsoleApp, but found at least one item {"DriveNow.Infrastructure"}.
Expected result.IsSuccessful to be True because R2 offending types: DriveNow.Application.Services.BookingService, DriveNow.Application.Services.PricingService, DriveNow.Application.Services.ReturnService, DriveNow.Application.Bookings.CancellationService, but found False.
   Expected errors.Distinct() to be empty because repository classes must end in Repository and live under Repositories. Violations: DriveNow.Infrastructure.Repositories.CustomerRepo, but found at least one item {"DriveNow.Infrastructure.Repositories.CustomerRepo"}.
   Expected errors to be empty because Domain and Application interfaces must be in their Interfaces namespace. Violations: DriveNow.Application.Receipts.ICurrencyFormatter (expected DriveNow.Application.Interfaces); DriveNow.Application.Receipts.ILineItemFactory (expected DriveNow.Application.Interfaces); DriveNow.Application.Receipts.IReceiptFormatter (expected DriveNow.Application.Interfaces); DriveNow.Application.Receipts.ITaxCalculator (expected DriveNow.Application.Interfaces), but found at least one item {"DriveNow.Application.Receipts.ICurrencyFormatter (expected DriveNow.Application.Interfaces)"}.
Failed!  - Failed:     4, Passed:    11, Skipped:     0, Total:    15, Duration: 89 ms - DriveNow.ArchitectureTests.dll (net10.0)
```


## Task 4 — Booking reference generator

حطيت IBookingReferenceGenerator في DriveNow.Domain.Interfaces لانه عقد لتوليد مرجع الحجز والتنفيذ العشوائي في DriveNow.Infrastructure.Services كده Application وInfrastructure بيعتمدوا على عقد في Domain من غير ما Domain يعتمد عليهم
 ConsoleApp بيربط التنفيذ بخدمة الحجز وفي الاختبار اقدر امرر مولد بيرجع مرجع ثابت

## Task 4 — Promo code repository

حطيت IPromoCodeRepository في DriveNow.Domain.Interfaces لانه عقد للوصول لكيان PromoCode الموجود في Domain

 التنفيذ فضل في Infrastructure.Repositories وPricingService بقى يعتمد على الواجهه بدل التنفيذ ConsoleApp بيمرر المستودع للخدمه كده التسعير ما بقاش مرتبط بـ Infrastructure 

كملت استخدام IPromoCodeRepository في BookingService وCancellationService كمان ConsoleApp بيمرر نفس المستودع للتسعير والحجز والالغاء والخدمتين بيعتمدوا على عقد Domain بدل نوع مستودع الخصم في Infrastructure

## Task 4 — Booking repository

حطيت IBookingRepository في DriveNow.Domain.Interfaces لانه عقد تخزين واسترجاع كيان Booking التنفيذ فضل في Infrastructure.Repositories وخدمات الحجز والارجاع والالغاء بتستقبل الواجهه من ConsoleApp كده اقدر ابدل مستودع الحجوزات في الاختبار من غير اعتماد على تنفيذه الحقيقي

## Task 4 — Vehicle repository

حطيت IVehicleRepository في DriveNow.Domain.Interfaces لانه عقد للوصول لكيان Vehicle وتحديثه التنفيذ فضل في Infrastructure.Repositories وخدمات الحجز والارجاع والالغاء بتستقبل الواجهه من ConsoleApp كده اقدر ابدل بيانات السيارات في الاختبار من غير تغيير الخدمات

## Task 4 — Customer repository

حطيت ICustomerRepository في DriveNow.Domain.Interfaces لانه عقد للوصول لكيان Customer وتحديثه غيرت اسم CustomerRepo لـ CustomerRepository علشان يطابق NAMING.md ومررت الواجهه لخدمات الحجز والارجاع والالغاء من ConsoleApp الخدمات دي ما بقتش تستخدم انواع repositories من Infrastructure لكن باقي اعتماديات الخدمات الخارجيه لسه موجوده

## Task 4 — Payment gateway

حطيت IPaymentGateway في DriveNow.Domain.Interfaces كعقد لعمليات التحصيل والاسترداد من غير تفاصيل بوابة الدفع. Application وInfrastructure بيعتمدوا على العقد في Domain، والتنفيذ فضل في Infrastructure.External. ConsoleApp بيمرره لخدمات الحجز والارجاع والالغاء، وفي الاختبار اقدر اتحكم في نجاح الدفع وفشله بدون انتظار او دفع حقيقي. باقي الاعتماديات الخارجيه لسه هتتفصل.

## Task 4 — SMS sender

حطيت ISmsSender في DriveNow.Domain.Interfaces كعقد بسيط لارسال الرسائل بدون تفاصيل التنفيذ Application وInfrastructure بيعتمدوا على العقد والتنفيذ فضل في Infrastructure.External

 ConsoleApp بيمرره للحجز والالغاء وفي الاختبار اقدر اتحقق من رقم العميل ونص الرساله من غير تشغيل SmsSender الحقيقي

## Task 4 — Email sender

حطيت IEmailSender في DriveNow.Domain.Interfaces كعقد لارسال البريد بدون تفاصيل الملفات التنفيذ فضل في Infrastructure.External وConsoleApp بيمرره للحجز والارجاع والالغاء الخدمات ما بقتش تستخدم انواع Infrastructure.External مباشرة وفي الاختبار اقدر افحص المستلم والعنوان والمحتوى بدون كتابة ملفات

## Task 4 — Audit log

حطيت IAuditLog في DriveNow.Domain.Interfaces كعقد لتسجيل رساله بدون تفاصيل الملف التنفيذ فضل في Infrastructure.Logging وبقى كائن بدل static وConsoleApp بيمرره للخدمات بعد فصل السجل ما بقاش فيه استخدام لـ Infrastructure في Application شلت مرجع المشروع منه وبقى بيعتمد على Domain بس

## Task 4 — Instance data store

حولت InMemoryDatabase لكائن مستقل ومررته للمستودعات الاربعه المخزن فضل في Infrastructure.Persistence لانه تفاصيل تخزين وApplication لسه بيتعامل مع واجهات المستودعات فقط ConsoleApp بينشئ مخزن واحد بساعة النظام وبيشاركه بين المستودعات علشان سلوك البرنامج يفضل زي الاول القوائم وعداد الحجوزات بقوا مستقلين لكل كائن وتواريخ البيانات الاوليه بتعتمد على TimeProvider

## Task 4 — Text file writer

حطيت ITextFileWriter في Infrastructure.Interfaces لانه تفاصيل ملفات تخص تنفيذ البريد والسجل فقط ومش مطلوب في Domain او Application. TextFileWriter بينفذ عمليات الملفات الحقيقيه وEmailSender وAuditLog بيستقبلوا الواجهه والمسار ConsoleApp بيمرر نفس مسارات outbox وaudit.log القديمه وفي الاختبار اقدر استخدم بديل يحفظ المحتوى في الذاكره

## Task 4 — Rental math service name

غيرت اسم RentalMath وملفه لـ RentalMathService علشان اسم الخدمه ينتهي بـ Service حسب NAMING.md حدثت استخداماته في التطبيق والاختبارات بدون تغيير الحسابات او اسماء الاختبارات وحالاتها 

## Task 4 — Service namespaces

نقلت CancellationService من Bookings وReceiptService من Receipts لمجلد وnamespace اسمه DriveNow.Application.Services حسب NAMING.md حدثت using في ConsoleApp واختبارات Legacy بدون تغيير منطق الخدمات او assertions الاختبارات وصف المخالفات القديم فوق بيوثق حاله الكود قبل التعديل

## Task 4 — Receipt interfaces

نقلت ICurrencyFormatter وILineItemFactory وIReceiptFormatter وITaxCalculator لـ DriveNow.Application.Interfaces علشان يطابقوا NAMING.md دي عقود خاصه بتجهيز وعرض الفاتوره في Application والتنفيذ فضل في Receipts حدثت استخداماتهم في الخدمات والتنفيذ واختبارات Legacy بدون تغيير سلوك الفاتوره او assertions الاختبارات

بعد نقل الواجهات شغلت اختبارات المعماريه كلها: 15 نجحوا و0 فشلوا في 197ms اختبارات Legacy الخمسه نجحت كمان


## Task 4 — Reference generator randomness

BookingReferenceGeneratorService بقى يستقبل Random من ConsoleApp بدل ما ينشئه داخل Generate صيغه DN والتاريخ والاربع حروف او ارقام فضلت زي ما هي المولد بيستخدم نفس كائن Random بين الاستدعاءات بدل انشاء كائن كل مره وفي الاختبار اقدر اثبت مصدر العشوائيه بدون واجهه اضافيه
