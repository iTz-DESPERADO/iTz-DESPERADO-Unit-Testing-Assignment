# Hidden Inputs — Task 4

Name: Gamal Khaled Mohamed

Group / ID: G4

## Steps

| Class | Original line | Hidden input | Why it hurts a test | How I removed it |
|---|---|---|---|---|
| CancellationService | 25, 33 | DateTime.Now | وقت الاختبار بيتغير ومش بقدر اتحكم في حدود وقت الالغاء | مررت TimeProvider في constructor واستخدمته بدل الوقت المباشر |
| ReturnService | 37 | DateTime.Now | وقت الارجاع وحساب التأخير بيتغيروا حسب وقت التشغيل | مررت TimeProvider في constructor واستخدمته لتحديد وقت الارجاع ومررت TimeProvider.System من ConsoleApp |
| PricingService | 77 | DateTime.Today | صلاحية كود الخصم بتعتمد على تاريخ الجهاز ومش بقدر اثبته في الاختبار | مررت TimeProvider في constructor واستخدمت التاريخ المحلي منه بدل DateTime.Today |
| BookingService | 18 | انشاء PricingService داخل الكلاس | مش بقدر امرر خدمة تسعير بساعه متحكم فيها | مررت PricingService في constructor وربطتها من ConsoleApp |
| BookingService | 56, 89, 125 | DateTime.Now | وقت انشاء الحجز وشروط الاستلام وبدء الايجار بتعتمد على وقت الجهاز | مررت TimeProvider في constructor واستخدمت الوقت المحلي منه في الثلاث اماكن وربطته من ConsoleApp |
| BookingService | 58, 150–159  | RandomCode وتوليد مرجع عشوائي | مرجع الحجز بيتغير في كل تشغيل ومش بقدر اثبته في الاختبار | مررت IBookingReferenceGenerator في constructor ونقلت التنفيذ العشوائي لـ BookingReferenceGeneratorService وربطته من ConsoleApp |
| PricingService | 13  | انشاء PromoCodeRepository داخل الكلاس | مش بقدر اتحكم في بيانات اكواد الخصم بدون المخزن الحقيقي | مررت IPromoCodeRepository في constructor وربطت التنفيذ من ConsoleApp المخزن static لسه هيتعدل في خطوه لاحقه |
| BookingService / CancellationService | 17 / 14  | انشاء PromoCodeRepository داخل الخدمتين | مش بقدر اعزل قراءة كود الخصم وتحديث عدد استخداماته في الاختبار | مررت IPromoCodeRepository للخدمتين من ConsoleApp  |
