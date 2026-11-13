## Task 2 — FIRST

### R1 — Test  Results

| Run | Duration (s) | Total | Passed | Failed | Failed Tests |
|---|---|---|---|---|---|
| 1 | 1.3459 | 3 | 2 | 1 | CreateBooking_ValidRequest_CreatesReservedBooking |
| 2 | 1.0638 | 3 | 2 | 1 | CreateBooking_ValidRequest_CreatesReservedBooking |
| 3 | 1.0564 | 3 | 2 | 1 | CreateBooking_ValidRequest_CreatesReservedBooking |
| 4 | 1.0523 | 3 | 2 | 1 | CreateBooking_ValidRequest_CreatesReservedBooking |
| 5 | 1.0637 | 3 | 2 | 1 | CreateBooking_ValidRequest_CreatesReservedBooking |


- شغلت الاختبارات خمس مرات

- كل مره ينجحو اتنين وواحد ما ينجح

- The Happy Path test failed with `BookingRejectedException: Vehicle is already booked for these dates`

- The Happy Path test passed when :  لما يشتغل لحاله بس لان هربنا من مشكله بتاعت ال static data بس لو اشتغل معاهم هيفشل تاني مش لان الاختبار غلط لا لان مفيش فصل للاسف بين ال tests

وكمان لما شغلته لحاله نجح وهادي النتيجه

DriveNow.UnitTests.Naive.BookingServiceNaiveTests.CreateBooking_ValidRequest_CreatesReservedBooking

   Source: BookingServiceNaiveTests.cs line 22

   Duration: 2.2 sec

   طبعا هنا التأخير السبب كان انتظار sms




- The Platinum test بياخد اكتر من 500ms في كل تشغيل

## R2 — FIRST 

| Principle | Meets FIRST? | My explanation / verdict |
|---|---|---|
| **F — Fast** | Partially | الاختبار بيعتمد على `SmsSender` وفيه `Thread.Sleep(500)` وده بيخلي الاختبار بطي بدون داعي صحيح كل الاختبارات اشتغلت في اقل من 10 ثواني لكن المشكلة ان مش قادر اعزل الـ SMS بسبب التصميم الحالي |
| **I — Independent** | No | للاسف الاختبارات بتعتمد على `Static Data` مشتركة وكمان انا استخدمت نفس السيارة في Happy Path وPlatinum بفترات متداخله  الاختبارات بقت بتاثر على بعض والمشكلة هنا في تصميم الاختبارات وعدم عزل البيانات وكمان تصميم الـ Repository اللي بيعتمد على `Static State` |
| **R — Repeatable** | Not Guaranteed | انا كنت متوقع ان مشكله الدفع العشوائي تظهر معايا لكن ده ما حصلش لان Platinum مش بيستدعي الدفع وHappy Path بيفشل قبل ما يوصل ليه  ما اقدرش اقول ان الاختبارات فشلت بشكل عشوائي لكن برضه مش ضامن انها دايما هتطلع بنفس النتيجه لان عندنا `Random` واعتمادات مش قادر اتحكم فيها بسبب عدم وجود `Dependency Injection` |
| **S — Self-validating** | Partially | الاختبارات بتقدر تحدد النجاح والفشل بنفسها باستخدام `FluentAssertions` لكن في Platinum انا قدرت اتاكد ان `Deposit = 0` وما قدرتش اتاكد ان `PaymentGateway.Charge()` ما تمش استدعاؤها يعني `Self-validating` متحقق في الحاجات اللي اختبرتها لكن لسه عندي نقص في `Test Coverage` بسبب التصميم |
| **T — Timely** | No | للاسف الاختبارات اتكتبت بعد ما النظام اتبنى وده خلى المشاكل تتراكم وبقى صعب نختبر الـ Service بالشكل المطلوب لو الاختبارات كانت بتتكتب مع تطوير كل Feature كنا هنكتشف مشكلة الاعتمادات من البداية ونستخدم `Dependency Injection` بدل ما نوصل للتصميم الحالي |

---



## R3 — Production Code Lines

| Principle | File | Line | Problem |
|---|---|---|---|
| **F — Fast** | `SmsSender.cs` | 7 | `Thread.Sleep(500)` بياخر الاختبار |
| **F — Fast** | `PaymentGateway.cs` | 20 | `Thread.Sleep(1500)` بياخر الاختبارات اللي بتوصل للدفع |
| **I — Independent** | `InMemoryDatabase.cs` | 12 | `public static List<Booking> Bookings = [];` بتخلي البيانات مشتركه بين الاختبارات |
| **I — Independent** | `BookingRepository.cs` | 17 | `InMemoryDatabase.Bookings.Add(booking)` بيضيف الحجوزات لنفس القائمه المشتركه |
| **I — Independent** | `BookingService.cs` | 37–39 | شرط التداخل بيرفض Happy Path بسبب الحجز اللي اتخزن من اختبار تاني |
| **R — Repeatable** | `PaymentGateway.cs` | 21 | `new Random().Next(10) == 0` ممكن يسبب فشل عشوائي للدفع  والنقطه هادي ظهرت معايا لما شغلت ال test لبيفشل لحاله|
| **R — Repeatable** | `BookingService.cs` | 58, 150–153 | `RandomCode()` بيولد Booking Reference مختلفة في كل تشغيل |
| **S — Self-validating** | `BookingService.cs` | 19 | `_payments = new()` بتمنعني من حقن Mock او Spy عشان اتاكد ان الدفع ما تمش استدعاؤه |
| **T — Timely** | `BookingService.cs` | 14–21 | الـ Service ينشا Dependencies بنفسها وده صعب اختبارها بعد ما النظام اتبنى |



وطبعا ده الفشل لظهر لما شغلت ال test اكتر من مره لحاله

 DriveNow.UnitTests.Naive.BookingServiceNaiveTests.CreateBooking_ValidRequest_CreatesReservedBooking

   Source: BookingServiceNaiveTests.cs line 16

   Duration: 1.5 sec

  Message: 

DriveNow.Infrastructure.External.PaymentFailedException : Gateway timeout

  Stack Trace: 

PaymentGateway.PauseAndMaybeFail() line 22

PaymentGateway.Charge(Int32 customerId, Decimal amount, String description) line 7

BookingService.CreateBooking(BookingRequest request) line 63

BookingServiceNaiveTests.CreateBooking_ValidRequest_CreatesReservedBooking() line 33

MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)


