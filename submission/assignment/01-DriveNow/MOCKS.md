# Task 8 - Legacy Receipt Tests

## R1 - Before running the legacy tests

الاختبارات الخمسه الحاليه تحمي طريقة تعاون `ReceiptService` مع الـinterfaces اكثر من انها تحمي شكل او صحة الـreceipt نفسه

- `Build_CallsLineItemFactoryOnce` يتاكد ان items بتتعمل مره واحده
- `Build_CallsTaxCalculatorOncePerLine` يتاكد ان VAT بيتحسب لكل line
- `Build_CallsCurrencyFormatterTwicePerLinePlusTwo` يثبت عدد استدعاءات `Format` مش النص النهائي
- `Build_CallsCollaboratorsInOrder` يثبت ترتيب calls بين الـmocks
- `Build_ReturnsWhatTheRendererReturns` يتاكد ان قيمة الـrenderer هي اللي بترجع

اعتقد انهم ممكن يمسكوا تغيير في عدد او ترتيب calls لكن مش هيمسكوا VAT غلط او formatting غلط طالما الـmocks راجعة نفس القيم اللي اتعمل لها setup

## R2 - Format مرة واحده

قبل الـrefactor كان `CurrencyFormatter.Format` يتنادى 8 مرات مع 3 lines مرتين لكل item ومره للـVAT ومره للـtotal بعده بقي 5 مرات فقط مرة لكل amount ثم النص الناتج يتعاد استعماله للعرض وحساب width الـconsole output للـreceipt نفسه لم يتغير

## R4 - بعد قراءة الاختبارات

التغيير في R2 كسر اختبار `Build_CallsCurrencyFormatterTwicePerLinePlusTwo` رغم ان نص الـreceipt ما اتغيرش لانه كان يقيس implementation detail وهو عدد calls وليس النتيجة التي يراها المستخدم

الاختبارات القديمة لن تمسك VAT غلط `ITaxCalculator` كان mock ويرجع `0m` في كل الحالات فـ`ReceiptService` لا يرى الحساب الحقيقي كذلك `ICurrencyFormatter` يرجع `X` و`IReceiptFormatter` يرجع `RECEIPT` لذلك لا يوجد assertion على VAT او شكل receipt الحقيقي

## R7 - interfaces التي شلتها

شلت `ILineItemFactory` و`ITaxCalculator` و`ICurrencyFormatter` و`IReceiptFormatter` كل واحده كانت لها implementation واحده فقط ولم تكن تستخدم خارج ReceiptService والاختبارات القديمة وجودها كان عامل seams صناعية للـmocks وخلى الاختبارات تقيس calls بدل receipt الحقيقي

بقيت classes العاديه وReceiptService يستعملها مباشره لو كان فيه integration خارجي او implementations متعدده كان ساعتها interface يبقى مفيد لكن ده مش موجود هنا
