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
