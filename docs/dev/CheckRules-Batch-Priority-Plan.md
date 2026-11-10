# Design: batch `CheckRules()` (#3736) + priority-ordered `CheckRules()` (#4235)

> Status: **implemented in #4990** (CSLA 11.0), on top of the baseline tests from #4981. This
> document started as the plan for that work and now records the design as it was built. Where
> the result differs from the original plan, the difference and the reason are noted.

## Why these are one piece of work

Before this change, `BusinessRules.CheckRules()` did:

```
CheckObjectRules(CheckRules, cascade: false)
foreach distinct PrimaryProperty in registration order:
    CheckRulesForProperty(property, cascade: true, CheckRules)   // recursive
```

- **#4235**: priority was only respected *within* one property (`orderby r.Priority` in
  `CheckRulesForProperty`). A priority -1 rule on property B ran after a priority 0 rule on
  property A if A's rules were registered first.
- **#3736**: every property ran with `cascade: true`, so each property's rules reran the rules
  of its affected properties and of every rule that used it as an input. In a batch where *every*
  rule runs anyway, most rules ran many times (objects with 100+ rules ran some rules hundreds of
  times).

Both problems came from the same structure: "for each property, run that property's rules and
recurse". Fixing #4235 alone would mean interleaving rules across properties, which doesn't fit the
recursive per-property cascade. Fixing #3736 replaces that loop with a batch runner. Once the batch
runner exists, #4235 is just its sort key.

## Rule runner abstraction

Deciding *which* rules run and in what order used to be tangled up with the mechanics of
*running* a rule, all inside `BusinessRules`. They are now separate (all types `internal`, in
`Source/Csla/Rules/RuleRunners/`):

```csharp
internal interface IRuleRunner
{
  List<string> CheckRules(IRuleRunnerHost host);                          // CheckRules()
  List<string> CheckRules(IRuleRunnerHost host, IPropertyInfo property);  // CheckRules(property)
  List<string> CheckObjectRules(IRuleRunnerHost host);                    // CheckObjectRules()
}

internal interface IRuleRunnerHost   // implemented by BusinessRules
{
  ApplicationContext ApplicationContext { get; }
  IReadOnlyList<IBusinessRuleBase> Rules { get; }
  bool CascadeOnDirtyProperties { get; }
  int ProcessThroughPriority { get; }
  void ClearBrokenRules(IPropertyInfo? property);
  void ClearBrokenRules(IPropertyInfo? property, ICollection<string> keepRuleNames);
  RuleRunOutcome RunRule(IBusinessRuleBase rule, RuleContextModes executionContext,
                         AsyncRuleCompletedHandler? onAsyncRuleCompleted);
}

internal delegate IEnumerable<string> AsyncRuleCompletedHandler(IRuleContext context);
```

- **A rule runner decides the policy:** which rules run, in what order, when to rerun and when to
  stop.
- **`BusinessRules` keeps the mechanics.** `RunRule` reads input values, executes the rule, applies
  output values, records broken rules and tracks busy state. It returns a `RuleRunOutcome`
  (affected and dirty property names, whether an error was recorded, whether the rule asked to stop
  processing). For an async rule it returns an empty outcome; when the rule completes, `RunRule`
  calls the `AsyncRuleCompletedHandler` the runner passed in. Keeping the single-rule step in
  `BusinessRules` means a runner can't corrupt broken-rule or busy state.
- **`BusinessRules`' public entry points keep the bookkeeping:** the `SuppressRuleChecking` check,
  setting and restoring `RunningRules` (in a `finally` block, so a failure such as the cascade cap
  leaves the rule engine usable), calling `AllRulesComplete()`, and removing duplicate names from the
  result.

Implementations:

- **`LegacyRuleRunner`**: the CSLA 10 code, moved out of `BusinessRules` unchanged, including the
  recursive `CheckRulesForProperty` cascade and the async-completion cascade.
- **`BatchRuleRunner`**: the new `CheckRules()` described below. It inherits `CheckRules(property)`
  and `CheckObjectRules()` from `LegacyRuleRunner`, so only `CheckRules()` changes.

