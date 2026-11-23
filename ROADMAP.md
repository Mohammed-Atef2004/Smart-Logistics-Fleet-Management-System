# 🚀 SLFMS — خطة التطوير الكاملة (Roadmap & Checklist)

> تقييم كامل + خطة تنفيذ مرحلية لتحويل المشروع من "قوي كمشروع Portfolio" إلى "مشروع بيستحق كلمة Production-Grade".
> آخر تحديث: نوفمبر 2026

---

## 📊 التقييم السريع

| المحور | الدرجة | ملاحظات |
|---|---|---|
| Domain Modeling & DDD | **8.5/10** | أقوى جزء في المشروع — Rich aggregates + Rules + Value Objects |
| Vertical Slices & التنظيم | **8/10** | بنية ثابتة ومنضطة عبر كل الـ features |
| Architecture (الالتزام الفعلي) | **5.5/10** | الـ Domain مش clean + الـ Architecture Tests فاضية |
| Testing | **5/10** | Domain tests موجودة، لكن مفيش integration tests |
| Security | **6/10** | Auth stack كامل لكن فيه ثغرات فعلية |
| Performance & Queries | **4/10** | مفيش pagination — كل GetAll بيجيب كل حاجة |
| Ops / DevEx (CI, Docker, Observability) | **2/10** | مفيش CI، مفيش Docker، مفيش health checks |
| Code Hygiene | **6/10** | بقايا مشروع تاني + typos منتشرة |

**الإجمالي: 7/10** — كود الدومين ممتاز، لكن المشروع ضعيف في "اللي حوالين الكود".

---

## 💪 نقاط القوة (حافظ عليها)

1. **Rich Domain Model حقيقي** — كل تغيير حالة بيمر على Business Rule (`CheckRule(...)`) مش assignment مباشر.
2. **Business Rules كـ first-class objects** — 26 rule كلاس منفصل.
3. **~76 Domain Event + 32 Value Object** — عمق modeling نادر في مشاريع كهذه.
4. **Result Pattern مفعّل بالكامل** — مفيش exceptions للأعمال، و`ApiController.HandleFailure` بيرجّع ProblemDetails.
5. **Vertical Slices منضطة** — كل feature فيها Commands/Queries/Validators/Handlers/EventHandlers بنفس الشكل.
6. **Cross-cutting عبر Interceptors** — Soft Delete + Audit + Domain Events بطريقة EF Core الصح.
7. **Auth stack كامل** — JWT + Refresh Tokens + TOTP 2FA + Blacklist + Audit Logging.
8. **Domain Event Handlers لكل حدث** — التفكير في side effects موجود من البداية.

---

## ⚠️ نقاط الضعف (مرتبة بالخطورة)

### 🔴 حرجة — بتكسر ادعاءات المشروع

| # | المشكلة | المكان | التأثير |
|---|---|---|---|
| C1 | ~~**الـ Domain مش Clean**: `Domain.csproj` فيه EF Core SqlServer + Identity + AutoMapper + MediatR~~ **✅ اتصلحت** — Domain.csproj فيه MediatR بس | `Domain/Domain.csproj` | أول reviewer محترف هيكت فورًا |
| C2 | ~~**Domain يعتمد على Application**~~ **✅ اتصلحت** — `ShipmentId` اترجع للـ Domain + كل الـ dead usings اتشالت + `DriverShiftService` اتنقل للـ Application | `Domain/Shipments/Shipment.cs` | مخالفة dependency rule صريحة |
| C3 | ~~**Architecture Tests فاضية**~~ **✅ اتصلحت** — 5 قواعد حقيقية في `Architecture.Tests/LayerDependencyTests.cs` | `Architecture.Tests/` | الـ README بيدّعي حاجة غير موجودة |
| C4 | ~~**بقايا مشروع تاني**: namespace `EducationalPlatform.Infrastructure.Services.Token`~~ **✅ اتصلحت** — TokenService → `Infrastructure.Services`، Controllers → `WebApi.Controllers`، TOTP issuer → "SLFMS" | `TokenService.cs`, `Program.cs`, `DependencyInjection.cs` | سوء سمعة فوري |
| C5 | ~~**Domain Events بتتـpublish قبل الـ commit**~~ **✅ اتصلحت** — الـ events بتتجمع في `SavingChanges` وبتتـpublish في `SavedChanges` (بعد الـ commit الناجح فقط) + فاشلة = discard؛ 5 regression tests بـ SQLite | `DomainEventInterceptor.cs` | مشكلة consistency حقيقية |
| C6 | ~~**TFMs غير متطابقة**: net8/net9 متخلطين~~ **✅ اتصلحت** — كل المشاريع net10.0 | كل الـ `.csproj` | خطر runtime conflicts |
| C7 | **الـ `.sln` بيشاور على مشروع خارجي مش موجود** | `SLFM.sln` | **✅ اتصلحت** |

### 🟠 مهمة — ثغرات فعلية

