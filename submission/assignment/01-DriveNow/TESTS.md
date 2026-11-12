## Task 1 — RentalMath

Name: Gamal Khaled Mohamed
Group / ID: G4

#### BR-01 — CalculateAge

| Business Rule | Test Method | Boundaries Tested |
|---|---|---|
| BR-01 | CalculateAge_ValidDate_ReturnsCorrectAge |غطيت الحالات قبل يوم الميلاد وفي يوم الميلاد وبعده وكمان  مواليد 29 فبراير في السنوات الكبيسة وغير الكبيسة |

شايف ان الحالات دي كفاية لحدود عيد الميلاد لاني اختبرت اليوم اللي قبله ونفس اليوم واليوم اللي بعده وكمان مواليد 29 فبراير في سنة كبيسة وسنة عادية 



#### AI Review — CalculateAge

| AI Suggestion | Decision | Why |
|---|---|---|
| Normal birthday boundaries | Rejected | Already covered |
| Leap-year birthday boundaries | Rejected | Already covered |
| Non-leap-year boundaries | Rejected | Already covered |
| Birth date equals calculation date | Accepted | علشان اتأكد ان العمر صفر لما تاريخ الحساب يكون نفس تاريخ الميلاد |
| Calculation date before birth date | Rejected | Not specified in BR-01 |
| Split tests into separate methods | Rejected | Parameterized test is sufficient |

#### Judge the AI

اقترح الذكاء الاصطناعي حالات لعيد الميلاد كنت مغطيها باستخدام InlineData لكن حالة تاريخ الميلاد يساوي تاريخ الحساب كانت ناقصة وقبلتها واضفتها

بس هو مش عارف يتأكد من تغطيه الحالات لان انا اعطيته اسم الداله فقط وما اعطيته البيانات كامله 

راجعت الاقتراحات وقارنتها بالاختبار الموجود عندي ولقيت اني غطيت بالفعل حالات عيد الميلاد العاديه والحالات الحديه  للسنوات الكبيسه



#### BR-02 — CalculateLicenseYears

| Business Rule | Test Method | Boundaries Tested |
|---|---|---|
| BR-02 | CalculateLicenseYears_GivenIssueDates_ReturnsCorrectLicenseYears |غطيت الحالات من تاريخ اصدار الرخصه وبعده وكمان  تأكدت من الاصدار لما يكون 29 فبراير في السنوات الكبيسة وغير الكبيسة |

عارف ان فيه حالات ناقصة زي اليوم اللي قبل اكتمال سنة من اصدار الرخصة وكمان لما المدة تكون اقل من سنة


### AI Suggestions Review — CalculateLicenseYears

| AI Suggestion | Decision | Why |
|---|---|---|
| Day before anniversary | Accepted | Missing from initial tests |
| Exact anniversary | Rejected | Already covered using InlineData |
| Day after anniversary | Rejected | Already covered using InlineData |
| Before leap-year anniversary | Accepted | Missing from initial tests |
| Before adjusted anniversary in non-leap year | Accepted | Missing from initial tests |
| Other leap-year boundaries | Rejected | Already covered using InlineData |
| Other non-leap-year boundaries | Rejected | Already covered using InlineData |
| Same issue date | Accepted | Verify that the result is 0 |
| Less than one complete year | Accepted | Verify that incomplete years are not counted |
| Anniversary still ahead | Rejected | Covered by the accepted before-anniversary case |
| Year transition | Accepted | Verify calculation across calendar-year boundaries |
| Multiple non-leap years | Rejected | The adjustment rule is already tested in a non-leap year |
| Future issue date | Rejected | Behavior is not specified in BR-02 |
| Separate tests for CalculateAge | Rejected | BR-01 is already tested separately |

#### Judge the AI — CalculateLicenseYears

 اقترح اضافة اختبارات مباشرة لدالة `CalculateAge` بس انا اختبرت قاعدة العمل `BR-01` بشكل منفصل  وعلشان هيك الاضافة مش  ضرورية

