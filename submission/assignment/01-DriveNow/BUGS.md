# Bugs

Name: Gamal Khaled Mohamed
Group / ID: G4

| # | Rule | Failing test | Expected | Actual | Fix commit |
|---|---|---|---|---|---|
| 1 | BR-03 | CalculateRentalDays_WithinGracePeriod_ReturnsOneDay — مدة 25 ساعه بالظبط | 1 day | 2 days | لم يتم الاصلاح بعد |
| 1 (حاله اضافيه بعد مراجعة AI) | BR-03 | CalculateRentalDays_GracePeriodAfterTwoDays_ReturnsCorrectDays — مدة 49 ساعه بالظبط | 2 days | 3 days | لم يتم الاصلاح بعد |
| 2 | BR-30 | Calculate_RentalDuration_ReturnsCorrectDaysAndBaseAmount — مدة 25 ساعه بالظبط | 1 day, 600 base amount | 2 days | لم يتم الاصلاح بعد |
| 3 | BR-33 | Calculate_RentalDays_ReturnsCorrectDurationDiscount — 7 ايام | 420 | 0 | لم يتم الاصلاح بعد |
| 4 | BR-37 | Calculate_DiscountCap_ReturnsCorrectPromoDiscount — 7 ايام مع Gold وPromo 15% | 210 | 630 | لم يتم الاصلاح بعد |
| 5 | BR-39 | Calculate_WorkedExample_ReturnsWholePriceBreakdown — Example B مع Silver وسائق اضافي | 700 | 0 | لم يتم الاصلاح بعد |
| 6 | BR-16 | CreateBooking_LuxuryDriverIsExactlyTwentyFiveWithThreeLicenseYears_CreatesBooking | booking is created | BookingRejectedException | لم يتم الاصلاح بعد |
| 7 | BR-21 | CreateBooking_PickupEqualsExistingReturn_CreatesBooking و CreateBooking_ReturnEqualsExistingPickup_CreatesBooking | back-to-back booking is created | BookingRejectedException | لم يتم الاصلاح بعد |

لما شغلت الاختبارات لقيت ان مدة 25 ساعه بالظبط بتتحسب يومين مع ان القاعده بتقول ان ساعه السماح ما بتضيف يوم الا لما نزيد عن 60 دقيقه

السبب ان الشرط الموجود في `RentalMath.CalculateRentalDays` هو `remainder.TotalMinutes >= 60` وده بيضيف يوم حتى لما الباقي يكون 60 دقيقه بالظبط

نتيجة التشغيل قبل اضافة حالات AI:28 حاله 27 نجحت وحاله واحده فشلت

رساله الفشل

```text
Expected result to be 1, but found 2.
```

### بعد اضافة حالات AI

لما ضفت الحالات اللي وافقت عليها من مراجعة AI وشغلت الاختبارات ظهرت حاله فاشله كمان لما المده تكون 49 ساعه بالظبط. المتوقع يومين بس الكود رجع 3 ايام

دي حاله تانيه لنفس الخطأ مش bug مختلف لان السبب برضه ان الكود بيضيف يوم لما الباقي يكون 60 دقيقه بالظبط بعد يومين كاملين

نتيجة التشغيل بعد اضافة حالات AI:37 حاله 35 نجحت وحالتين فشلوا ومده الاختبارات كانت 0.7 ثانيه

رساله الفشل

```text
Expected result to be 2, but found 3.
```

### BR-30

لما شغلت اختبار مدة الايجار في `PricingService` لقيت ان مدة 25 ساعه بالظبط رجعت يومين بدل يوم واحد حسب القاعدة الاختبار وقف عند فحص `RentalDays` لذلك فحص `BaseAmount` ما وصل له وطبعا هدا bug يتبع ال tests القديمه  لانه بيعتمد في حسابه علي RentalMathService

رساله الفشل

```text
Expected result.RentalDays to be 1, but found 2.
```

### BR-33

لما شغلت اختبار خصم المدة لقيت ان 7 ايام رجعت من غير خصم مع ان القاعدة بتقول من 7 الى 13 يوم الخصم 10% المتوقع 420 من BaseAmount قيمته 4200 لكن الكود رجع 0

السبب ان الشرط في `PricingService` مكتوب `days > 7` وده بيبدأ الخصم من 8 ايام بدل 7 ايام

رساله الفشل

```text
Expected price.DurationDiscount to be 420M, but found 0M.
```

### BR-37

لما جربت 7 ايام مع Gold وPromo 15% المتوقع PromoDiscount يكون 210 لان خصم المدة 10% وخصم الولاء 10% فيفضل 5% فقط قبل حد 25% الكود رجع 630 يعني حسب Promo 15% كامل

الخطأ هنا ان `PricingService` ما بيطبقش حد 25% على خصم الـPromo وفشل BR-33 عند 7 ايام ظاهر في نفس الحالة كمان لكنه مسجل فوق كـ bug منفصل

رساله الفشل

```text
Expected price.PromoDiscount to be 210M, but found 630M.
```

### BR-39

في Example B العميل Silver وعنده سائق اضافي المتوقع تكلفة السائق تكون 700 لمدة 7 ايام لان المجاني مطلوب لـGold وPlatinum فقط لكن الكود رجع 0

السبب ان `DriverCost` بيستخدم `tier >= LoyaltyTier.Silver` فبيعطي Silver السائق المجاني مع انه مش موجود في القاعدة

رساله الفشل من مثال BR-40 الكامل

```text
Expected property actual.AdditionalDriversCost to be 700M, but found 0M.
```

### BR-16

اختبرت Luxury مع عمر 25 سنة بالظبط ورخصة عمرها 3 سنوات القاعدة تقول age at least 25 لكن الكود رفض الحجز

السبب ان الشرط في `BookingService` مكتوب `age > 25` بدل `age >= 25`

رساله الفشل

```text
Luxury vehicles require age 25+ and 3 years of license
```

### BR-21

اختبرت حجز جديد وقت استلامه يساوي وقت ارجاع الحجز الموجود. القاعدة تقول back-to-back مسموح لكن الكود رفضه كانه overlap

بعد مراجعة AI ضفت الطرف الثاني برضه: وقت ارجاع الحجز الجديد يساوي وقت استلام الحجز الموجود فشل بنفس الرسالة ونفس السبب

السبب ان شرط overlap يستخدم `request.PickupAt <= existing.ReturnAt` و `request.ReturnAt >= existing.PickupAt` لازم تكون المقارنة strict علشان وقتين متساويين ما يتحسبوش overlap

رساله الفشل

```text
Vehicle is already booked for these dates
```


