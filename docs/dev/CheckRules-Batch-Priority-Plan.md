# Plan: batch `CheckRules()` (#3736) + priority-ordered `CheckRules()` (#4235)

> Status: draft for review. Comments on the PR that adds this file are welcome, especially on the
> behavior changes, the cycle-cap behavior, the rule runner split, and whether the design leaves
> enough room for the future priority-ordered async rule work.

## Why these are one piece of work

Today `BusinessRules.CheckRules()` (`Source/Csla/Rules/BusinessRules.cs:759`) does:

```
CheckObjectRules(CheckRules, cascade: false)
foreach distinct PrimaryProperty in registration order:
    CheckRulesForProperty(property, cascade: true, CheckRules)   // recursive
```

- **#4235**: priority is only respected *within* one property (`orderby r.Priority` in
  `CheckRulesForProperty`). A priority -1 rule on property B runs after a priority 0 rule on
  property A if A's rules were registered first.
- **#3736**: every property runs with `cascade: true`, so each property's rules rerun the rules
  of its affected properties and of every rule that uses it as an input. In a batch where *every*
  rule runs anyway, that means most rules run many times (100+ rule objects run some rules hundreds
  of times).

Both problems come from the same structure: "for each property, run that property's rules and
recurse". Fixing #4235 alone means interleaving rules across properties, which doesn't fit the
recursive per-property cascade. Fixing #3736 replaces that loop with a batch runner. Once the batch
runner exists, #4235 is just its sort key. So the plan is: build the batch runner (#3736), then
switch its sort key to priority (#4235).

## Rule runner abstraction

Today, the decision about *which* rules run and in what order is tangled up with the mechanics of
*running* a rule, all inside `BusinessRules`. The plan separates them:

- **A rule runner decides the policy:** which rules run, in what order, when to rerun, and when to
  stop. A runner owns all three entry points: `CheckRules()`, `CheckRules(property)` and
  `CheckObjectRules()`.
- **`BusinessRules` keeps the mechanics**, which runners reach through a small host interface:
  - get the rules for the type
  - run one rule (`RunRule`): read input values, execute, apply output values, record broken
    rules, track busy state, handle async completion
  - clear a property's broken rules
  - read settings (`ProcessThroughPriority`, `CascadeOnDirtyProperties`, `SuppressRuleChecking`)
  - signal completion (`IHostRules` callbacks, `RunningRules`)

  Keeping the single-rule step in `BusinessRules` means a runner can't corrupt broken-rule or busy
  state, and the host interface stays small.

Sketch (names are placeholders):

```csharp
internal interface IRuleRunner
{
  List<string> CheckRules(IRuleRunnerHost host);
  List<string> CheckRules(IRuleRunnerHost host, IPropertyInfo property);
  List<string> CheckObjectRules(IRuleRunnerHost host);
}

internal interface IRuleRunnerHost
{
  IReadOnlyList<IBusinessRuleBase> Rules { get; }
  RuleRunOutcome RunRule(IBusinessRuleBase rule, RuleContextModes mode, IAsyncRuleCompletionHandler onAsyncComplete);
  void ClearBrokenRules(IPropertyInfo? property);
  // settings, completion signals ...
}
```

Implementations:

- **`LegacyRuleRunner`**: today's behavior, moved out of `BusinessRules` unchanged.
- **`BatchRuleRunner`**: the new `CheckRules()` described below. It inherits the property-changed
  path (`CheckRules(property)`) and `CheckObjectRules()` from `LegacyRuleRunner`, so only the batch
  path changes.
- **Later:** a priority-ordered async runner (see the async section).

Runners keep no state between calls. They're resolved from DI, never serialized with the business
object, and looked up lazily from `ApplicationContext`, so they're still there after
deserialization. Per-call state lives in a state object created for each call.

**The runner interfaces are internal in v11.** Registering them through DI is still worthwhile,
because the public option below just picks which internal runner gets registered, and the tests can
run against each runner. Making the interfaces public (with a `CslaOptions.UseRuleRunner<T>()`) is
a later decision. By then the async runner will have tested whether the host interface is the right
shape.

## Target algorithm for `BatchRuleRunner.CheckRules()`

Don't flip `CascadeOnDirtyProperties` temporarily, as the issue text suggests. That field is
serialized state, and async rule callbacks read it while the batch is running. Pass the batch mode
explicitly instead.

1. **Object rules.** Run as today, first and with no cascade. Add their `DirtyProperties` to the
   dirty set used in pass 2 (today those outputs are dropped).
2. **Pass 1: every property rule, once, with no cascade.**
   - Candidates: the type's rules where `PrimaryProperty != null && CanRunRule(..., CheckRules)`.
   - Sort key (stable): `(Priority, firstAppearanceIndex(PrimaryProperty), registrationIndex)`.
     When all priorities are equal (the common case), this gives exactly today's order, so the only
     ordering change is the one #4235 asks for.
   - Clear each property group's broken rules before the pass.
   - Keep **per-property** short-circuit state: `anyRuleBroken` (for `ProcessThroughPriority`) and
     `stopped` (`StopProcessing`). Skip a rule if its property group is short-circuited.
     Short-circuit behavior stays per property, as today. Only the interleaving changes.
   - Collect affected, output and dirty property names.
3. **Pass 2: rerun only what dirty values invalidate.** With dirty set D, the groups to rerun are
   `Q = D ∪ { r.PrimaryProperty | r.InputProperties ∩ D ≠ ∅ }`. Rerun each group in Q **in full**
   (clear its broken rules, then run all its rules) with the same interleaved runner, restricted to Q,
   in mode `CheckRules | AsAffectedProperty` so `DenyAsAffectedProperty` is honored as it is in
   today's cascade.
   - Rerun whole groups rather than single rules: if a priority 0 rule used to fail and
     short-circuit priority 1 rules, those rules never ran, so the whole group has to run again.
4. **Loop.** If `CascadeOnDirtyProperties`, repeat pass 2 with the newly dirty set until it is empty.
   Otherwise stop after one pass 2 (this matches today's single cascade level). Stop after
   `MaxCheckRulesPasses` rounds (see Decisions) so rules that keep changing each other's values fail
   clearly. Today that case recurses until a `StackOverflowException` kills the process; the
   baseline tests confirmed this.
5. **Async rules.** They start in pass order. Their completion keeps today's behavior and reruns
   affected properties. The batch can't see their outputs, so the async completion handler passed to
   `RunRule` is separate from batch control.
6. **Return value.** The union of every group that ran, plus affected and output property names,
   with duplicates removed (same contents as today). Then `AllRulesComplete()` as today.

### Behavior changes to document (Upgrading to CSLA 11.md)

- Cross-property execution order follows priority.
- In a batch, reruns happen only when a value changed through `OutputPropertyValues` (dirty
  tracking). A rule that changes a property some other way, such as writing to `context.Target`
  directly, no longer triggers dependent reruns in `CheckRules()`. Its dependents run once in
  normal pass order.
- `CascadeIfDirty` has no effect in batch mode, because batch reruns already require a dirty value.
  (Side note: the existing check at `BusinessRules.cs:949` tests the dependent rule's *primary*
  property, not the input that changed. That looks wrong, but it's a separate issue.)
- A cycle that never settles now throws instead of overflowing the stack.
- How to opt out (`RuleOptions.UseLegacyCheckRules`).

### Note on the desk check in #3736

The desk check in the issue leaves out one rerun. `SetCreditHold` lists `OnCreditHold` as an input,
so when `OnCreditHold` goes dirty, pass 2 also reruns the OrderTotal group. That is correct (the
rule's input changed), but the test expectations should count it.

## PR sequence

1. **Baseline tests. Done: #4981.** `CheckRulesBaselineTests` (16 tests, `Csla.Tests`) pins
   today's execution order, rerun counts, final values, broken rules and return values of
   `CheckRules()`/`CheckRulesAsync()`:
   - cross-property priority
   - the #3736 credit-hold scenario, with cascade on and off
   - output-value chains
   - per-property short-circuit (`ProcessThroughPriority`, `StopProcessing`)
   - `DenyCheckRules`, `DenyAsAffectedProperty`
   - input dependencies and `CascadeIfDirty`
   - object rules
   - async completion
   - a cycle that settles (cascade on and off), and one that never settles (cascade off only)

   The never-settling cycle with cascade on crashes the test host today, so it gets a test in PR 4,
   once the cap makes it throw. The older `CascadeTests` are still `[Ignore]`d; reviving them is
   separate work. Later PRs update these expectations, so each behavior change shows in the diff.
2. **Extract `RunRule` and the host interface.** Pull the per-rule body of `RunRules`
   (`BusinessRules.cs:991-1141`) into `RunRule(...) -> RuleRunOutcome { affected, dirty,
   brokenError, stop }`. Pull async completion handling into one handler passed to `RunRule`.
   `BusinessRules` implements `IRuleRunnerHost`. Pure refactor; the baseline tests stay green.
3. **Add `IRuleRunner` and `LegacyRuleRunner`.** Move today's `CheckRules()`,
   `CheckRules(property)`, `CheckObjectRules()` and `CheckRulesForProperty` logic into
   `LegacyRuleRunner`, register it in `AddCsla()`, and have `BusinessRules` delegate to the resolved
   runner. Still no behavior change; the baseline tests prove it.
4. **`BatchRuleRunner` (#3736).** Steps 1–6 above, using today's order key
   `(firstAppearance, Priority, registration)`. Add `RuleOptions` (`UseLegacyCheckRules`,
   `MaxCheckRulesPasses`) and make the batch runner the default. Run the baseline tests under both
   runners through `UseLegacyCheckRules`, so the legacy runner is shown to still match today. Update
   run-count expectations for the batch runner. Add the never-settling-cycle test. Add a
   `csla.benchmarks` case (around 100 rules with dependencies) to compare before and after. Add the
   upgrade doc entry.
5. **Priority order (#4235).** Change the batch runner's sort key to
   `(Priority, firstAppearance, registration)`. A tiny PR that can be reverted on its own, with
   updated cross-property priority expectations and a doc line.
6. **Optional: skip stale-free reruns.** Use per-property change sequence numbers. Rerun group G for
   dirty P only if P changed after G last started, and not by G's own rules. This removes the
   remaining redundant reruns, such as the OrderTotal rerun above. Do it only if the benchmark shows
   it's worth it.

PRs 4 and 5 could be one PR. Keeping them separate makes each behavior change reviewable and
revertible.

## Decisions (settled)

1. **Opt-out switch.** v11 uses the batch runner by default. Add a new `RuleOptions` group on
   `CslaOptions`, following the existing `DataOptions` / `BindingOptions` pattern, with
   `UseLegacyCheckRules` (default `false`). When it's `true`, `AddCsla()` registers
   `LegacyRuleRunner` instead of `BatchRuleRunner`. Because the runner interfaces are internal,
   this public option is the supported way to choose a runner. Mark it `[Obsolete]` in v12 and
   remove it later. The option is global for now: a per-type or per-instance choice would need more
   API, and a per-instance setting on `BusinessRules` would add a serialized field.
2. **Object rules run first**, as today, and are not part of the priority interleave. Their dirty
   outputs feed pass 2.
3. **Configurable cycle cap.** Add `RuleOptions.MaxCheckRulesPasses`, the number of pass-2 rounds
   allowed when `CascadeOnDirtyProperties` is on (default 100). When the cap is reached, throw
   `InvalidOperationException` naming the properties that are still changing. Throwing rather than
   stopping quietly is proposed (feedback welcome): stopping quietly would leave the object in an
   unsettled state that nobody sees. It only applies to the batch runner. The property-changed
   recursion is unchanged.
4. **No v10.x backport.** v11 only.
5. **Runner interfaces are internal in v11.** `IRuleRunner`, `IRuleRunnerHost` and the runner
   implementations are `internal`. They're registered through DI so the option can pick one, and so
   a public extension point can be added later without restructuring.

## Leave room for priority-ordered async execution (future project)

The future goal: async rules honor priority, and each async rule says whether it can run in
parallel with others or must run alone. That turns priority groups into checkpoints: the next
group can't start until the awaited async rules in the current group finish. Their results can then
short-circuit later groups, and their outputs feed the dirty set. This plan doesn't add that, but
it shouldn't get in its way either. Concretely:

- **It becomes a new runner.** The async work adds a runner (or extends `BatchRuleRunner`), rather
  than more mode flags in `BusinessRules`. Sorting by priority first already lines the batch
  runner's groups up with the future checkpoints. The async runner's `CheckRulesAsync()` path can
  await at each group boundary. Synchronous `CheckRules()` would keep starting async rules without
  waiting, because it can't await. That likely means `IRuleRunner` gains async entry points then,
  which is easy while it's internal.
- **Keep batch state in one object** (per-property short-circuit state, the dirty set, affected
  names, the pass counter), not in local variables spread across loops. The async runner updates it
  from rule completions, so access goes through methods that can take a lock.
- **One change point for async completions.** `RunRule` receives the async completion handler as an
  argument. This plan keeps today's handler, which reruns affected properties. The async runner
  passes a handler that adds outputs and broken results to the batch state instead.
- **No public async-scheduling API yet.** No `AsyncExecutionMode` or run-alone/parallel property on
  rules. Its shape (#4773 left the open question of whether rules in the same priority run together
  or one at a time) belongs to the async project.
- **Tests carry forward.** The baseline tests record rule *start* order, so they still hold when
  async rules start awaiting at group boundaries.

## Out of scope

- Priority-gated or run-alone/parallel **async** execution (see the section above). In this plan,
  priority ordering only controls when async rules *start*, and async results don't short-circuit
  anything, same as today.
- Ordering of cascades inside `CheckRules(property)` (the property-changed path). Its behavior is
  unchanged. Once it lives in a runner, it's easier to revisit along with the async work.
- A public rule runner extension point (see Decisions).