قبلت اقتراحات اضافة الحالات الحدية قبل ذكرى اصدار الرخصه وحالات السنوات الصفرية والانتقال بين السنوات الميلادية بهدف تحسين تغطية الاختبارات الخاصة بقاعدة العمل `BR-02`


#### BR-03 — CalculateRentalDays

| Business Rule | Test Method | Boundaries Tested |
|---|---|---|
| BR-03 | CalculateRentalDays_WithinGracePeriod_ReturnsOneDay |غطيت مدة 24 ساعه وكمان 25 ساعه بالظبط علشان اتأكد ان ساعه السماح ما بتضيف يوم |
| BR-03 | CalculateRentalDays_BeyondGracePeriod_ReturnsTwoDays |غطيت لما المده تكون 25 ساعه ودقيقه علشان اتأكد ان بعد ساعه السماح بينضاف يوم |
| BR-03 | CalculateRentalDays_MinimumDuration_ReturnsOneDay |غطيت لما الارجاع يكون بعد الاستلام بثانيه علشان اتأكد ان اقل مده بتتحسب يوم واحد |
| BR-03 | CalculateRentalDays_InvalidReturnTime_ThrowsArgumentException |غطيت لما وقت الارجاع يكون نفس وقت الاستلام وكمان قبله بثانيه واتأكدت من نوع الخطأ والرساله |

غطيت الحالات دي علشان اتأكد من حساب اليوم وساعه السماح وكمان اقل مده والوقت الغلط للارجاع بس لسه محتاج اراجع اذا كانت كفايه لكل حدود القاعده ولا فيه حالات ناقصه


### AI Suggestions Review — CalculateRentalDays

| AI Suggestion | Decision | Why |
|---|---|---|
| 24 hours returns 1 day | Rejected | موجوده بالفعل باستخدام InlineData |
| 24 hours 59 minutes returns 1 day | Accepted | ناقصه وعلشان اختبر قبل نهايه ساعه السماح بدقيقه |
| 25 hours exactly returns 1 day | Rejected | موجوده بالفعل وهي الحاله اللي كشفت الخطأ في الكود |
| 25 hours 1 minute returns 2 days | Rejected | موجوده بالفعل باستخدام InlineData |
| 48 hours returns 2 days | Accepted | علشان اتأكد من حساب اكتر من يوم كامل |
| 48 hours 59 minutes returns 2 days | Accepted | علشان اتأكد من ساعه السماح بعد يومين كاملين |
| 49 hours exactly returns 2 days | Accepted | علشان اختبر حد ال 60 دقيقه بالظبط بعد يومين |
| 49 hours 1 minute returns 3 days | Accepted | علشان اتأكد ان اليوم الزياده بينضاف لعدد الايام المحسوبه مش بيرجع يومين دايما |
| 72 hours returns 3 days | Accepted | مثال موجود في README ولسه ما اختبرته |
| 1 second returns 1 day | Rejected | موجوده بالفعل في اختبار اقل مده |
| 3 hours returns 1 day | Accepted | مثال موجود في README وبيختبر مده اقل من يوم |
| 23 hours 59 minutes returns 1 day | Accepted | علشان اتأكد من الحد الادنى قبل اكتمال اول يوم بدقيقه |
| Return equals pickup | Rejected | موجوده بالفعل في اختبار الوقت الغلط للارجاع |
| Return before pickup | Rejected | موجوده بالفعل في اختبار الوقت الغلط للارجاع |
| 25 hours 1 second returns 2 days | Accepted | علشان اتأكد ان تجاوز ساعه السماح بثانيه بيضيف يوم كمان |

رفضت الحالات الموجوده علشان ما اكررها مش علشان النتيجه اللي اقترحها غلط


#### Judge the AI — CalculateRentalDays

الـ AI اقترح حالات انا مختبرها بالفعل زي 24 ساعه و25 ساعه بالظبط و25 ساعه ودقيقه وكمان الارجاع في نفس الوقت او قبله. هو شاف اسماء الاختبارات بس وما شاف بيانات InlineData فعلشان هيك ما قدر يتأكد من الحالات الموجوده