| # | المشكلة | المكان |
|---|---|---|
| M1 | Refresh token مخزّن **plain text** في جدول Users | `TokenService.cs` |
| M2 | Token Blacklist في **MemoryCache singleton** — بيضيع مع restart ومش يشتغل multi-instance | `TokenBlacklistService.cs` |
| M3 | ~~**User enumeration** في الـ login~~ **✅ اتصلحت قبل موعدها في جولة الـ auth hardening** — نفس الـ `InvalidCredentials` للإيميل المجهول والباسورد الغلط + dummy password check للـ timing | `LoginCommandHandler.cs` |
| M4 | `Shipment.Version` مش **concurrency token** — بيزيد رقم بس مفيش حماية فعلية | `Shipment.cs` + `ShipmentConfiguration.cs` |
| M5 | كل الـ **GetAll queries من غير pagination** — `_unitOfWork.Drivers.EntityQuery.Select(x => x)` | كل الـ query handlers |
| M6 | Repository/UoW مهدولة: UoW يكشف 4 aggregates بس + `DriverRepository` متشال من الـ DI بـ comment | `UnitOfWork.cs`, `DependencyInjection.cs` |
| M7 | Packages unused: `AWSSDK.Core`, `MassTransit.*`, `BCrypt` المكرر, `NetArchTest` في كل مشروع | كل الـ `.csproj` |
| M8 | README بيوصّل حط secrets في `appsettings.Development.json` — عكس best practice | `README.md` |

### 🟡 جمالية — بتهدّ سمعة الكود

| # | المشكلة | المكان |
|---|---|---|
| H1 | Typo `Presistence` في الفولدرات والـ namespaces | `Infrastructure/Presistence/*` |
| H2 | `ValidatoinBehaviour`, `CreateInoivceCommandValidator`, `IAudiatable` | Application/Common, Domain/SharedKernel |
| H3 | ملفين اختبارات مكررين: `InventoryItemTests.cs` + `InverntoryItemTests.cs` | `Domain.Tests/` |
| H4 | ملف باسم `Address .cs` (فيه space) | `Domain/Warehouse/ValueObjects/` |
| H5 | Namespace mismatch: `Domain.Inventory` vs `Domain.InventoryItems` | `AppDbContext.cs`, `InventoryItem.cs` |
| H6 | `PaymentFailedEvent` + `PaymentFailedDomainEvent` — event مكرر | `Domain/Payments/Events/` |
| H7 | `using Infrastructure.Repositories.Vehicle.Infrastructure.Repositories;` — namespace معوج من فولدرات nested | `UnitOfWork.cs` |
| H8 | `.github/workflows` فاضية — مفيش CI | `.github/` |
| H9 | `ICacheService`/`InMemoryCacheService` مش مستخدمين في أي query | `Infrastructure/Services/` |

---

## 🗺️ الـ Roadmap — 7 مراحل

> القاعدة: **كل مرحلة لازم المشروع يبقى build + tests green بعدها.** متنقلش للمرحلة اللي بعدها قبل ما الـ checklist بتاعها يخلص.

---

### 🧹 المرحلة 0 — التنظيف والتوحيد (2-3 أيام)

**الهدف:** تصفير الديون السطحية. أسهل مرحلة وأكبر تأثير بصري.

- [x] **0.0** ~~تنضيف الـ repo نفسه من الـ build artifacts~~ **✅ خلصت** — `.gitignore` كامل + `git rm -r --cached` لـ 1,323 ملف bin/obj/.vs + حذف `repomix-output.xml` (snapshot مولد قديم) — working tree بقى clean 100%
- [x] **0.1** ~~شيل كل الـ namespaces اللي فيها `EducationalPlatform`~~ **✅ خلصت** — كمان اتصلح الـ 2FA issuer name اللي كان "EducationalPlatform" في الـ authenticator app
  - [x] `Infrastructure/Services/TokenService.cs`
  - [x] `API/Program.cs`
  - [x] `Infrastructure/DependencyInjection.cs`
- [x] **0.2** ~~شيل الـ packages الغير مستخدمة~~ **✅ خلصت** — اتشالت بالفعل في commit الـ net10: `AWSSDK.Core`, `MassTransit.*`, `NetArchTest.Rules` (من غير الـ tests). **تعديل على الخطة:** `BCrypt.Net-Next` **مستخدم فعلًا** في `Infrastructure/Services/PasswordHasher.cs` (الـ IPasswordHasher المخصص) — هيفضل. اللي اتشال النهارده: `AutoMapper` من Infrastructure (صفر استخدام — التسجيل كله في API/Application)
- [x] **0.3** ~~وحّد الـ Target Framework~~ **✅ خلصت** — اتحوّلت كل المشاريع على **net10.0** (مش net9.0 زي ما كانت الخطة) في commit الـ package unification، والـ versions متطابقة (EF/JwtBearer 10.0.15)
- [x] **0.4** ~~صلّح الـ typos~~ **✅ خلصت** — `Presistence` → `Persistence` (فولدر + namespaces + 41 ملف)، `ValidatoinBehaviour` → `ValidationBehavior` (+ `LoggingBehaviour` → `LoggingBehavior`)، `CreateInoivceCommandValidator` → `CreateInvoiceCommandValidator`، `IAudiatable` → `IAuditable`، `InverntoryItemTests` اتحمّج مع `InventoryItemTests.cs` (27 اختبار مكرر → 16 شاملين بيغطوا الـ Domain Events والـ edge cases)، `Address .cs` → `Address.cs`
- [x] **0.5** ~~صلّح الـ namespaces المعوجة~~ **✅ خلصت** —
  `Infrastructure.Repositories.Vehicle.Infrastructure.Repositories` اتفك (nested namespace accidental) واتبسّط لـ `Infrastructure.Repositories` زي كل الـ repos،
  `VehicleUniqnessChecker.cs` → `VehicleUniquenessChecker.cs`،
  `Domain.InventoryItems` اتوحّد على `Domain.Inventory` (entity + interface + 27 ملف)،
  `DbSet<InsuranceClaim> insuranceClaims` → `InsuranceClaims`
