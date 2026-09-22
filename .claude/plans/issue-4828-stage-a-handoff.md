# CSLA #4828 — Stage A handoff notes

Written 2026-09-22 (Stage A work done 2026-09-15) on ROCKYMINI. Everything a later session needs
to pick this up on any machine.

- Issue: https://github.com/MarimerLLC/csla/issues/4828 ("Remove all legacy data portal operation method support")
- Public plan comment: https://github.com/MarimerLLC/csla/issues/4828#issuecomment-5689040061
- Branch: `issue-4828/remove-legacy-operation-methods`, pushed to `origin` (git@github.com:MarimerLLC/csla.git)
- Stage A commit: `64aa11edb` ("#4828 Remove legacy data portal operation method name fallback (Stage A)"),
  branched from `origin/main` @ `408c05eef`
- Issue is assigned to @rockfordlhotka. **No PR is open** — by design, CI would fail until Phase 2 is done.
- Full plan (the phase-by-phase version this came from) is NOT in the repo: `.claude/plans/` is gitignored
  in csla. On ROCKYMINI it lives at `S:\src\rdl\csla\.claude\plans\issue-4828.md`. Stage A worktree:
  `S:\src\rdl\csla\.claude\worktrees\issue-4828`.

To resume anywhere:

    git fetch origin
    git worktree add .claude/worktrees/issue-4828 issue-4828/remove-legacy-operation-methods

---

## THE OPEN QUESTIONS — D1 to D4 (these are what Rocky needs to answer)

These are Phase 2: CSLA base-class methods that have no operation attribute, so they were only ever
reachable through the legacy name fallback that Stage A deleted. Each needs a decision before Stage B.

### D2 is the urgent one — default child create

`Core/BusinessBase.Child_Create()` (protected virtual, ~line 1323) calls `BusinessRules.CheckRules()`.
Today it runs for any child with no `[CreateChild]` method. `BusinessListBase.Child_Create()` and
`BusinessBindingListBase.Child_Create()` are no-op defaults.

**Evidence from the Stage A test run: all 129 test failures come from this one thing.** Removing the
fallback means any list or child without its own create-child method now throws
`TargetParameterCountException ... [CreateChild]()`. Affected across Csla.test (GraphMerge, Basic,
BusinessListBase, BasicModern, Interceptors, DataPortalChild), Csla.Blazor.Test fakes, and GraphMergerTest.

Options:
- (a) Mark them `[CreateChild]`, and verify overload scoring against user `[CreateChild]` overloads plus #4925 generation.
- (b) Remove them and document that children need their own `[CreateChild]` that calls `CheckRules()`.
- (c) Have `ChildDataPortal` fall back to `CheckRules()` itself when no create-child method exists.

**Claude's recommendation: (a).** They're `protected virtual`, so the #4090 private-method filter
(`level < 0` → `!m.IsPrivate`) doesn't skip them, and a user method on the derived class still scores
higher than a base-class one. The failure volume shows how much ordinary code depends on this default.
Silently dropping the initial `CheckRules()` (option b) is the "silent behavior change" risk flagged in the plan.

**DECISION: _______**

### D1. "Not supported" stubs

Private methods that throw `NotSupportedException` with a friendly resource message:

| Class | Methods without attribute |
|---|---|
| `CommandBase` | `DataPortal_Create(object)`, `DataPortal_Fetch(object)`, `DataPortal_Update()`, `DataPortal_Delete(object)` |
| `ReadOnlyBase` | `DataPortal_Create(object)`, `DataPortal_Update()` (`DataPortal_Delete` already has `[Delete]`) |
| `ReadOnlyListBase`, `ReadOnlyBindingListBase`, `NameValueListBase`, `DynamicListBase`, `DynamicBindingListBase` | `DataPortal_Update()`, `DataPortal_Delete(object)` |

Options:
- (a) Delete them and accept a generic "method not found" error.
- (b) Keep the friendly message by adding the attribute. Because of #4090, **private** attributed methods on a
  base class are skipped, so they'd also need to become `protected` / `private protected`. Check that a stub
  never outscores a user method, and check what #4925's generator emits for them.

Either way, remove the stale `SuppressMessage` attributes (CA1801/CA1811/CA1822) on them.