قال ان اختبار ساعه السماح واختبار اقل مده ممكن يكونوا مكررين لو الاتنين بيختبروا مدد اقل من يوم بس عندي مش مكررين لان الاول بيختبر 24 و25 ساعه والتاني بيختبر ثانيه واحده

اقتراحاته عن حساب اكتر من يوم وحدود ساعه السماح بعد يومين كانت ناقصه عندي وقبلتها علشان اتأكد ان الحساب صحيح مع مدد مختلفه

وكمان راجعت كلامه عن الثانيه الواحده وهو قال انها اقل مده موجبه على مستوى دقه الثواني وده صحيح بس مش اقل مده بيدعمها DateTime لانه بيدعم ticks اقل من الثانيه

## Task 5 — PricingService

| Rule | Tests | Boundaries I covered 
|---|---|---|
| BR-30 | `Calculate_RentalDuration_ReturnsCorrectDaysAndBaseAmount`, `Calculate_InvalidRentalSpan_ThrowsArgumentException`, `Calculate_BogusRandomName_DoesNotChangeBaseAmount` | جربت مدة اقل من يوم و25h و25h 1m، وكمان مدة غير صحيحة |
| BR-31 | `Calculate_WeekendDayStarts_ReturnsCorrectWeekendSurcharge` | جربت بداية الايجار يوم الخميس والجمعة والسبت |
| BR-32 | `Calculate_DriverAge_ReturnsCorrectYoungDriverSurcharge`, `Calculate_BogusFixedAge_ReturnsExpectedYoungSurcharge` | اختبرت عمر 21 و25 وكمان عمر 23 باستخدام Bogus |
| BR-33 | `Calculate_RentalDays_ReturnsCorrectDurationDiscount` | اختبرت 7 و14 يوم |
| BR-34 | `Calculate_LoyaltyTier_ReturnsCorrectLoyaltyDiscount` | غطيت مستويات None وSilver وGold وPlatinum 
| BR-35 | `Calculate_PromoPercent_ReturnsPercentageOfBase`, `Calculate_ValidPromo_ReturnsPercentOfBase` | جربت نسب 5% و15%، وتاكدت من التعامل مع اختلاف الحروف في كود الخصم |
| BR-36 | `Calculate_PromoValidation_ReturnsExpectedOutcome` | اختبرت الكود المفقود، promo صالحة، promo منتهية، الحد الادنى للايام، وفئة غير مسموحة |
| BR-37 | `Calculate_DiscountCap_ReturnsCorrectPromoDiscount` | جربت Promo لوحدها وحالة تجمع خصم المدة والولاء مع Promo |
| BR-38 | `Calculate_InsurancePlanAndCategory_ReturnsCorrectInsuranceCost` | جربت Basic وStandard مع Economy وPremium مع Luxury |
| BR-39 | `Calculate_AdditionalDriversAndTier_ReturnsCorrectDriverCost` | اختبرت صفر وسائق اضافي، وGold مع اول سائق مجاني ومع سائقين |
| BR-40 | `Calculate_WorkedExample_ReturnsWholePriceBreakdown`, `Calculate_WorkedExample_SnapshotMatchesApproved` | استخدمت مثالين كاملين مع `MemberData` وVerify

**think these are enough because**

 لان الاختبارات الحالية بتغطي كل القواعد في شكلها الاساسي خصوصا حدود الخصومات ورسوم السائقين والتامين

لكن مش شايف ان التغطية كاملة 100% انا غالبا بنسى boundary cases مثلا ما جربتش الايجار اللي يعدي على ويك اندين او بداية الايجار بعد منتصف الليل او لما `AllowedCategories` تكون `null` او لما اكثر من خصم ياثر على نفس السعر

كمان اختبار القاعده لوحدها شيء واختبارها مع قواعد ثانيه شيء مختلف علشان كده حاسس ان بعض التركيبات محتاجة اختبار اضافي

### R4 — Bogus and FIRST