- [x] **0.6** ~~الـ event المكرر `PaymentFailedDomainEvent`~~ **✅ اتصلحت** — طلع class فاضي dead مش event مكرر فعليًا: `PaymentFailedEvent` هو الحدث الحقيقي المستخدم (بيـpublish من `Payment.cs`)، والفاضي اتحذف من غير أي تعديل على publisher/handler
- [x] **0.7** ~~الـ `DbSet` الـ duplicates~~ **✅ اتحققت** — مفيش duplicates أصلًا: كل aggregate له DbSet واحد في `AppDbContext` (الـ fix الوحيد كان casing بتاع الـ claims في 0.5)
- [x] **0.8** ~~شغّل: `dotnet build` + `dotnet test`~~ **✅ بنجاح** — 0 errors + Domain.Tests 86/86 + Architecture.Tests 9/9
- [x] **0.9** ~~Commit منفصل لكل مجموعة تغييرات~~ **✅ خلصت** — كل بند ليه commit مستقل

**معيار النجاح:** مفيش أي namespace غريب، مفيش package dead، كل المشاريع **net10.0**، والـ tests خضرا. ✅ **المرحلة 0 اكتملت بالكامل**

---

### 🏛️ المرحلة 1 — Architecture Compliance (أسبوع)

**الهدف:** الـ Domain يبقى pure فعلًا، والـ NetArchTest يفرض القواعد أوتوماتيك.

> **✅ الحالة: أُنجزت — 5/5 Architecture Tests خضرا + 98/98 Domain Tests.**

- [x] **1.1** انقل الـ Value Objects اللي الـ Domain بيستخدمها من Application:
  - [x] `ShipmentId.cs` كان معرّف بـ namespace `Application.Features.Shipments.ValueObjects` رغم إنه جوه فولدر الـ Domain — اتصلح لـ `Domain.Shipments.ValueObjects`
  - [x] اتحدث 13 ملف في Domain + 13 في Application\Features\Shipment + 2 في Infrastructure + 1 في API كانوا بيعتمدوا على الـ namespace الغلط
- [x] **1.2** انقل `Domain/Settings/` (ApiSettings, EmailSettings) للـ Application:
  - [x] المكان الجديد: `Application/Common/Settings/` بنamespace `Application.Common.Settings`
  - [x] **Bonus:** اكتُشف إن الـ options مش مربوطة في أي مكان أصلًا (`Configure` غير موجودة) — الـ handlers كانت بتاخد object فاضي! اتربطت في `AddApplication` و `AddInfrastructure`
- [x] **1.3** نظّف `Domain.csproj`: باقي فيه MediatR بس (شيل EF SqlServer + Identity + AutoMapper + NetArchTest)
  - [x] اكتُشف أن API كان بياخد `AddEntityFrameworkStores` transitively من الـ Domain — أُعلن عنه صراحة في API.csproj
  - [x] اكتُشف أن `LogoutCommandHandler` كان بيستخدم extension جاي من Identity packages — اتستبدل بـ `FindFirst()?.Value`
- [x] **1.4** انقل `DriverShiftService` من `Domain/DomainServices/` للـ Application (cross-aggregate orchestration + كان بيستخدم EF Core جوه الـ Domain) — اتسجّلت في الـ DI والـ handler بتحقنها بدل `new`
- [x] **1.5** اكتب الـ **Architecture Tests الحقيقية** في `Architecture.Tests/LayerDependencyTests.cs` (5 قواعد):
  - [x] Domain ميعتمدش على Application/Infrastructure/API/EF/ASP.NET/AutoMapper (استثناء موثّق: MediatR)
  - [x] Application ميعتمدش على Infrastructure أو API
  - [x] Infrastructure ميعتمدش على API
  - [x] كل الـ Application Handlers = sealed (اتسلم 54 handler)
  - [x] كل الـ Aggregate Roots الـ concrete = sealed
- [x] **1.6** الـ `.sln` كان بيشاور على مشروع خارجي مش موجود (`Delegates and Eventw`) — اتشال
- [x] **1.7** حذف ملفات ميتة: `Domain/SharedKernel/Route.cs` (كود غير مستخدم مربوط بـ EF) + dead usings (Mono.Cecil, Microsoft.Identity.Client, EF.Infrastructure)
- [x] **1.8** نقل كل الـ persistence/service abstractions من الـ Domain للـ **Application layer** (Dependency Inversion الصح — الـ Application يعرّف احتياجاته والـ Infrastructure يطبقها):
  - [x] `IGenericRepository` + `IUnitOfWork` + `ITokenBlacklistService` اتنقلوا من `Domain/Interfaces/Repositories/` → `Application/Common/Interfaces/`
  - [x] الـ 10 repo interfaces الخاصة بالـ aggregates (`IVehicleRepository`, `IDriverRepository`, `IShiftRepository`, `IShipmentRepository`, `IUserRepository`, `IInvoiceRepository`, `IPaymentRepository`, `IInventoryItemRepository`, `IClaimRepository`, `IWarehouseRepository`) اتنقلوا من فولدرات الـ Domain → `Application/Common/Interfaces/` (كانت لازم تتحرك لأنها بتورث من `IGenericRepository<T>` — ولو فضلت في Domain هتخلّي Domain يعتمد على Application وده بيكسر قاعدة الـ dependency)
  - [x] `IEmailService`, `IIdentityService`, `IPasswordHasher`, `ITokenService` كانت فعليًا في فولدر الـ Application لكن بأسماء namespaces مضللة (`Domain.Interfaces.Services`) — اتصلحت لـ `Application.Common.Interfaces`
  - [x] **استثناء موثّق:** `IVehicleUniquenessChecker` فضل في `Domain/Vehicles` لأنه domain service الـ `Vehicle.Create` بيستهلكه مباشرة (الـ aggregate نفسه محتاجه، مش الـ Application)
  - [x] حوالي 140 ملف اتحدثوا (usings + namespaces) و `Domain/Interfaces/` اتفضى واتشال
