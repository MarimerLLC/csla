# Issue #4828 — Remove all legacy data portal operation method support

**Branch:** `issue-4828/remove-legacy-operation-methods` (from `origin/main`, which is CSLA 11.0.0-alpha)
**Issue:** https://github.com/MarimerLLC/csla/issues/4828
**Public plan (issue comment):** https://github.com/MarimerLLC/csla/issues/4828#issuecomment-5689040061
**Related:** #4878 (suppressors — do after this), #4925 (open PR, explicit operation calls via generators — see "Sequencing"), #4616 / #4595 (added `UseLegacyOperationMethods`), #4090 (base-class private operation filtering), #4359 (closed by #4925)

## Context

The data portal still finds operation methods by name (`DataPortal_Create`, `Child_Fetch`, …)
when no attributed method exists. This is controlled by `DataPortalOptions.UseLegacyOperationMethods`
(default `true`). Name matching has caused real bugs (#4595: an unrelated `DataPortal_Execute`
got invoked). `main` is now 11.0, so this is the time for the breaking change.

**Decided:**
- No deprecation step in 10.x. No `[Obsolete]` on `v10.x`. Remove outright in 11.0.
- CSLA0014 (`DoesOperationHaveAttributeAnalyzer`, currently `Info`) stays as the main upgrade aid.
  It gets repurposed as a Warning ("this method will never be called"), and its add-attribute code fix stays.
- `DataPortal_OnDataPortalInvoke*`, `DataPortal_OnDataPortalException`, and `Child_On*` hooks are
  **out of scope**. They're overridable virtuals exposed via `IDataPortalTarget`, not name-matched operations.

## Sequencing with open PR #4925

PR #4925 ("Call data portal operation methods explicitly via bundled source generators", branch
`feature/explicit-operation-method-calls`) was still under review with changes requested when this plan was written.

- **No overlap:** `ServiceProviderMethodCaller.cs`, `DataPortalOptions.cs`, the legacy-name test files. Safe to do now.
- **Trivial textual overlap:** #4925 adds `partial` to the class declaration line of `CommandBase`,
  `ReadOnlyBase`, `ReadOnlyListBase`, `ReadOnlyBindingListBase`, `NameValueListBase`, `DynamicListBase`,
  `DynamicBindingListBase`, `BusinessListBase`, `BusinessBindingListBase`, `BusinessDocumentBase`.
  It also appends to `releasenotes.md`, `AnalyzerReleases.Unshipped.md`, analyzer `Constants.cs`, and `Resources.resx`.
- **Semantic overlap (the reason to wait):** #4925's generator emits a nested `IDataPortalOperations`
  interface plus named dispatch for **attributed** operation methods in `partial` classes, abstract
  base classes included. If Phase 2 below adds attributes to base-class methods, the generator will
  start generating for them inside `Csla.dll`. Implement Phase 2 against #4925's final code.
- **Already aligned:** #4925's new analyzers (CSLA0024/0025) detect operations by attribute only. Its test
  `TypeWithOperationsShouldBePartialAnalyzerTests` confirms an attribute-less `DataPortal_Fetch` produces no diagnostic.

### Order of work
1. **Stage A — now, on a branch from `origin/main`:** Phase 1 (runtime removal), Phase 4 (test
   conversions), Phase 5 (docs draft), and write down the Phase 2 decisions (ask Rocky).
   CI is expected to fail until Phase 2 is done, so don't open the PR yet.
2. **Stage B — after #4925 merges:** `git fetch`, rebase onto `origin/main`, resolve the trivial conflicts,
   implement Phase 2 against the partial base classes plus the generator, then Phase 3 (analyzers). Run the
   full build and tests, then open the PR.

Before starting either stage: `git fetch`, check `gh pr view 4925 --json state,mergedAt`, and check
`gh pr list` for other new PRs touching these files. Rocky works across multiple computers.

---

## Phase 1 — Runtime removal (Stage A)