استخدمت **Fixed Seed** مع Bogus علشان البيانات ما تتغيرش كل مرة اشغل فيها الاختبارات لو اختبار فشل اقدر اعيده بنفس البيانات واعرف المشكلة من غير ما العشوائية تغير النتيجة

في اختبار `Calculate_BogusRandomName_DoesNotChangeBaseAmount` استخدمت اسم عشوائي لان اسم العميل اصلا مش المفروض ياثر على حساب السعر

لكن في `Calculate_BogusFixedAge_ReturnsExpectedYoungSurcharge` خليت العمر ثابت على 23 سنة لان لو العمر اتغير عشوائيا ممكن تتغير رسوم السائق الشاب وساعتها الاختبار يبقى غير ثابت

### R5 — FluentAssertions

في اختبار `Calculate_WorkedExample_ReturnsWholePriceBreakdown` استخدمت `BeEquivalentTo` بدل ما اقارن كل Property لوحدها لان المطلوب هنا التاكد من نتيجة `PriceBreakdown` كاملة

واستخدمت `AssertionScope` علشان لو فيه اكثر من قيمة غلط اقدر اشوف الاختلافات مع بعض بدل ما الاختبار يقف عند اول Assertion

### R6 — Verify vs Assertions

شايف ان كل طريقة مناسبة لنوع معين من الاختبارات

الـ **Assertions العادية** اوضح لما اكون باختبر Rule محددة زي خصم الولاء او تكلفة التأمين لان الاختبار بيبين بالضبط ايه القيمة اللي متوقعها

اما **Verify** فهو مناسب اكثر للامثلة الكاملة لان `PriceBreakdown` فيها حقول كثيره والـ Snapshot بتساعدني الاحظ لو اي قيمة اتغيرت

### AI Suggestions Review — PricingService

| AI Suggestion | Decision | Why |
|---|---|---|
| BR-30: 24h و24h 59m وحدود اكثر من يوم | Accepted | ناقصين عندي وعلشان اتاكد من حساب BR-03 لما يستخدمه PricingService |
| BR-30: فحص BaseAmount مع كل مدة | Rejected | اختبار BR-30 الحالي بيفحص RentalDays وBaseAmount مع بعض |
| BR-31: Sunday وايجار فيه اكثر من يوم ويعدي ويك اندين | Accepted | انا غطيت الخميس والجمعه والسبت بس ولسه الحالات دي ناقصه |
| BR-31: اليوم الجزئي بعد ساعه السماح يبدا يوم جمعه او سبت | Accepted | مهم علشان اتاكد ان اليوم الزيادة يغير رسوم الويك اند صح |
| BR-32: عمر 20 و24 وحدود يوم الميلاد | Accepted | اختبرت 21 و25 فقط ولسه حدود العمر الثانية وتاريخ الميلاد ناقصين |
| BR-33: 6 و13 و15 يوم | Accepted | عندي 7 و14 فقط ومحتاج اتاكد قبل وبعد حدود الخصم |
| BR-34: كل مستويات الولاء | Rejected | انا مختبر None وSilver وGold وPlatinum بالفعل |
| BR-35: Promo بنسبة 0% وسعر Base فيه كسور | Accepted | ناقصين وبيختبروا النسبة والحساب بشكل اوضح |
| BR-36: تاريخ الصلاحية من اول واخر يوم، null categories، وحد الاستخدام واولوية الاخطاء | Accepted | الحالات دي مش موجودة في النسخة الاولى من ClassData |
| BR-37: بالظبط 25% وتحت وفوق الحد وPromo تبقى صفر | Accepted | اختبرت  واحده فقط ولسه حدود cap ناقصه |
| BR-38: كل خطط التامين مع Luxury وغير Luxury واكثر من يوم | Accepted | انا غطيت حالات اساسية فقط |
| BR-39: Silver وPlatinum واكثر من سائق واكثر من يوم | Accepted | انا اختبرت None وGold فقط وما اختبرت Silver وPlatinum بشكل واضح |
| BR-40: اختبار total كامل لكل اجزاء السعر | Rejected | موجود عندي بالفعل في `Calculate_WorkedExample_ReturnsWholePriceBreakdown` لكنه ما انبعتش للـAI لانه مش من اسماء اختبارات القواعد اللي ارسلتها |
| `Calculate_PromoPercent` و`Calculate_ValidPromo` ممكن يكونوا مكررين | Rejected | واحد بيتاكد من حساب نسب الخصم وواحد بيتاكد من promo صالحة واختلاف الحروف فمش نفس الهدف |