- [x] **1.11** **تصحيح معماري بعد research** — الـ aggregate repository interfaces رجعت للـ Domain جنب aggregates بتاعتهم (أثبتّه: Microsoft Learn DDD guide، Vaughn Vernon IDDD، DDD Sample، eShopOnContainers — كلهم بيقولوا "repository interfaces live in the domain model layer"):
  - [x] الـ 10 interfaces (`IClaimRepository`, `IDriverRepository`, `IInventoryItemRepository`, `IInvoiceRepository`, `IPaymentRepository`, `IShiftRepository`, `IShipmentRepository`, `IUserRepository`, `IVehicleRepository`, `IWarehouseRepository`) رجعوا لفولدر الـ aggregate بتاعهم بنفس namespace الـ aggregate
  - [x] بقوا **self-contained** — بيعرّفوا الـ members اللي الـ handlers محتاجينها فعلًا (EntityQuery/AddAsync/Update/Delete + الـ custom methods) من غير وراثة من `IGenericRepository` (لأن Domain مش مسموح يعتمد على Application — الـ arch tests بيفرضوا كده، وده كمان أقرب لـ Vernon: الـ repository بيتكلم لغة الـ aggregate)
  - [x] `IUnitOfWork` + `IGenericRepository` فضلوا في `Application.Common.Interfaces` (الـ transaction boundary بتاع الـ use case + الـ generic persistence contract اللي الـ Infrastructure بيطبقه)
  - [x] 41 ملف consumers اتحدثوا (usings) + `InvoiceRepository.CountAsync()` اتضاف صراحة (الـ interface signature مختلفة عن الـ base)
- [x] **1.9** `IUnitOfWork` بقى **transaction boundary بس** — شيلنا منه الـ repository accessors (`Vehicles/Drivers/Shifts/Shipments/VehicleUniquenessChecker`) ووراثة `IDisposable`:
  - [x] الـ interface دلوقتي فيه `CompleteAsync` + `BeginTransactionAsync`/`CommitTransactionAsync`/`RollbackTransactionAsync` بس — مفيش أي repository منه
  - [x] الـ repos اتسجلت في الـ DI فرادى (`IDriverRepository`, `IShiftRepository`, `IShipmentRepository`, `IVehicleUniquenessChecker`) والـ handlers بتحقنها مباشرة (~33 handler اتحدثوا)
  - [x] الـ query handlers بقت تعتمد على الـ repos مباشرة من غير `IUnitOfWork` خالص — الـ commands بتحقن الاتنين
  - [x] `UnitOfWork` مش بيـdispose الـ DbContext — الـ DI scope هو اللي بيملكه (كانت غلطة: `Dispose()` كان بيقتل الـ context وهي DI scope لسه عايش)
- [x] **1.10** `Application/GlobalUsings.cs` — بديل عن تكرار نفس بلوك الـ usings في أول كل ملف (~296 ملف اتشال منها الـ usings المتكررة):
  - [x] global usings: `MediatR`, `AutoMapper`, `FluentValidation`, `Domain.SharedKernel`, `Application.Common.Interfaces`, `Microsoft.Extensions.Logging`
  - [x] كمان اتشالت الـ `using System;`/`System.Collections.Generic`/`System.Linq`/`System.Threading(.Tasks)` الـ redundant (مغطاة بـ `ImplicitUsings`) و `System.Text` من الملفات اللي مبتستخدمهاش فعليًا (5 ملفات بس محتاجينها)
  - [x] **متسابش global:** `Microsoft.EntityFrameworkCore` — الـ handlers مملوكة persistence concerns دي ديون المرحلة 3

**معيار النجاح:** `dotnet test` على Architecture.Tests بيعدّي، والـ Domain.csproj مفيهوش غير MediatR. ✅

**⚠️ ديون معروفة متبقية (مشكلة جاية):** ~55 handler في Application لسه بيستخدموا `Microsoft.EntityFrameworkCore` (عشان `Include()` في الـ queries) — دي مشكلة "Queries بتملك persistence concerns" وبتنتمي للمرحلة 3.

---

### 🔄 المرحلة 2 — Domain Events & Consistency (أسبوع)

**الهدف:** الـ events تضمن Delivery صح، والـ concurrent updates تتمنع فعلًا.