**Claude's recommendation: (b) if #4925's generator output for them is clean, else (a).** No Stage A test
failures pointed here, so this is a message-quality call, not a correctness one.

**DECISION: _______**

### D3. ObjectFactory child create

`ServiceProviderMethodCaller` ~lines 170/179 (still present, deliberately left for Stage B) look for a method
literally named `Child_Create`, first on the factory type, then on the target type.

Options:
- (a) Look for `[CreateChild]` on the target type.
- (b) Add `CreateChildMethodName` to `ObjectFactoryAttribute` to match `CreateMethodName` etc.
- (c) Keep the factory-side name (factories are name-based by design) and only drop the target-type fallback.

**Claude's recommendation: keep the factory-side name (c), and switch the target-type lookup to `[CreateChild]` (a).**
No Stage A test failures pointed here.

**DECISION: _______**

### D4. Sync `Child_Update(params object?[])`

On `BusinessListBase`, `BusinessBindingListBase`, `BusinessDocumentBase`. The async `Child_UpdateAsync`
already has `[UpdateChild]`, so the sync versions are just ordinary protected helpers now.

**Claude's recommendation: leave them alone.** Lots of user `[Update]` methods call `Child_Update()`
directly — renaming or obsoleting breaks source compatibility for no runtime gain. No Stage A failures here.

**DECISION: _______**

---

## What Stage A actually did (commit 64aa11edb — 58 files, +129/-88)

### Phase 1 — runtime removal (complete)
- `Source/Csla/Reflection/ServiceProviderMethodCaller.cs`: deleted the `useLegacyMethods` lookup, the
  "if no attribute-based methods found, look for legacy methods" block, and the `|nolegacy` cache-key suffix;
  `GetCacheKeyName` lost its `bool useLegacyMethods` parameter.
- `Source/Csla/Configuration/Fluent/DataPortalOptions.cs`: removed `UseLegacyOperationMethods`.
- Left in place on purpose: the hard-coded `m.Name == "Child_Create"` ObjectFactory paths (~lines 170/179) — that's D3.
- Note: `using Csla.Configuration;` is still at the top of ServiceProviderMethodCaller.cs. It may now be
  unused (no `Csla.Configuration` type names remain in the file); not verified, worth an IDE0005 check in Stage B.

### Phase 4 — test conversions (Stage A portion complete)
94 legacy-named test methods got their matching attribute — 85 by script, 9 by hand.

Scripted with the attached `add-operation-attributes.py` (also below). It finds method *declarations* only,
walks upward past attributes/comments to see whether an operation attribute is already present, and inserts
`[Attr]` on its own line above the declaration. Insert-only, so line endings are preserved. Run it without
`--apply` first to review the report. The pathspec used in Stage A:

    python add-operation-attributes.py --apply Source/tests \
      ':!Source/tests/Csla.Analyzers.IntegrationTests' \
      ':!Source/tests/Csla.Analyzers.Tests' \
      ':!Source/tests/Csla.test/Silverlight' \
      ':!Source/tests/Csla.test/CslaDataProvider' \
      ':!Source/tests/Csla.test/Data' \
      ':!Source/tests/Csla.test/DataPortal/ESTransactionalRoot.cs'

Skipped on purpose, and why:
- `Csla.test/Silverlight`, `CslaDataProvider`, `Data`, `ViewModelTests`, `Windows`, `DataBinding`,
  `GraphMergerTest`, `IdentityConverter` — `Compile Remove`d in `Csla.Tests.csproj`, so not compiled.
- `DataPortal/ESTransactionalRoot.cs` — also `Compile Remove`d.
- `Csla.Analyzers.Tests` and `Csla.Analyzers.IntegrationTests` — Stage B / Phase 3 work.
- `Csla.Ios.Test` and `Csla.Web.Mvc.Test` were included even though they aren't in `csla.test.sln`.

Hand edits in `csla.netcore.test/DataPortal/ServiceProviderMethodCallerTests.cs`:
- Deleted `FindLegacyMethod_DefaultEnabled_FindsLegacyFallback`, `FindLegacyMethod_Disabled_FindsAttributedMethodInstead`,
  `FindLegacyOnlyMethod_Disabled_NoFallback`, and the `LegacyDisabledBase/Concrete` types.