#### Judge the AI — PricingService

الـ AI اقترح حدود وتركيبات كثيره انا فعلا ما فكرتش فيها خصوصا BR-31 مع اليوم الزيادة من BR-03، وBR-36 لان فيه حالات تحقق كثيرة واولوية بين الاخطاء قبلت اغلبها لانها بتكمل نقص واضح في الاختبارات الاولى

هو قال ان BR-40 ما عندهاش اختبار صريح لكن ده مش صحيح عندي لان `Calculate_WorkedExample_ReturnsWholePriceBreakdown` بيقارن `PriceBreakdown` كاملة و`Calculate_WorkedExample_SnapshotMatchesApproved` بيعمل Verify. السبب انه ما شاف الاسماء دي لاني ارسلت له اختبارات القواعد فقط

وبرضه كلامه عن تعارض BR-37 مع 14 يوم وPlatinum صحيح: النص بيقول الخصومات كلها ما تعديش 25% وفي نفس الوقت يقول نحافظ على مدة وولاء لكن الاتنين لوحدهم 30% علشان كده رفضت الحالة دي لحد ما يكون فيه نص صريح انه اعمل كده او لازم اراجع حضرتك في الموضوع

## Task 6 — BookingService

| Rule | Tests | اللي اختبرته |
|---|---|---|
| BR-10 | `CreateBooking_CustomerDoesNotExist_ThrowsCustomerNotFoundException` | customer مش موجود والرسالة |
| BR-11 | `CreateBooking_CustomerIsBlacklisted_RejectsBooking` | blacklist |
| BR-12 | `CreateBooking_DriverIsTwenty_RejectsBooking` و InlineAutoData للاعمار 20 و21 و25 | حد العمر |
| BR-13 | `CreateBooking_LicenseIsLessThanOneYearOld_RejectsBooking` | قبل سنة من الرخصة |
| BR-14 - BR-15 | vehicle مش موجود وvehicle مش available | التحقق من السيارة |
| BR-16 - BR-17 | Luxury عند 25 ورخصة 3 سنوات وVan عند سنتين | حدود Luxury وVan |
| BR-18 - BR-20 | pickup بعد ساعتين، rental period، وadditional drivers | حدود الوقت وعدد السائقين |
| BR-21 - BR-22 | back-to-back و2 open bookings | overlap وعدد الحجوزات |
| BR-23 - BR-25 | price وReserved وcreated time وmileage وreference | بيانات الحجز الجديد |
| BR-26 | كل deposits وPremium وPlatinum بدون gateway | الدفع |
| BR-27 | promo update وsave وSMS وemail وaudit | اجراءات بعد الحجز |

### Self review - BookingService

**think these are enough because**

غطيّت كل BR من 10 الى 27 وحطيت الحدود المهمة زي 20 و21 و25 للعمر، ساعتين بالظبط، 30 يوم، 0 الى 3 سائقين، وback-to-back 

لكن ممكن اكون نسيت combinations بين القواعد مثلا Luxury مع pickup boundary او promo مع payment failure

### AI Suggestions Review — BookingService