- [x] **2.1** ~~غيّر `DomainEventInterceptor`:~~ **✅ خلصت**
  - [x] `SavingChangesAsync`: بس **يجمع** الـ events ويـclearها (من غير publish)
  - [x] `SavedChangesAsync`: بعد الـ commit الناجح، اعمل publish
  - [x] لو حصل استثناء في الـ save، الـ events مينشرش أبدًا
  - [x] + sync parity (`SavingChanges`/`SavedChanges`/`SaveChangesFailed`) + stash per-context
  - [x] + 5 regression tests في `Architecture.Tests/DomainEventInterceptorTests.cs` (SQLite in-memory: نجاح → publish، فشل → مفيش side effects، الـ events تتفضى من الـ aggregates)
- [ ] **2.2** اعمل **Outbox بسيط** (ده المستوى التاني):
  - [ ] جدول `OutboxMessage` (Id, Type, Payload JSON, OccurredOnUtc, ProcessedOnUtc, Error)
  - [ ] الـ interceptor يسجل الـ events في الجدول **في نفس الـ transaction بتاع الـ save**
  - [ ] Background worker (HostedService أو MassTransit) يقرا ويـpublish ويحدّث `ProcessedOnUtc`
  - [ ] Retry + poison-message handling
- [ ] **2.3** الـ operations اللي بتلمس أكتر من aggregate (زي ProcessPayment → Invoice) لازم تكون في transaction واحدة أو event-driven saga — اختار ووثّق القرار
- [ ] **2.4** Optimistic Concurrency:
  - [ ] `ShipmentConfiguration`: خلي `Version` عمود `rowversion` حقيقي (`.IsRowVersion()`)
  - [ ] كرر نفس الشيء لأي aggregate بيحتاجها (Payment, Invoice)
  - [ ] اعمل `DbUpdateConcurrencyException` handling في ExceptionHandlingMiddleware → ترجّع **409 Conflict**
- [ ] **2.5** Idempotency للـ Payments: ProcessPayment يمنع الدفع المكرر بنفس الـ transaction reference

**معيار النجاح:** لو الـ DB commit فشل، مفيش أي event handler اتنفذ. ولو حد عدّل Shipment قبل إنتك، بتشوف 409.

---

### 📑 المرحلة 3 — Query Side & Performance (أسبوع)

**الهدف:** الـ queries تبقى قابلة للتوسع (pagination/filter/sort) والـ caching يشتغل فعلًا.

- [ ] **3.1** اعمل `PagedResult<T>` في `Domain/SharedKernel/` (Items, Page, PageSize, TotalCount, TotalPages)
- [ ] **3.2** طبّق pagination على **كل** الـ `GetAll*` queries (Drivers, Shipments, Vehicles, Inventory, Warehouses, Invoices, Claims, Shifts, Users, Payments):
  - [ ] الـ Query record ياخد `Page`, `PageSize` (+ validation: PageSize ≤ 100)
  - [ ] الـ Handler يبني query بمصفوفة `Where` اختيارية + `OrderBy` + `Skip/Take`
- [ ] **3.3** Filters:
  - [ ] `GetAllShipmentsQuery` بـ status + priority + date range + tracking search
  - [ ] `GetAllDriversQuery` بـ status
  - [ ] `GetAllInvoicesQuery` بـ status + payer
- [ ] **3.4** استخدم الـ `ICacheService` الموجود فعليًا:
  - [ ] Decorate الـ GetById queries (cache-aside pattern، TTL قصير)
  - [ ] Invalidate من الـ Event Handlers الموجودة أصلًا (`ShipmentDeliveredEvent` → شيل الـ cache بتاع الـ shipment ده)
  - [ ] اختيار ذكي: الـ queries اللي بستدعى كتير فقط (GetById, GetAllShipments)
- [ ] **3.5** راجع الـ Include strategies: الـ GetById queries اللي بتـinclude collections — اتأكد إنها بترجّع DTO بـ projection مش تحمّل aggregate كامل للقراءة
- [ ] **3.6** حط **Indexes** في الـ EF Configurations على أعمدة البحث (`TrackingNumber`, `Status`, `CreatedAt`, `SenderId`)

**معيار النجاح:** مفيش endpoint راجّع unbounded list، والـ logs بتبين cache hits.

---

### 🔐 المرحلة 4 — Security Hardening (أسبوع)

**الهدف:** سد الثغرات الفعلية.

- [ ] **4.1** Hash الـ refresh token قبل تخزينه (SHA256 أو BCrypt) — الـ lookup يبقى بالـ hash
- [ ] **4.2** Refresh token rotation مع **reuse detection**: لو refresh token مستخدم قبل كده اتقدم تاني → اعمل invalidate لكل جلسات المستخدم
- [ ] **4.3** سجّل الجلسات: جدول RefreshTokens (بدل عمود واحد على الـ User) عشان multi-device login
- [ ] **4.4** Blacklist حقيقي:
  - [ ] اختيار 1 (سهل): جدول `RevokedTokens` في الـ DB مع cleanup job
  - [ ] اختيار 2 (متبقى للمرحلة 7): Redis
- [x] **4.5** ~~صلّح الـ user enumeration في LoginCommandHandler~~ **✅ خلصت مبكرًا في جولة الـ auth hardening** (commit 3b27fee):
  - [x] نفس الـ رسالة (InvalidCredentials) في الحالتين
  - [x] نفس الـ timing تقريبًا (dummy password check لو الإيميل مش موجود)
