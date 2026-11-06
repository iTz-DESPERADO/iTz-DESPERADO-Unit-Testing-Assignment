# Bugs

Name: Gamal Khaled Mohamed
Group / ID: G4

| # | Rule | Failing test | Expected | Actual | Fix commit |
|---|---|---|---|---|---|
| 1 | BR-03 | CalculateRentalDays_WithinGracePeriod_ReturnsOneDay — مدة 25 ساعه بالظبط | 1 day | 2 days | لم يتم الاصلاح بعد |
| 1 (حاله اضافيه بعد مراجعة AI) | BR-03 | CalculateRentalDays_GracePeriodAfterTwoDays_ReturnsCorrectDays — مدة 49 ساعه بالظبط | 2 days | 3 days | لم يتم الاصلاح بعد |

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

