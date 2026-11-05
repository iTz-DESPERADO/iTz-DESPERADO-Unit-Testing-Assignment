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