- [ ] **4.6** Rate limiting على الـ auth endpoints (استخدم `Microsoft.AspNetCore.RateLimiting` المدمج في .NET 9) — خصوصًا login + forgot-password
- [ ] **4.7** الـ README: غير تعليمة "حط secrets في appsettings" لـ User Secrets (dev) + Environment Variables (prod)
- [ ] **4.8** راجع الـ JWT:
  - [ ] SecretKey من غير default (throw لو مفقود)
  - [ ] `AccessTokenMinutes` من options مش hard-coded
  - [ ] ضيف `aud`/`iss` validation strict (موجودة) + `requireExpirationTime`
- [ ] **4.9** سجّل Audit للـ IP في failed logins (موجود) + ضيف lockout after N attempts (Identity فيه `options.Lockout` — فعّله)
- [ ] **4.10** DataProtection keys: خزّنها في persistent location/Redis في الإنتاج

**معيار النجاح:** الكراكيست اللي بيلعب في الـ login مش يعرف يفرّق بين الإيميل الغلط والباسورد الغلط، والـ tokens مش مفهومة من الـ DB dump.

---

### 🧪 المرحلة 5 — Testing Strategy (أسبوعين)

**الهدف:** تغطي كويس، وتكتشف الـ regressions قبل المراجعين.

- [ ] **5.1** نطّف الـ Domain.Tests (من المرحلة 0) ووحدّه في فولدر مرتب لكل aggregate
- [ ] **5.2** زوّد الـ Domain tests للـ aggregates الضعيفة (ShiftTests فيه 2 tests بس):
  - [ ] Shift: cancel, complete, overlap rules, invalid transitions
  - [ ] Shipment: كامل الـ lifecycle + الـ 7 rules الموجودة
  - [ ] User: ~~lockout, 2FA rules, password reuse~~ **✅ مغطاة** (`Domain.Tests/Users/UserTests.cs` + `Application.Tests` فحص lockout و2FA وreuse على مستوى الـ handlers بعد جولة الـ auth)
  - [ ] Invoice + Payment + Claim: كل rule
- [ ] **5.3** الـ Rule classes: test لكل rule (IsBroken true/false cases)
- [ ] **5.4** **Integration tests** بـ `WebApplicationFactory` + Testcontainers (SQL Server):
  - [ ] مشروع جديد: `Integration.Tests`
  - [ ] Full flow: Register → Confirm Email → Login → Refresh → 2FA Setup → Verify
  - [ ] Shipment lifecycle: Create → AddPackage → AssignCarrier → Dispatch → AddRoutePoint → Deliver
  - [ ] Payment flow: Pay → Refund → Failure handling
  - [ ] Concurrency: محاولتين يعدلوا نفس الـ shipment → واحدة تنجح وواحدة 409
  - [ ] Pagination: seed 50 record → verify paging
- [ ] **5.5** Test الـ Pipeline Behaviors (Validation + Logging) — request invalid → 400 من غير ما يوصل للـ handler
- [ **5.6** ] Coverage: target 60%+ Domain, 40%+ Application handlers. استخدم `coverlet` (موجود)
- [ ] **5.7** اعمل `dotnet test` يشتغل في CI (المرحلة 6)

**معيار النجاح:** الـ CI يفشل لو حاجة اتبCrakt — مش بس الـ domain tests.

---

### 🛠️ المرحلة 6 — DevEx & Ops (أسبوع)

**الهدف:** المشروع يشغّل نفسه ويـreveal نفسه.

- [ ] **6.1** GitHub Actions CI (`.github/workflows/ci.yml`):
  - [ ] Trigger: push + PR
  - [ ] Steps: restore → build → `dotnet test` (كل المشاريع) → publish artifacts
  - [ ] Matrix على net9.0
  - [ ] Badge في الـ README
- [ ] **6.2** Docker:
  - [ ] `API/Dockerfile` (multi-stage, non-root user)
  - [ ] `docker-compose.yml`: API + SQL Server + Redis
  - [ ] الـ migrations تشتغل على startup (Migrate من Program في بيئة الحاوية بس)
- [ ] **6.3** Health Checks:
  - [ ] `/health` (DbContext ping)
  - [ ] `/health/ready` + `/health/live` منفصلين
  - [ ] HealthChecks.UI (اختياري)
- [ ] **6.4** Serilog:
  - [ ] استبدل logging الافتراضي بـ Serilog (Console sink + File sink rolling)
  - [ ] Request logging + enrichers (CorrelationId, MachineName)
  - [ ] كل الـ handlers بـ log structured (موجودة في LoggingBehaviour — حسّنها)
- [ ] **6.5** `Microsoft.AspNetCore.OpenApi` أو Swagger improvements:
  - [ ] XML comments + examples
  - [ ] API Versioning ( Asp.Versioning.Http )
