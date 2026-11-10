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
