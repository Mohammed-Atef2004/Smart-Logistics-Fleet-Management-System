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
| C4 | **بقايا مشروع تاني**: namespace `EducationalPlatform.Infrastructure.Services.Token` | `TokenService.cs`, `Program.cs`, `DependencyInjection.cs` | سوء سمعة فوري |
| C5 | **Domain Events بتتـpublish قبل الـ commit** — لو الـ save فشل، الـ side effects حصلت أصلاً | `DomainEventInterceptor.cs` | مشكلة consistency حقيقية |
| C6 | ~~**TFMs غير متطابقة**: net8/net9 متخلطين~~ **✅ اتصلحت** — كل المشاريع net10.0 | كل الـ `.csproj` | خطر runtime conflicts |
| C7 | **الـ `.sln` بيشاور على مشروع خارجي مش موجود** | `SLFM.sln` | **✅ اتصلحت** |

### 🟠 مهمة — ثغرات فعلية

| # | المشكلة | المكان |
|---|---|---|
| M1 | Refresh token مخزّن **plain text** في جدول Users | `TokenService.cs` |
| M2 | Token Blacklist في **MemoryCache singleton** — بيضيع مع restart ومش يشتغل multi-instance | `TokenBlacklistService.cs` |
| M3 | **User enumeration** في الـ login: NotFound للإيميل الغلط، InvalidCredentials للباسورد الغلط | `LoginCommandHandler.cs` |
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

- [ ] **0.1** شيل كل الـ namespaces اللي فيها `EducationalPlatform` (استبدلها بـ `Infrastructure.Services`):
  - [ ] `Infrastructure/Services/TokenService.cs`
  - [ ] `API/Program.cs`
  - [ ] `Infrastructure/DependencyInjection.cs`
- [ ] **0.2** شيل الـ packages الغير مستخدمة من كل الـ `.csproj`:
  - [ ] `AWSSDK.Core` من Application
  - [ ] `MassTransit.EntityFrameworkCore` + `MassTransit.RabbitMQ` من Infrastructure (هترجعها في المرحلة 7 لو محتاجها)
  - [ ] `BCrypt.Net-Next` من API (Identity بيعمل الـ hashing)
  - [ ] `NetArchTest.Rules` من Domain + Application + Infrastructure (هتبقى في Architecture.Tests بس)
- [ ] **0.3** وحّد الـ Target Framework على **net9.0** في كل المشاريع + طابق الـ versions (JwtBearer 9.x بدل 8.0.0)
- [ ] **0.4** صلّح الـ typos (استخدم rename بالـ IDE مش يدوي):
  - [ ] `Presistence` → `Persistence` (فولدرات + namespaces + usings)
  - [ ] `ValidatoinBehaviour` → `ValidationBehavior`
  - [ ] `CreateInoivceCommandValidator` → `CreateInvoiceCommandValidator`
  - [ ] `IAudiatable` → `IAuditable`
  - [ ] `InverntoryItemTests` → دمجه مع `InventoryItemTests.cs` (بعد مقارنة المحتوى واختيار الأفضل)
  - [ ] `Address .cs` → `Address.cs` (شيل الـ space من اسم الملف)
- [ ] **0.5** صلّح الـ namespaces المعوجة:
  - [ ] `Infrastructure.Repositories.Vehicle.Infrastructure.Repositories` → namespace سليم
  - [ ] وحّد `Domain.Inventory` vs `Domain.InventoryItems` على واحد
  - [ ] `DbSet<InsuranceClaim> insuranceClaims` → `InsuranceClaims`
- [ ] **0.6** احذف الـ event المكرر `PaymentFailedDomainEvent` (خلي واحد بس) + عدّل الـ publisher والـ handler
- [ ] **0.7** شيل الـ `DbSet<Invoice>` الـ duplicates من `AppDbContext` لو فيه، واتأكد إن كل aggregate له DbSet واحد
- [ ] **0.8** شغّل: `dotnet build` + `dotnet test` — لازم كله ينجح
- [ ] **0.9** Commit منفصل لكل مجموعة تغييرات (clean history بيبيع)

**معيار النجاح:** مفيش أي namespace غريب، مفيش package dead، كل المشاريع net9.0، والـ tests خضرا.

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

**معيار النجاح:** `dotnet test` على Architecture.Tests بيعدّي، والـ Domain.csproj مفيهوش غير MediatR. ✅

**⚠️ ديون معروفة متبقية (مشكلة جاية):** ~55 handler في Application لسه بيستخدموا `Microsoft.EntityFrameworkCore` (عشان `Include()` في الـ queries) — دي مشكلة "Queries بتملك persistence concerns" وبتنتمي للمرحلة 3.

---

### 🔄 المرحلة 2 — Domain Events & Consistency (أسبوع)

**الهدف:** الـ events تضمن Delivery صح، والـ concurrent updates تتمنع فعلًا.

- [ ] **2.1** غيّر `DomainEventInterceptor`:
  - [ ] `SavingChangesAsync`: بس **يجمع** الـ events ويـclearها (من غير publish)
  - [ ] `SavedChangesAsync`: بعد الـ commit الناجح، اعمل publish
  - [ ] لو حصل استثناء في الـ save، الـ events مينشرش أبدًا
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
- [ ] **4.5** صلّح الـ user enumeration في LoginCommandHandler:
  - [ ] نفس الـ رسالة (InvalidCredentials) في الحالتين
  - [ ] نفس الـ timing تقريبًا (dummy password check لو الإيميل مش موجود)
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
  - [ ] User: lockout, 2FA rules, password reuse
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