| AI Suggestion | Decision | Why |
|---|---|---|
| BR-10, BR-11, BR-14, BR-15 exact messages | Rejected | موجودة بالفعل في Assertions لكن الـ AI شاف اسماء الاختبارات فقط |
| BR-12 يوم قبل 21 وBR-13 سنة رخصة بالظبط | Accepted | حدود مهمة كانت ناقصة بشكل واضح |
| BR-16 age 24 وlicense سنتين كل واحد لوحده | Accepted | علشان اتاكد ان شروط Luxury مستقلة |
| BR-17 Van مع سنة رخصة | Accepted | شرط Van مختلف عن شرط الرخصة العام |
| BR-18 tick قبل ساعتين | Accepted | الحد قبل ساعتين ما كانش موجود |
| BR-19 tick بعد pickup و30 يوم بالظبط | Accepted | علشان اراجع بداية ونهاية المدة بدقة |
| BR-20 صفر و3 سائقين | Accepted | كنت مختبر -1 و4 فقط في الحد الغلط |
| BR-21 كل overlap والطرف الثاني من back-to-back | Accepted | كان عندي طرف مسموح واحد فقط وما اختبرت overlap فعلي |
| BR-22 حجز واحد وCompleted booking | Accepted | علشان اتاكد ان الحجوزات المقفولة ما تتحسبش |
| BR-23 Verify ان PricingService اتنادى | Rejected | `PricingService` class ملموس وليس dependency قابلة للـmock والاختبار الحالي يتحقق من السعر الناتج |
| BR-24 فصل كل property | Rejected | Status وCreatedAt وStartMileage موجودين فعلا في نفس الاختبار لانهم نتيجة نفس السلوك |
| BR-25 تاريخ pickup مختلف عن created-at | Accepted | اضفته بـ SpyReferenceGenerator علشان اتاكد انه بيمرر pickup date |
| BR-26 حالات deposit مختلفة | Rejected | موجودة بالفعل لكل الفئات مع Luxury Premium وPlatinum بدون gateway |
| BR-27 side effects في نجاح promo | Rejected | موجودة بالفعل: promo وsave وSMS وemail وaudit |
| BR-27 حجز مرفوض لا يعمل side effects | Accepted | حالة مهمة علشان promo ما تتستهلكش وما تبعتش confirmation غلط |

#### Judge the AI — BookingService

الـ AI افترض ان بعض الحاجات ناقصة لان الاسم وحده ما يثبتش الـ Assertions راجعت الكود ولقيت ان رسائل BR-10 الى BR-15 وdeposit combinations وside effects للحجز الناجح موجودة بالفعل فرفضتهم مش علشان الاقتراح غلط لكن علشان الكود مغطيهم

قبلت الحدود وحالات overlap والحجوزات المقفولة وعدم وجود side effects للحجز المرفوض لانها كانت فعلا ناقصة بعد الاضافة ظهر نفس bug BR-21 في الطرفين المسموحين من back-to-back، وظهر bug BR-16 عند عمر 25 من قبل

## Task 7 — Pickups, Returns and Cancellations

| Rule | Tests | اللي اختبرته |
|---|---|---|
| BR-28 | `StartRental_BookingDoesNotExist_ThrowsBookingNotFoundException`, `StartRental_BookingIsNotReserved_ThrowsInvalidBookingStateException`, `StartRental_OneTickBeforeAllowedTime_ThrowsInvalidBookingStateException`, `StartRental_AtOrAfterAllowedTime_MarksBookingActive` | الحجز المفقود والحالات الغلط وحد ساعة قبل pickup على الجهتين |
| BR-41 | `ProcessReturn_LateMinutes_ReturnsCorrectLateFee` | 0 و59 و60 و61 و239 و240 و241 دقيقة وكمان 24 ساعة وبعدها بدقيقة |
| BR-42 | `ProcessReturn_DistanceOverAllowance_ReturnsCorrectMileageFee` | حد المسافة وextra km وLuxury |
| BR-43 | `ProcessReturn_MissingFuelQuarters_ReturnsCorrectFuelFee`, `ProcessReturn_FuelQuartersOutsideRange_ThrowsArgumentException` | 4 و3 و2 quarters وحدود fuel الغلط |
| BR-44 | `ProcessReturn_DamageAndInsurance_ReturnsCorrectDamageCharge` | Basic وStandard وPremium والضرر قبل وبعد deductible |
| BR-45 | `ProcessReturn_EarlyReturn_DoesNotRefundUnusedRentalDays` | رجوع بدري بدون refund لايام الايجار |
| BR-46 | `ProcessReturn_ValidReturn_CompletesRentalAndHandlesPayments` | Completed وmileage وloyalty وrefund والايميل وaudit |
| BR-50 | `Cancel_BookingIsNotReserved_ThrowsInvalidBookingStateException`, `Cancel_PickupTimeHasArrived_ThrowsInvalidBookingStateException` | Reserved فقط ووقت pickup نفسه |
| BR-51 | `Cancel_HoursUntilPickup_ReturnsCorrectFee` | 48h و قبلها، 24h و قبلها، وPlatinum |
| BR-52 | `Cancel_DepositIsMoreThanFee_RefundsTheDifference`, `Cancel_FeeIsMoreThanDeposit_ChargesTheDifference` | deposit اكبر من fee والعكس |
| BR-53 | `Cancel_ValidBooking_CancelsAndSendsNotifications` | Cancelled وpromo decrement وSMS وemail وaudit |

