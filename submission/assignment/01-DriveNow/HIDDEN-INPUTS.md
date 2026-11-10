# Hidden Inputs — Task 4

Name: Gamal Khaled Mohamed

Group / ID: G4

## Steps

| Class | Original line | Hidden input | Why it hurts a test | How I removed it |
|---|---|---|---|---|
| CancellationService | 25, 33 | DateTime.Now | وقت الاختبار بيتغير ومش بقدر اتحكم في حدود وقت الالغاء | مررت TimeProvider في constructor واستخدمته بدل الوقت المباشر |
| ReturnService | 37 | DateTime.Now | وقت الارجاع وحساب التأخير بيتغيروا حسب وقت التشغيل | مررت TimeProvider في constructor واستخدمته لتحديد وقت الارجاع ومررت TimeProvider.System من ConsoleApp |