- [ ] **6.6** `.editorconfig` + analyzers:
  - [ ] `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
  - [ ] Nullable = enable في كل المشاريع
  - [ ] Code analysis level = latest
- [ ] **6.7** README update نهائي — بيدّعي الـ features الحقيقية بس

**معيار النجاح:** `docker compose up` يرفع المشروع كامل، وbadge الـ CI أخضر في الـ README.

---

### 🚀 المرحلة 7 — Depth (اختيارية — أسبوعين+)

**الهدف:** تزيد في المشروع من ناحية الـ domain نفسه مش بس البنية.

- [ ] **7.1** Redis distributed cache بدل in-memory (ICacheService implementation تانية)
- [ ] **7.2** MassTransit + RabbitMQ بدل الـ in-process MediatR dispatch للـ integration events (packages موجودة أصلاً)
- [ ] **7.3** Background jobs (Hangfire أو Quartz):
  - [ ] Inventory reorder monitoring
  - [ ] Maintenance due reminders
  - [ ] Refresh token cleanup
  - [ ] Expired claim auto-close
- [ ] **7.4** Real-time tracking بـ SignalR (Shipment tracking updates في الـ moment)
- [ ] **7.5** Domain features إضافية:
  - [ ] Trip aggregate (رحلة كاملة بين Shifts و Shipments)
  - [ ] Route optimization service
  - [ ] Driver-shipment assignment مع capacity constraints
- [ ] **7.6** Reporting queries (read-optimized): fleet utilization, revenue by period, delivery success rate
- [ ] **7.7** Real payment gateway abstraction (Stripe Test mode) بدل MockPaymentGateway
- [ ] **7.8** OpenTelemetry + Jaeger/Grafana للـ tracing

---

## ✅ Checklist سريع (تسلسل تنفيذ كامل)

```
Week 1:  المرحلة 0 (تنظيف)  ←  ⏱️ 2-3 أيام  →  build + tests green
Week 2:  المرحلة 1 (Architecture)  ←  ⏱️ أسبوع  →  Domain pure + Arch tests شغالة
Week 3:  المرحلة 2 (Events + Concurrency)  ←  ⏱️ أسبوع  →  Outbox + rowversion
Week 4:  المرحلة 3 (Queries + Caching)  ←  ⏱️ أسبوع  →  Pagination كامل + cache
Week 5:  المرحلة 4 (Security)  ←  ⏱️ أسبوع  →  refresh hashing + rate limit
Week 6-7: المرحلة 5 (Testing)  ←  ⏱️ أسبوعين  →  Integration tests + coverage
Week 8:  المرحلة 6 (DevEx/Ops)  ←  ⏱️ أسبوع  →  CI + Docker + Health + Serilog
بعد كده: المرحلة 7 (حسب الوقت)
```

---

## 🎯 ترتيب الأولوية لو وقتك ضيق

1. **المرحلة 0** (تنظيف) — 2-3 أيام، أعظم تغيير في أول انطباع
2. **المرحلة 1** (Architecture tests + Domain pure) — أسبوع، أعظم مبيعًا في الـ interviews
3. **المرحلة 6.1** (GitHub Actions CI) — يوم واحد بس، ودليل حي إن المشروع محترف
4. **المرحلة 5** (Integration tests) — أسبوعين، بتفرق كتير في تقييم الجودة
5. **المرحلة 2** (Outbox + concurrency) — أسبوع، العمق التقني الحقيقي
6. الباقي بالترتيب

---

## 📐 تعريف "خلصت"

المشروع يبقى Production-Grade لما:

- [ ] الـ Domain مية بالمية clean (مفيش dependency على أي framework خارجي غير MediatR)
- [ ] Architecture Tests بتفرض القواعد وبتفشل لو حد خرقها
- [ ] كل query paginated + indexed
- [ ] Domain events atomic مع الـ save (Outbox)
- [ ] Optimistic concurrency شغال وبيرجع 409
- [ ] Refresh tokens hashed + reuse detection
- [ ] Rate limiting على auth
- [ ] Integration tests بتغطي الـ flows الأساسية
- [ ] CI أخضر + Docker compose بيشغل المشروع بأمر واحد
- [ ] Health checks + structured logging
- [ ] الـ README كلها مطابقة للواقع

---

# 🔍 Scanning & Enhancement Roadmap (منفصلة عن خطة التطوير)

> خطة موازية: لكل موديول branch مستقل باسم `<module>-enhance`، وفحص كامل senior-level من الـ Domain للـ Endpoints والـ Tests، مع بحث في المصادر الموثوقة، والتسليم **Pull Request** فيه خطة الملفات المحدثة + الأسباب + الـ resources + شرح للمذاكرة. النظام يفضل **Monolith + Clean Architecture** بدون تغييرات معمارية.

| # | الموديول | الحالة |
|---|---|---|
| 1 | **Vehicle** (Domain → Application → Infrastructure → API → Tests) | ✅ **خلصت** — branch `vehicle-enhance` + PR |
| 2 | **Driver** (Domain → Application → Infrastructure → API → Tests) | ✅ **خلصت** — branch `driver-enhance` + PR |
| 3 | **Shift** (Domain → Application → Infrastructure → API → Tests) | ✅ **خلصت** — branch `shift-enhance` + PR |
| 4 | Shipment (+ Package) | ⏳ |
| 5 | User/Security (مراجعة ثانية بعد جولة الـ auth) | ⏳ |
| 6 | Inventory / Warehouse | ⏳ |
| 7 | Claim / Invoice / Payment | ⏳ |
| 8 | Audit / Notifications | ⏳ |

> ملاحظة: جولة الـ Vehicle عالجت جزء من W1 (CS0108 في أحداث Vehicle + الـ audit props المكررة في `Vehicle` + تحويل `Entity<TId>` setters لـ protected)، وجولة الـ Driver كمّلت نفس النمط (أحداث Driver بقت payload properties مش `Id` + state machine واضح + Terminate)، وجولة الـ Shift قفلت ملف أحداث Shift كمان (كل event بقى `{ get; init; }` وبـ payload كامل). باقي W1 (`ValueObject.Equals` CS8765 + User/Shipment shadows) هيُعالج في جولات الموديولات الخاصة بيها أو في W1 المباشر لو سبقناها.

### جولة Driver — ملخص اللي اتعمل
- **Domain:** `DriverStatus` بقى فيه الحالات الموصولة بس (`Active/Suspended/Terminated`)، `Terminate()` نهائي وبيحرر الـ shift، `ClearShift()` بيرجع `Result`، قواعد `Suspend/Reactivate` بقت محترمة، `Hire/UpdateLicense` بيتحققوا من الـ license + التفرد عبر `IDriverUniquenessChecker`، والـ rating بيرفض `NaN/Infinity`، والـ events كلها PascalCase ومش باسم `Id` (حل CS0108).
- **Application:** كل الـ handlers بترجع `Result`، أخطاء موحدة من `DriverErrors`، use cases جديدة (`Terminate`, `ClearShift`, `AssignShift` موصول)، `DriverShiftService` بقى بيكلم `driver.AssignShift`، الـ DTO اتسطح لـ primitives، والـ `DriverProfile` mapping صريح (كان بيكسر GET at runtime).
- **Infrastructure:** `DriverConfiguration` بقى في namespace منظم بأطوال مطابقة للـ domain + query filter + index على `Status`، `DriverRepository.GetByIdAsync` tracked، و`DriverUniquenessChecker` جديد، وميجريشن `AlignDriverColumnLengths`.
- **API:** `DriversController` واحد على `api/drivers`، الـ id من الـ route، 201 Created عند الـ hire، وvalidators/behaviors اتفعّلوا لأول مرة + `ValidationException` بترجع 400.
- **Tests:** Domain (state machine + VO bounds + NaN + event naming guard) وApplication (handlers + mapping + cross-aggregate shift service).

### جولة Shift — ملخص اللي اتعمل
- **Domain:** اتشال الـ enum المكرر (`ShiftStatus` كان متعرّف مرتين، واحد جوه `Shift.cs` وواحد في `Enums/`)، الـ `ShiftId` بقى `readonly record struct` بـ `New()`/`IsEmpty`، والـ aggregate بقى state machine كامل (`Planned → Active → Completed` / `Planned → Cancelled` / `Active → Aborted`) مع `Reschedule` للشيفتات المخططة، ورفض النوافذ المقلوبة/الأطول من 24 ساعة، وتوحيد كل التواريخ على UTC، وتسجيل `StartedAtUtc`/`EndedAtUtc` الفعليين، وأحداث كاملة الـ payload (مش باسم `Id`).
- **Application:** كل الـ handlers بترجع `Result` (بدل `throw new Exception`) وبتقرأ من `GetByIdAsync` tracked، و`CompleteShift` بقى بينادي `Complete()` فعلًا (كان بينادي `Cancel()`!) والملف اللي كان اسمه غلط اتصلح، وuse cases جديدة (`Abort`, `Reschedule`)، و`CreateShift` بقى بيمر على `IDriverShiftService` فبقى فيه مسار إنشاء واحد بيتحقق من الـ driver والـ overlap، والـ queries اتصلحت (كانت بتعمل `Map<Result<...>>` على `IQueryable` من غير `await`)، و`ShiftClosedEventHandler` جديد بيحرر `CurrentShiftId` بتاع السواق عند Complete/Cancel/Abort.
- **Infrastructure:** `ShiftConfiguration` اتنقلت لـ `Configurations/Shift` بأطوال أعمدة مطابقة + query filter + `ValueGeneratedNever` + indexes على `(DriverId, Status)` و`(DriverId, ShiftStart)`، `ShiftRepository` اتنقل لـ `Repositories/Shift` وبقى فيه `GetByIdAsync` tracked و`ListAsync` مفلترة وoverlap بيستثني الـ Cancelled/Aborted والشيفت نفسه، وميجريشن `AlignShiftSchema` (أعمدة `StartedAtUtc`/`EndedAtUtc` + index جديد).
- **API:** `ShiftsController` على `api/shifts`، الـ id من الـ route بـ `Guid` (الـ model binder ماكانش يعرف يبني `ShiftId`)، endpoints جديدة `abort` و`PUT /{id}` للـ reschedule، و`GET /` بفلاتر `driverId` و`status`.
- **Tests:** Domain (كل انتقالات الـ state machine + النوافذ + UTC + الـ events + `ShiftId`)، وApplication (handlers لكل use case + overlap + queries + mapping + event handler بيحرر السواق).

## منهجية الفحص لكل موديول

1. **Domain:** الـ aggregate invariants، الـ Rules مستخدمة فعلًا، الـ Value Objects Result-pattern، الـ Events ليها publishers/handlers حقيقيين وpayload naming سليم.
2. **Application:** الـ handlers بتفحص كل Result قبل `.Value`، أخطاء موحدة من الـ Errors classes، الـ use cases الناقصة مكتملة، الـ validators متوافقة مع الـ domain.
3. **Infrastructure:** الـ EF mapping متوافقة مع الـ domain (أطوال أعمدة/converters/owned types)، مفيش state مكرر.
4. **API:** الـ contracts مربوطة بـ primitives مش Domain VOs، الـ endpoints كاملة.
5. **Tests:** Domain tests لكل invariant + handler tests بـ fakes لكل use case.
6. **Resources مرجعية:** Microsoft Learn (EF Core owned entities / DDD patterns)، eShopOnContainers (aggregate encapsulation + rules pattern)، Milan Jovanović (owned types + aggregate design).
