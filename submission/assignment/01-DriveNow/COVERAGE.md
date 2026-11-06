# Code Coverage Report

Name: Gamal Khaled Mohamed
Group / ID: G4

## Baseline Coverage

Measured before Task 1 using the 5 legacy tests

| Project | Line Coverage | Branch Coverage |
|---|---:|---:|
| DriveNow.Application | 7.80% | 3.22% |
| DriveNow.Domain | 3.96% | 0% |
| DriveNow.Infrastructure | 0% | 0% |

Tests: 5 passed, 0 failed

Command:

first i download package coverlet.collector then make this command

dotnet test --collect:"XPlat Code Coverage"

## After Task 1

قست التغطيه بعد اضافة اختبارات RentalMath وحالات مراجعة AI باستخدام الامر ده من مجلد DriveNow

```powershell
dotnet test DriveNow.sln --collect:"XPlat Code Coverage"
```

النتيجه دي مجمعه من تقريري LegacyTests وUnitTests مش من تقرير UnitTests لوحده. حسبت الاسطر المغطاه بدون تكرارها بين التقريرين والنسب مقربه لخانتين

| After task | Project | Covered lines / Total | Line Coverage | Covered branches / Total | Branch Coverage |
|---|---|---:|---:|---:|---:|
| Task 1 | DriveNow.Application | 34 / 401 | 8.48% | 6 / 186 | 3.23% |
| Task 1 | DriveNow.Domain | 28 / 114 | 24.56% | 12 / 18 | 66.67% |
| Task 1 | DriveNow.Infrastructure | 0 / 218 | 0% | 0 / 20 | 0% |

تقرير LegacyTests غطى 4 اسطر في Domain وتقرير UnitTests غطى 24 سطر مختلفين فالمجموع 28 سطر. الفروع المغطاه في Domain كلها من اختبارات UnitTests

اخر تشغيل UnitTests اللي سجلته كان 37 حاله: 35 نجحت وحالتين فشلوا بسبب نفس خطأ BR-03 المسجل في BUGS.md. ملفات التغطيه لوحدها ما بتثبتش عدد الاختبارات الناجحه والفاشله في تشغيل الحل كامل


احتفظت بارقام Baseline زي ما سجلتها وقتها. لاحظت ان تقرير LegacyTests الحالي لوحده مختلف عنها: Application فيه 34/401 سطر وDomain فيه 4/114 سطر. سبب الفرق محتاج مراجعة تقرير Baseline الاصلي ومش هغير الارقام القديمه من غير دليل