Runners keep no state between calls. `AddCsla()` registers the chosen runner as a singleton.
`BusinessRules` looks it up lazily from its `ApplicationContext`, so it isn't serialized and is
available again after deserialization (tested).

## `BatchRuleRunner.CheckRules()`

The issue text suggested turning `CascadeOnDirtyProperties` off for the duration of the batch. That
wasn't done: the field is serialized state, and async rule callbacks read it while the batch is
running. The batch mode is explicit in the runner instead.

Each call builds a `BatchPlan` from the type's rules: the property rules in run order, the
properties that have rules, each property's rules, and which properties each property's value is
an input to.

1. **Object rules.** Run first, as before, with no cascade. Values they change through output values
   are added to the dirty set used in pass 2 (the legacy runner drops them).
2. **Pass 1: every property rule, once, with no cascade.**
   - Run order: `(Priority, first registration of the rule's property, rule registration)`. When
     all priorities are equal (the common case), this is exactly the CSLA 10 order, so the only
     ordering change is the one #4235 asks for.
   - Each property group's broken rules are cleared at the start of the pass.
   - Rules are filtered with `CanRunRule(..., CheckRules)` as they come up.
   - Short-circuiting stays **per property**: once a rule for a property records an error, that
     property's rules with priority above `ProcessThroughPriority` are skipped; a `StopProcessing`
     result skips the property's remaining rules. Only the interleaving across properties changes.
   - Affected, output and dirty property names are collected.