`Source/Csla/Reflection/ServiceProviderMethodCaller.cs` (line numbers as of `origin/main` @ 408c05eef)
- Line ~122: remove `var useLegacyMethods = cslaOptions.DataPortalOptions.UseLegacyOperationMethods;`
  (and `cslaOptions` if it's now unused).
- Line ~137: drop `useLegacyMethods` from the `GetCacheKeyName(...)` call.
- Lines ~199–213: delete the "if no attribute-based methods found, look for legacy methods" block.
- Line ~440: remove the `useLegacyMethods` parameter and the `|nolegacy` suffix from `GetCacheKeyName`.
- Lines ~170 and ~179, ObjectFactory path: hard-coded `m.Name == "Child_Create"` for `CreateChildAttribute`.
  **Leave in place for Stage A.** It's a Phase 2 decision (see D3).

`Source/Csla/Configuration/Fluent/DataPortalOptions.cs`
- Line ~63–68: remove the `UseLegacyOperationMethods` property and its XML doc.

grep `origin/main` for any other `UseLegacyOperationMethods` usage (docs, samples, tests) and remove it.

## Phase 2 — Base classes that rely on name matching (Stage B; decide in Stage A)

These CSLA base-class methods have **no operation attribute**, so they're reached only through the legacy fallback.

### D1. "Not supported" stubs (private, throw `NotSupportedException` with a friendly resource message)
| Class | Methods without attribute |
|---|---|
| `CommandBase` | `DataPortal_Create(object)`, `DataPortal_Fetch(object)`, `DataPortal_Update()`, `DataPortal_Delete(object)` |
| `ReadOnlyBase` | `DataPortal_Create(object)`, `DataPortal_Update()` (`DataPortal_Delete` already has `[Delete]`) |
| `ReadOnlyListBase`, `ReadOnlyBindingListBase`, `NameValueListBase`, `DynamicListBase`, `DynamicBindingListBase` | `DataPortal_Update()`, `DataPortal_Delete(object)` |

Options:
- (a) Delete them and accept a generic "method not found" error.
- (b) Keep the friendly message by adding the attribute. Because of #4090, **private** attributed methods on a base
  class are skipped (`level < 0` → `!m.IsPrivate`), so they'd also need to become `protected`/`private protected`.
  Check the scoring so a stub never wins over a user method, and check what #4925's generator emits for them.

Either way, remove the stale `SuppressMessage` attributes (CA1801/CA1811/CA1822) on them.
**Decision: TBD.**

### D2. Default child create
- `Core/BusinessBase.Child_Create()` (protected virtual, ~line 1323) calls `BusinessRules.CheckRules()`. Today
  it runs for any child with no `[CreateChild]` method. **Removing the fallback silently skips the initial rule check.**
- `BusinessListBase.Child_Create()` and `BusinessBindingListBase.Child_Create()`: no-op defaults.

Options:
- (a) Mark them `[CreateChild]`, and verify overload scoring against user `[CreateChild]` overloads plus #4925 generation.
- (b) Remove them and document that children need their own `[CreateChild]` that calls `CheckRules()`.
- (c) Have `ChildDataPortal` fall back to `CheckRules()` itself when no create-child method exists.

**Decision: TBD.**

### D3. ObjectFactory child create
`ServiceProviderMethodCaller` ~lines 170/179 look for a method named `Child_Create` on the factory type,
then on the target type.

Options:
- (a) Look for `[CreateChild]` on the target type.
- (b) Add `CreateChildMethodName` to `ObjectFactoryAttribute` to match `CreateMethodName` etc.
- (c) Keep the factory-side name (factories are name-based by design) and only drop the target-type `Child_Create` fallback.

**Decision: TBD.**

### D4. Sync `Child_Update(params object?[])`
On `BusinessListBase`, `BusinessBindingListBase`, `BusinessDocumentBase`. The async `Child_UpdateAsync` already has
`[UpdateChild]`, so the sync versions just become ordinary protected helpers.
Confirm, and decide whether to leave, rename, or obsolete them.
**Decision: TBD.**

### Also in Phase 2
Fix XML doc comments that mention `DataPortal_XYZ`: `CommandBase` (~33–39), `Core/BusinessBase`
(~243, 267, 271), `DynamicListBase` (~32), `DynamicBindingListBase` (~30), `IDataPortal` (~90), `IDataPortalT` (~93).

## Phase 3 — Analyzers (Stage B)

`Source/Csla.Analyzers/Csla.Analyzers/`
- `Extensions/IMethodSymbolExtensions.cs`: remove the `byNamingConvention` checks in `IsRootDataPortalOperation` /
  `IsChildDataPortalOperation`.
- `Extensions/DataPortalOperationQualification.cs`: drop `ByNamingConvention`, or collapse the type to a bool.
  Update `Deconstruct` callers.
- `CslaMemberConstants.cs` ~79–127: remove the `DataPortal_*` / `Child_*` operation constants. They're still used by
  `DoesOperationHaveAttributeAddAttributeCodeFix.cs`, so keep what CSLA0014 needs, or move it there.
- Review every consumer of the qualification:
  - `DoesChildOperationHaveRunLocalAnalyzer`
  - `DoesOperationHaveAttributeAnalyzer`
  - `EvaluateOperationAttributeUsageAnalyzer`
  - `FindOperationsWithIncorrectReturnTypesAnalyzer`
  - `FindOperationsWithNonSerializableArgumentsAnalyzer`
  - `FindRefAndOutParametersInOperationsAnalyzer`
  - `IsOperationMethodPublicAnalyzer`
  - `ObjectAuthorizationRulesAttributeAnalyzer`
  - `ITypeSymbolExtensions`
- **CSLA0014:** keep the legacy-name detection *only here*. Severity `Info` → `Warning`. New message: the method
  looks like a legacy operation but will never be invoked; add the operation attribute. Keep the code fix.
  Update `docs/analyzers/CSLA0014-DoesOperationHaveAttributeAnalyzer.md`, which still says
  "For now (CSLA version 5), this is only an informational analyzer".
- `AddObjectAuthorizationRules` naming convention (`IsAddObjectAuthorizationRulesOperation`): **out of scope unless
  Rocky says otherwise.** It's a separate static-method convention, and `AuthorizationRuleManager` binds it by name.
- `AnalyzerReleases.Unshipped.md`: record the CSLA0014 severity change.
- Then #4878 (suppressors for IDE0051 / CA1822 / IDE0060 / IDE0058) can build on the attribute-only check.

## Phase 4 — Tests (Stage A, finish in Stage B)

About 150 files use legacy names: `Csla.test` ~123, `csla.netcore.test` ~15, `Csla.Web.Mvc.Test` 5, `Csla.Blazor.Test` 1,
`Csla.Ios.Test` 1, `Csla.Analyzers.IntegrationTests` 6, `Csla.Analyzers.Tests` 2.
Find them with:
`git grep -l -E "DataPortal_(Create|Fetch|Insert|Update|Delete|DeleteSelf|Execute)\b|Child_(Create|Fetch|Insert|Update|DeleteSelf|Execute)\b" -- Source/tests`

- Add the matching attribute to every legacy-named test operation that lacks one:

  | Legacy name | Attribute |
  |---|---|
  | `DataPortal_Create` | `[Create]` |
  | `DataPortal_Fetch` | `[Fetch]` |
  | `DataPortal_Insert` | `[Insert]` |
  | `DataPortal_Update` | `[Update]` |
  | `DataPortal_Delete` | `[Delete]` |
  | `DataPortal_DeleteSelf` | `[DeleteSelf]` |
  | `DataPortal_Execute` | `[Execute]` |
  | `Child_Create` | `[CreateChild]` |
  | `Child_Fetch` | `[FetchChild]` |
  | `Child_Insert` | `[InsertChild]` |
  | `Child_Update` | `[UpdateChild]` |
  | `Child_DeleteSelf` | `[DeleteSelfChild]` |
  | `Child_Execute` | `[ExecuteChild]` |

  Don't rename methods; keep the diff mechanical. Skip methods that already have an attribute, and skip `override`s of
  base-class virtuals (e.g. `DataPortal_OnDataPortalInvoke`, which is out of scope).
  Watch for `Async` suffixes (`DataPortal_FetchAsync`, `Child_UpdateAsync`), which the regex above doesn't catch:
  grep separately.
- Ignore the Silverlight/Fakes duplicate folders only if they aren't compiled (check `Csla.Tests.csproj` includes).
- `csla.netcore.test/DataPortal/ServiceProviderMethodCallerTests.cs`:
  - Delete `FindLegacyMethod_DefaultEnabled_FindsLegacyFallback` (~516), `FindLegacyMethod_Disabled_FindsAttributedMethodInstead`
    (~527), and `FindLegacyOnlyMethod_Disabled_NoFallback` (~541), plus their test types `LegacyFallbackBase/Concrete`,
    `LegacyDisabledBase/Concrete`, `LegacyOnlyCreate` (~1054–1090). Replace them with a test that a legacy-named,
    attribute-less method is **not** found under default config.
  - Check `FindChildLegacyUpdate` (~260) and convert or delete it.
- `csla.netcore.test/DataPortal/OperationNameFlowTests.cs`: its test types use `DataPortal_Fetch` names. Add attributes.
  The operation name comes from the attribute type, so assertions shouldn't change.
- Stage B: add tests for the D1–D4 outcomes (stub behavior, child create `CheckRules`, ObjectFactory child create).
- Stage B: update analyzer unit and integration tests for attribute-only detection and CSLA0014 as a Warning.

## Phase 5 — Docs & release notes (draft in Stage A)

- `releasenotes.md` (CSLA 11 section): breaking change entry with before/after code, a pointer to CSLA0014 plus its code fix,
  and the D1–D4 behavior changes (especially child `CheckRules` if D2 = b).
- Upgrade guide: add an "Upgrading to CSLA 11" section (check whether `docs/Upgrading to CSLA 11.md` exists yet;
  follow the format of `docs/Upgrading to CSLA 10.md`).
- Update legacy-name samples in `docs/analyzers/CSLA0002`, `CSLA0009`, `CSLA0010`, `CSLA0012`, `CSLA0013`, `CSLA0014`,
  `docs/Abstractions-in-CSLA.md` (~329–412), `docs/Data-Access.md` (~7–10).
- `Samples/` pin CSLA 10 packages, so leave them alone.

## Verification

- `dotnet build Source\csla.test.sln`
- `dotnet test Source\csla.test.sln --no-build --verbosity normal --filter TestCategory!=SkipOnCIServer --settings Source/test.runsettings`
- `dotnet build Source\Csla.Analyzers.sln` and run the analyzer tests.
- Final grep: no runtime references to `UseLegacyOperationMethods`, and no `"DataPortal_" +` / `"Child_" +` name building
  in `Source/Csla`. Remaining `Child_Create` references are only those justified by D2/D3.
- Stage B: after rebasing on #4925, confirm generated `IDataPortalOperations` output for the base classes is what D1/D2 intend.
  Check the snapshot tests in `Csla.Generator.AutoImplementProperties.CSharp.Tests`.

## Risks

- **Silent behavior change for children without `[CreateChild]`:** they no longer run `CheckRules` on create (D2).
  Users with attribute-less legacy-named methods only find out at runtime ("method not found"). CSLA0014 as a Warning is the only compile-time signal.
- **Merge churn with #4925** in the base classes and analyzer resource files.
- **Test conversion volume:** a scripted rewrite risks attributing methods that are intentionally unattributed
  (e.g. tests asserting "not found"). Review the diff per file.

## Commits

Message format: `#4828 Description`, with trailer `Co-Authored-By: Claude Opus 5 (1M context) <noreply@anthropic.com>`.