- Added `FindMethod_PrivateBaseWithLegacyNamedMethod_FindsAttributedMethod` (the #4595 regression: private
  `[Execute]` on a base plus an attribute-less `DataPortal_Execute` must resolve to `Execute`) and
  `FindLegacyNamedMethodWithoutAttribute_NotFound`.
- Renamed `LegacyFallbackBase/Concrete` → `LegacyNamedBase/Concrete`; kept `LegacyOnlyCreate`.
- Renamed `FindChildLegacyUpdate` → `FindChildNoParamsUpdate` and attributed `BasicChild.Child_Update`.

**Two methods are intentionally left without attributes** — they are the negative-test fixtures. Don't let a
future scripted pass "fix" them: `LegacyNamedBase<T>.DataPortal_Execute()` and `LegacyOnlyCreate.DataPortal_Create()`.

One pre-existing oddity, untouched: `Csla.test/DataPortal/DataPortalExceptionTest.cs:164` has a `Child_Fetch`
carrying `[Fetch]` (root, not child). Flagged by the script as MISMATCH.

### Phase 5 — docs drafted
- `releasenotes.md`: CSLA 11 breaking-change entry.
- `docs/Upgrading to CSLA 11.md`: NEW file, with the legacy-name → attribute mapping table and before/after code.
- `docs/analyzers/CSLA0014-*.md`: rewritten for the CSLA 11 behavior and Warning severity (the severity change
  itself is Phase 3, not yet implemented).
- `docs/analyzers/CSLA0002, 0009, 0010, 0012, 0013`: samples now use attributes.
- `docs/Abstractions-in-CSLA.md` (~329) and `docs/Data-Access.md` (~7-10): legacy names removed.
- `Samples/` deliberately untouched (they pin CSLA 10 packages).
- Both `releasenotes.md` and the upgrade guide carry `<!-- TODO(#4828 Stage B): ... -->` markers where the
  D1-D4 text goes.

---

## Stage B checklist (after #4925 merges)

#4925 = "Call data portal operation methods explicitly via bundled source generators", branch
`feature/explicit-operation-method-calls`. As of 2026-09-22 it is still OPEN. Check with:
`gh pr view 4925 --json state,mergedAt`.

1. `git fetch`, rebase the branch onto `origin/main`. Expect trivial conflicts: #4925 adds `partial` to the
   class declaration lines of `CommandBase`, `ReadOnlyBase`, `ReadOnlyListBase`, `ReadOnlyBindingListBase`,
   `NameValueListBase`, `DynamicListBase`, `DynamicBindingListBase`, `BusinessListBase`,
   `BusinessBindingListBase`, `BusinessDocumentBase`; it also appends to `releasenotes.md`,
   `AnalyzerReleases.Unshipped.md`, analyzer `Constants.cs`, and `Resources.resx`.
2. Implement D1-D4 per Rocky's answers, against #4925's final code. The reason to wait: #4925's generator emits
   a nested `IDataPortalOperations` interface plus named dispatch for **attributed** operations in `partial`
   classes, abstract bases included — so attributing base-class methods changes what gets generated inside Csla.dll.
   Check the snapshot tests in `Csla.Generator.AutoImplementProperties.CSharp.Tests`.