### Self review - Pickups, Returns and Cancellations

**think these are enough because**

غطيت كل حدود الوقت اللي في القواعد: ساعة قبل pickup لبدء الايجار 59 و60 و61 و240 دقيقة للتاخير 24 و48 ساعة للالغاء وكمان اختبرت قبل وبعد الحدود المهمة

### AI Suggestions Review — Pickups, Returns and Cancellations

| AI Suggestion | Decision | Why |
|---|---|---|
| BR-28 exact one hour، 59 minutes، بعد pickup، والرسائل | Rejected | موجودة بالفعل في الاختبارات الحالية: tick قبل الحد، عنده، وبعده، والرسالة exact |
| BR-41 119 و120 و121 دقيقة و59m59s | Accepted | حالات rounding داخل الساعة ودقة الثواني كانت ناقصة |
| BR-41 59/60/61 و240/241 و1440/1441 | Rejected | موجودة بالفعل في `InlineData` |
| BR-42 Luxury عند 1 km زيادة | Accepted | علشان اتأكد من سعر Luxury بالحد الاول |
| BR-42 allowance مع rental days وregular rate | Rejected | موجودة بالفعل بحالات 2 days وEconomy |
| BR-43 كل fuel values وVan | Accepted | كان عندي حالات اساسية وSUV فقط |
| BR-44 zero damage وBasic عند deductible | Accepted | حدود deductible دي ناقصة |
| BR-45 early return 7 days وextras | Rejected | الاختبار الحالي يثبت قيمة الايجار ما تتغيرش وعدم وجود refund لليوم الباقي |
| BR-46 combined extras في charge واحد | Accepted | علشان اجمع late fee وmileage وfuel وdamage في سيناريو واحد |
| BR-46 loyalty وzero extras وdeposit branches | Rejected | موجودة بالفعل في اختبار return الكامل: 4 الى 5 Silver وzero extras وdeposit refund |
| BR-50 booking missing | Accepted | كان ناقص |
| BR-50 Active/Completed/Cancelled وpickup now | Rejected | موجودة بالفعل |
| BR-51 49h وPlatinum عدة مرات | Rejected | 48h و24h وعلى الجهتين موجودة، وPlatinum عند ساعة واحدة يغطي branch الاعفاء |
| BR-52 deposit يساوي fee | Accepted | حد equality كان ناقص |
| BR-53 cancellation بدون promo | Accepted | علشان promo غير المستخدم ما يتغيرش |
| BR-53 الرسائل والpromo decrement والfee | Rejected | موجودة بالفعل في اختبار cancellation الكامل |

#### Judge the AI — Pickups, Returns and Cancellations

الـ AI شاف اسماء الاختبارات فقط علشان كده اقترح حالات موجودة فعلا في `InlineData` زي 59 و60 و61 دقيقة و240 و241 دقيقة او موجودة في Assertions زي رسائل الخطأ وpromo decrement

قبلت الحالات اللي فعلًا ناقصة خصوصا seconds في BR-41 وحدود fuel وdeductible والـcombined extras وحد equality بين deposit وfee بعد الاضافة ما ظهرش bug جديد وفضلوا نفس BR-41 عند 240 دقيقة وBR-51 عند 48 ساعة
