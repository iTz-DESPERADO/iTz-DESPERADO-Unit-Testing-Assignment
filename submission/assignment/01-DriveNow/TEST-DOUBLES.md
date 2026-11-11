# Task 6 - BookingService Test Doubles

| Double | Kind | Test that uses it | Why this kind |
|---|---|---|---|
| `DummyAuditLog` | Dummy | أغلب اختبارات `BookingServiceRulesTests` | الـ audit log مش مهم في الاختبار ده لكن الـ constructor محتاجه |
| `StubCustomerRepository` و `StubVehicleRepository` | Stub | اختبارات BR-10 ل BR-25 | برجع Customer او Vehicle معروف علشان اختبر قرار واحد من غير بيانات حقيقية |
| `SpySmsSender`, `SpyEmailSender`, `SpyAuditLog` | Spy | `CreateBooking_ValidPromo_StoresBookingAndSendsConfirmation` | محتاج اتاكد اي رسالة اتبعتت بعد الحجز |
| `HandWrittenPaymentMock` | Mock | `CreateBooking_NonPlatinumCustomer_ChargesCorrectDeposit` و `CreateBooking_PlatinumCustomer_DoesNotChargeDeposit` | مهم اتاكد هل Charge اتنادت وباي مبلغ |
| `FakeBookingRepository` | Fake | اختبارات BR-21 وBR-22 وBR-27 | فيه List بسيطة تمثل الحجز المخزن وتسمح اشوف overlap وعدد الحجوزات المفتوحة |

## Moq

اعدت 3 اختبارات باستخدام Moq: customer مش موجود overlap وPlatinum بدون deposit

الـ hand-written Fake او Spy اوضح لما يكون عندي حالة صغيرة او عايز اشوف البيانات اللي اتخزنت او الرسالة اللي اتبعتت Moq اسهل لما يكون المطلوب interaction واحد زي `Verify` او `Times.Never`

Moq ممكن يتكسر اسرع لو غيرت signature او غيرت ترتيب الcalls لان Setup وVerify مربوطين بالتفاصيل دي الـ Fake بيتكسر غالبا لما السلوك اللي بنختبره نفسه يتغير

## AutoFixture

اول Customer anonymous من AutoFixture كان ممكن يكسر BR-12 لان تاريخ الميلاد والـ license date عشوائيين ومش مضمون يكونوا صالحين للحجز عملت `ValidAdultCustomerCustomization` علشان Customer الافتراضي يبقى adult ورخصته اقدم من سنه

استخدمت `[Theory, AutoMoqData]` مع `[Frozen] Mock<IPaymentGateway>` علشان BookingService يتبني تلقائي وبعدها اتحقق ان Platinum ما بينادي payment gateway

استخدمت `[InlineAutoMoqData]` للاعمار 20 و21 و25 العمر هو الثابت في الاختبار والباقي AutoFixture بيبنيه

## AutoFixture او Bogus

AutoFixture مناسب لما محتاج objects كثيره بسرعة زي `CreateBooking_AutoMoqBuildsService_PlatinumGatewayIsNeverCalled` لان dependencies بتتعمل تلقائي

Bogus مناسب لما شكل البيانات نفسها مهم او عايز بيانات اقرب للواقع زي `Calculate_BogusRandomName_DoesNotChangeBaseAmount` في Task 5

## Verify snapshot

اختبار `CreateBooking_FixedClockAndReference_MatchesApprovedSnapshot` بيعمل snapshot للـ Booking بعد انشاؤه الوقت ثابت والـ reference ثابت علشان الـ snapshot ما يتغيرش كل تشغيل

لو الوقت او reference عشوائيين الاختبار يبقى flaky وده يكسر FIRST لان نفس الكود ممكن ينجح مره ويفشل مره