3. Phase 3 — analyzers (`Source/Csla.Analyzers/Csla.Analyzers/`):
   - `Extensions/IMethodSymbolExtensions.cs`: drop the `byNamingConvention` checks in `IsRootDataPortalOperation` /
     `IsChildDataPortalOperation`.
   - `Extensions/DataPortalOperationQualification.cs`: drop `ByNamingConvention` or collapse to a bool; update `Deconstruct` callers.
   - `CslaMemberConstants.cs` ~79-127: remove the `DataPortal_*` / `Child_*` constants, keeping whatever CSLA0014
     needs (they're used by `DoesOperationHaveAttributeAddAttributeCodeFix.cs`).
   - Review every qualification consumer: `DoesChildOperationHaveRunLocalAnalyzer`, `DoesOperationHaveAttributeAnalyzer`,
     `EvaluateOperationAttributeUsageAnalyzer`, `FindOperationsWithIncorrectReturnTypesAnalyzer`,
     `FindOperationsWithNonSerializableArgumentsAnalyzer`, `FindRefAndOutParametersInOperationsAnalyzer`,
     `IsOperationMethodPublicAnalyzer`, `ObjectAuthorizationRulesAttributeAnalyzer`, `ITypeSymbolExtensions`.
   - CSLA0014 keeps legacy-name detection (only here), severity Info → Warning, keeps its code fix.
     Record it in `AnalyzerReleases.Unshipped.md`. Doc is already updated.
   - `AddObjectAuthorizationRules` naming convention: OUT OF SCOPE unless Rocky says otherwise.
4. Fix XML doc comments still naming `DataPortal_XYZ`: `CommandBase` (~33-39), `Core/BusinessBase` (~243, 267, 271),
   `DynamicListBase` (~32), `DynamicBindingListBase` (~30), `IDataPortal` (~90), `IDataPortalT` (~93).
5. Convert the analyzer test projects, add tests for the D1-D4 outcomes, resolve the TODO markers in the docs.
6. Full build + test, then open the PR.

Commit format: `#4828 Description`, trailer `Co-Authored-By: Claude Opus 5 (1M context) <noreply@anthropic.com>`.

---

## Verification, and the SDK trap on ROCKYMINI

Normal commands:

    dotnet build Source\csla.test.sln
    dotnet test Source\csla.test.sln --no-build --verbosity normal --filter TestCategory!=SkipOnCIServer --settings Source/test.runsettings
    dotnet build Source\Csla.Analyzers.sln

**`main` targets net11.0 and `Source/global.json` pins SDK 11.0.100-rc.1.26425.128.** As of 2026-09-22
ROCKYMINI has only .NET 10 SDKs (10.0.401, 10.0.100-rc.1, 9.0.304) and runtime 10.0.12 — no .NET 11 SDK
anywhere on the box, so the solution will NOT build there as-is. Rocky believed .NET 11 was installed; it isn't.
Either install the .NET 11 SDK, or use the Stage A workaround:

1. Strip `net11.0` from every csproj (attached `strip-net11-from-csprojs.py`; 25 files).
2. Build and test as normal — everything resolves to net10.0.
3. Revert with `git checkout -- <the 25 csproj paths>`. They were never committed in Stage A.

Approaches that did NOT work, don't retry them: `dotnet build -f net10.0` (restore still evaluates every TFM);
`-p:TargetFrameworks=net10.0` (breaks the netstandard2.0 generator projects); injecting a targets file via
`CustomAfterMicrosoftCommonTargets` / `CustomAfterMicrosoftCommonCrossTargetingTargets` (breaks reference TFM matching).

## Stage A test results (net10.0, 2026-09-15)

Build: 0 errors. Tests: **129 failures, every one of them D2.** Categories, by declaring type:

    18  Csla.Blazor.Test.Fakes.FakePersonEmailAddresses  [CreateChild]()
    17  Csla.Test.GraphMerge.FooList
    15  Csla.Test.Basic.Children
    15  Csla.Test.BusinessListBase.ChildList
    12  Csla.Test.Basic.GrandChildren
    10  Csla.Test.BasicModern.ChildList
    10  Csla.Test.GraphMergeAsync.FooList
     7  Csla.Test.Server.Interceptors.Children
     6  Csla.Test.DataBinding.ChildEntityList
     5  Csla.Test.DataPortalChild.ChildList
     3  GraphMergerTest.Business.ChildItems
     3  Csla.Test.Basic.RootListChild
     2  Csla.Test.GraphMerge.LeafUniqueIdentities
     1  each: ModernChild, Basic.TestItem, ValidationRules.Child, Basic.Child, Authorization.ChildItem
     1  OTHER: NullReferenceException in Blazor SaveThenCancel_ValidatePropertyValue (looks like a knock-on)

Nothing failed for D1, D3 or D4 reasons, and nothing failed in the 94 converted test types. Once D2 lands,
this suite should go green — that's the Stage B gate.

The classifier script that produced that breakdown is attached as `classify-test-failures.py`
(feed it a `dotnet test` log).