3. **Pass 2: rerun only what changed values invalidate.** Let D be the set of properties whose values
   rules changed in the previous pass. The property groups to rerun are:
   - every property in D that has rules, plus
   - the primary property of every rule that has at least one input property in D.

   In set notation, `Q = D ∪ P`, where `P = { r.PrimaryProperty : r.InputProperties ∩ D ≠ ∅ }`
   (`:` reads "such that"). Each group in Q reruns **in full**, interleaved in run order, in mode
   `CheckRules | AsAffectedProperty`, so `DenyAsAffectedProperty` is honored as in the legacy
   cascade.
   - Whole groups rerun, not single rules: if a priority 0 rule used to fail and short-circuited
     priority 1 rules, those rules never ran, so the whole group has to run again.
   - When a group reruns, its broken rules are cleared **except** those of rules that can't run in
     the rerun mode (in practice, rules marked `DenyAsAffectedProperty`). Their results still
     describe the current state; clearing them made an object with such a failing rule look valid.
     Results of rules that are short-circuited on the rerun are still cleared. (Found in review of
     #4990; the plan had said "clear its broken rules".)
4. **Loop.** With `CascadeOnDirtyProperties`, pass 2 repeats with the newly changed properties
   until nothing changes. Without it, there is one rerun pass, matching the single cascade level of
   the legacy runner. With cascading, after `MaxRerunPasses` rerun passes (default 100) the runner
   throws `InvalidOperationException` naming the properties still changing. The legacy runner
   recurses until a `StackOverflowException` kills the process in that case; the baseline tests
   confirmed this.
5. **Async rules** start in run order. Their completion keeps the legacy behavior: the handler
   reruns the rules of the async rule's affected properties. The batch can't see their output
   values.
6. **Return value:** every group that ran, plus affected and output property names (same contents
   as before).

### Behavior changes (documented in `Upgrading to CSLA 11.md`)

- Rules run in priority order across properties.
- Reruns happen only when a value changed through output values (`context.AddOutValue`). A rule that
  changes a property some other way, such as writing to the business object directly, no longer
  triggers dependent reruns in `CheckRules()`.
- Affected properties' rules don't rerun in `CheckRules()` unless a value changed; they already ran
  once in the batch. The affected properties are still in the names `CheckRules()` returns.
- `CascadeIfDirty` has no effect in `CheckRules()`, because every rerun already requires a changed
  value. It still applies to `CheckRules(property)`.
- Rules that never settle throw instead of overflowing the stack.
- How to opt out: `RuleOptions.UseLegacyCheckRules()`.

### The desk check in #3736

The desk check in the issue leaves out one rerun: `SetCreditHold` lists `OnCreditHold` as an
input, so when `OnCreditHold` changes, pass 2 also reruns the OrderTotal group. The tests confirm
it. That scenario now runs 9 rules, where the legacy runner runs 24 (cascade on) or 21 (cascade
off).

## How it was delivered

The plan proposed one PR per step. The work shipped as **#4981** (baseline tests) and a single
PR, **#4990**, with one commit per step so each step can still be reviewed and reverted on its own.

1. **#4981, baseline tests (merged).** `CheckRulesBaselineTests` (16 tests) pins the CSLA 10
   execution order, rerun counts, final values, broken rules and return values of `CheckRules()` /
   `CheckRulesAsync()`: cross-property priority, the #3736 credit-hold scenario, output-value
   chains, per-property short-circuiting, `DenyCheckRules`, `DenyAsAffectedProperty`, input
   dependencies and `CascadeIfDirty`, object rules, async completion, and dependency cycles. The
   older `CascadeTests` are still `[Ignore]`d.
2. **#4990, commit 1:** extract `RunRule` behind `IRuleRunnerHost`. No behavior change.
3. **#4990, commit 2:** add `IRuleRunner` and move the CSLA 10 logic into `LegacyRuleRunner`. No
   behavior change; the full test suite passed at this commit.
4. **#4990, commit 3:** `BatchRuleRunner` (#3736) as the default, with today's order key; rule
   options; the cap message; the upgrade doc. The baseline tests now run with
   `UseLegacyCheckRules()` and pass **unchanged**. `CheckRulesBatchTests` covers the same
   scenarios for the batch runner, so the two classes show each behavior change side by side.
5. **#4990, commit 4:** priority order across properties (#4235). Only the sort key changes.
6. **Review follow-ups in #4990:** keep the results of rules excluded on rerun, and restore
   `RunningRules` in a `finally` block (Copilot review); make the options specific to the chosen
   behavior (Stefan Ossendorf's review of this document).

`CheckRulesBatchTests` ended with 22 test methods (25 cases). It also covers the cap (including the
never-settling cycle that crashes the legacy runner), `MaxRerunPasses` configuration and
validation, last-choice-wins for the options, the rerun handling of excluded and short-circuited
rules, the rule engine staying usable after the cap exception, and `CheckRules()` after
serialization.

### Measurements

The plan called for a `Csla.Benchmarks` case. That project references the published Csla 10.1.0
package, not the source, so it couldn't measure this work. A throwaway timing test (not committed)
was used instead: a 20-property object with 104 rules, Release build, net10.0, average per
`CheckRules()` call.

| Values | Legacy | Batch |
|---|---|---|
| Already consistent | 619 rule runs, ~330–450 µs | 104 runs, ~75 µs |
| All zero (rules change 19 values) | 619 runs (cascade off) / 1,083 runs (cascade on), ~340–740 µs | 208 runs, ~115–145 µs |

### Step 6 (skip reruns for rules that already ran after a change): measured, not merged

The plan made this optional, depending on the benchmark. A prototype numbered every rule run and
reran a group only if a value it depends on changed at or after the group last started. That is a
little more conservative than the plan's "not by G's own rules": if a group's own rule changes a
value the group reads, the group still reruns, because an earlier rule in the group may have read
the old value.

It removed reruns in narrow shapes (a chain registered in dependency order: 5 runs to 3; an object
rule that sets a value: 5 runs to 3). On the 104-rule object it saved **no** rule runs and was
10–20% slower, because there the reruns are real dependencies: rules read values that change later
in the pass. It wasn't merged. It can be revisited if real-world objects turn out to look like the
narrow cases.

## Decisions

1. **Opt-out, with options per behavior.** v11 uses the batch runner by default. `RuleOptions` on
   `CslaOptions` (`o.Rules(...)`, following the `DataOptions` / `BindingOptions` pattern) chooses
   the behavior with a method that takes only that behavior's settings, so no setting is silently
   ignored by the chosen runner:

   ```csharp
   o.Rules(r => r.UseBatchCheckRules(b => b.MaxRerunPasses = 200));   // default
   o.Rules(r => r.UseLegacyCheckRules());                             // CSLA 10 behavior, temporary
   ```

   If both are called, the last call wins. `AddCsla()` registers `BatchRuleRunner` or
   `LegacyRuleRunner` accordingly; these methods are the supported way to choose a runner, because
   the runners are internal. A future runner adds its own `Use…(…)` method with its own options.
   `UseLegacyCheckRules()` is to be marked `[Obsolete]` in v12 and removed later. The choice is
   global: a per-type or per-instance choice would need more API, and a per-instance setting on
   `BusinessRules` would add a serialized field.
2. **Object rules run first**, outside the priority interleave. Their changed values feed pass 2.
3. **Configurable cycle cap that throws.** `BatchCheckRulesOptions.MaxRerunPasses` (default 100,
   at least 1). Reaching it throws `InvalidOperationException` naming the properties still
   changing; stopping quietly would leave the object unsettled with no sign of it. It applies only
   to the batch runner's `CheckRules()`.
4. **No v10.x backport.** v11 only.
5. **Runner interfaces are internal in v11.** Making them public (for example a
   `UseRuleRunner<T>()`) is a later decision, once the async runner has tested whether the host
   interface is the right shape.

## Room for priority-ordered async execution (future project)

The future goal: async rules honor priority, and each async rule says whether it can run in
parallel with others or must run alone. Priority groups then become checkpoints: the next group
can't start until the awaited async rules in the current group finish, their results can
short-circuit later groups, and their output values feed the dirty set. This work doesn't add that,
but it was built not to get in the way:

- **It becomes a new runner** (or an extension of `BatchRuleRunner`), with its own
  `RuleOptions.Use…(…)` method, rather than more mode flags in `BusinessRules`. The batch runner
  already sorts by priority first, which lines its groups up with the future checkpoints.
  `IRuleRunner` will likely need async entry points for `CheckRulesAsync()`, which is easy while it
  is internal.
- **One change point for async completions.** `RunRule` takes the completion handler as an
  argument. The batch runner passes the legacy handler, which reruns affected properties; an async
  runner can pass one that adds output values and results to the batch's state instead.
- **Batch state is only partly consolidated.** `BatchPlan` holds the per-call run order and
  dependencies, and a per-pass dictionary holds each group's short-circuit state; the dirty set and
  affected names are still locals in `CheckRules()`. The step 6 prototype moved these into a single
  state object; the async runner should do the same, behind methods that can take a lock, because
  it will update that state from rule completions.
- **No public async-scheduling API yet.** No `AsyncExecutionMode` or run-alone/parallel property on
  rules; its shape (#4773 left open whether rules with the same priority run together or one at a
  time) belongs to the async project.
- **Tests carry forward.** The baseline and batch tests record rule *start* order, so they still
  hold when async rules start awaiting at group boundaries.

## Out of scope, and follow-ups

- **Priority-gated or run-alone/parallel async execution** (see above). Priority ordering only
  controls when async rules *start*, and async results don't short-circuit anything.
- **#4991: `WaitForIdle()` returns before async rule completion finishes.** Found in review (Stefan
  Ossendorf). An async rule's completion marks the object idle before it raises `PropertyChanged`,
  the `IsValid`/`IsSavable` notifications and `ValidationComplete`, and the code after
  `await WaitForIdle()` can run inline inside the rules lock. Planned for 11.0 as a breaking
  change. With this work, the fix is in one place: `RunRule`'s async completion.
- **Clear-then-skip in the legacy cascade.** `LegacyRuleRunner.CheckRulesForProperty` clears all of
  a property's broken rules, then skips rules marked `DenyAsAffectedProperty`, so their errors can
  be lost when a property is checked as an affected property, including in `CheckRules(property)`.
  This is CSLA 10 behavior, left unchanged in the legacy runner; the batch runner's rerun pass no
  longer does it. The property-changed path should get the same fix in a separate issue.
- **`CascadeIfDirty` check.** In the legacy cascade it tests the dependent rule's *primary*
  property, not the input that changed. That looks wrong, but it's a separate issue.
- **Ordering of cascades inside `CheckRules(property)`** is unchanged. Now that it lives in a
  runner, it's easier to revisit along with the async work.
- **The `[Ignore]`d `CascadeTests`** still need to be revived or replaced.
- **A public rule runner extension point** (see Decisions).
