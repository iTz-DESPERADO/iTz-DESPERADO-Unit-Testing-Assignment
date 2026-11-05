# Bugs

Name: Gamal Khaled Mohamed
Group / ID: G4

| # | Rule | Failing test | Expected | Actual | Fix commit |
|---|---|---|---|---|---|
| 1 | BR-03 | CalculateRentalDays_WithinGracePeriod_ReturnsOneDay — مدة 25 ساعه بالظبط | 1 day | 2 days | لم يتم الاصلاح بعد |

لما شغلت الاختبارات لقيت ان مدة 25 ساعه بالظبط بتتحسب يومين مع ان القاعده بتقول ان ساعه السماح ما بتضيف يوم الا لما نزيد عن 60 دقيقه

السبب ان الشرط الموجود في `RentalMath.CalculateRentalDays` هو `remainder.TotalMinutes >= 60` وده بيضيف يوم حتى لما الباقي يكون 60 دقيقه بالظبط

نتيجة التشغيل:28 حاله 27 نجحت وحاله واحده فشلت

رساله الفشل

```text
Expected result to be 1, but found 2.
```
