# Plan: batch `CheckRules()` (#3736) + priority-ordered `CheckRules()` (#4235)

> Status: draft for review. Comments on the PR that adds this file are welcome, especially on the
> behavior changes, the cycle-cap behavior, and whether the design leaves enough room for the
> future priority-ordered async rule work.

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

## Target algorithm for `CheckRules()`

Don't flip `CascadeOnDirtyProperties` temporarily, as the issue text suggests. That field is
serialized state, and async rule callbacks read it while the batch is running. Pass the batch mode
explicitly instead.

1. **Object rules.** Run as today, first and with no cascade. Add their `DirtyProperties` to the
   dirty set used in pass 2 (today those outputs are dropped).
2. **Pass 1: every property rule, once, with no cascade.**
   - Candidates: `TypeRules.Rules` where `PrimaryProperty != null && CanRunRule(..., CheckRules)`.
   - Sort key (stable): `(Priority, firstAppearanceIndex(PrimaryProperty), registrationIndex)`.
     When all priorities are equal (the common case), this gives exactly today's order, so the only
     ordering change is the one #4235 asks for.
   - `BrokenRules.ClearRules(property)` for each property group before the pass.
   - Keep **per-property** short-circuit state: `anyRuleBroken` (for `ProcessThroughPriority`) and
     `stopped` (`StopProcessing`). Skip a rule if its property group is short-circuited. Short-circuit
     behavior stays per property, as today. Only the interleaving changes.
   - Collect affected, output and dirty property names.
3. **Pass 2: rerun only what dirty values invalidate.** With dirty set D, the groups to rerun are
   `Q = D ∪ { r.PrimaryProperty | r.InputProperties ∩ D ≠ ∅ }`. Rerun each group in Q **in full**
   (clear its broken rules, then run all its rules) with the same interleaved runner, restricted to Q,
   in mode `CheckRules | AsAffectedProperty` so `DenyAsAffectedProperty` is honored as it is in
   today's cascade.
   - Rerun whole groups rather than single rules: if a priority 0 rule used to fail and
     short-circuit priority 1 rules, those rules never ran, so the whole group has to run again.
4. **Loop.** If `CascadeOnDirtyProperties`, repeat pass 2 with the newly dirty set until it is empty.
   Otherwise stop after one pass 2 (this matches today's single cascade level). Add an iteration
   cap so rules that keep changing each other's values fail clearly, instead of recursing until a
   `StackOverflowException`, which can happen today.
5. **Async rules.** They start in pass order. Their completion callback keeps today's behavior and
   reruns affected properties. The batch can't see their outputs, so separate RunRules' `cascade`
   argument (which drives the async callback) from batch control.
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

### Note on the desk check in #3736

The desk check in the issue leaves out one rerun. `SetCreditHold` lists `OnCreditHold` as an input,
so when `OnCreditHold` goes dirty, pass 2 also reruns the OrderTotal group. That is correct (the
rule's input changed), but the test expectations should count it.

## Refactor that makes this clean

Extract the per-rule body of `RunRules` (`BusinessRules.cs:991-1141`) into
`RunRule(rule, executionContext, cascadeAsync) -> RuleRunOutcome { affected, dirty, brokenError, stop }`.

- Existing `RunRules` becomes a loop over `RunRule` with no behavior change. It is still used by
  `CheckRules(property)`, `CheckObjectRules` and the cascade in `CheckRulesForProperty`.
- The new `CheckRulesBatch` loops over the sorted candidates using `RunRule` and keeps the
  per-property short-circuit state in a dictionary.

## PR sequence

1. **Characterization tests (no product change).** Today almost all cascade tests call
   `CheckRules(property)`. Add tests for `CheckRules()`:
   - a counting rule that records execution order and counts per rule
   - the #3736 order scenario (credit hold / can check out / sign agreement)
   - cross-property priority (A priority 1 registered first, B priority -1)
   - per-property short-circuit (`ProcessThroughPriority`, `StopProcessing`), `DenyCheckRules`,
     `DenyAsAffectedProperty`, async rules plus `CheckRulesAsync`, object rule outputs, the cascade
     spreadsheet case, the Ce↔Cd cycle in `CascadeRoot`

   Tests whose expectations change in later PRs get updated in those PRs, so the diff shows each
   behavior change.
2. **Extract `RunRule`.** Pure refactor; all tests stay green. Includes pulling the async
   completion handling into one handler that `RunRule` receives as an argument (see the async
   section).
3. **Batch runner (#3736).** `CheckRulesBatch` with steps 1–6, using today's order key
   `(firstAppearance, Priority, registration)`. Add `RuleOptions` (`UseLegacyCheckRules`,
   `MaxCheckRulesPasses`). Run the PR 1 tests under both modes, so legacy mode is shown to still
   match today. Update run-count expectations for batch mode. Add a `csla.benchmarks` case (around
   100 rules with dependencies) to compare before and after. Add the upgrade doc entry, including
   the opt-out.
4. **Priority order (#4235).** Change the sort key to `(Priority, firstAppearance, registration)`.
   A tiny PR that can be reverted on its own, with cross-property priority tests and a doc line.
5. **Optional: skip stale-free reruns.** Use per-property change sequence numbers. Rerun group G for
   dirty P only if P changed after G last started, and not by G's own rules. This removes the
   remaining redundant reruns, such as the OrderTotal rerun above. Do it only if the benchmark shows
   it's worth it.

PRs 3 and 4 could be one PR. Keeping them separate makes each behavior change reviewable and
revertible.

## Decisions (settled)

1. **Opt-out switch.** v11 uses the batch behavior by default. Add a new `RuleOptions` group on
   `CslaOptions`, following the existing `DataOptions` / `BindingOptions` pattern, with
   `UseLegacyCheckRules` (default `false`). When it's `true`, `CheckRules()` runs today's loop,
   which stays as a private `CheckRulesLegacy()`. Keeping it costs almost nothing, because
   `CheckRulesForProperty` stays for the property-changed path. Mark it `[Obsolete]` in v12 and
   remove it later. `BusinessRules` reads the options through
   `_applicationContext.GetRequiredService<CslaOptions>()`, as `AddDataAnnotations` already does.
   Keep the options global for now: a per-instance setting on `BusinessRules` would add a
   serialized field.
2. **Object rules run first**, as today, and are not part of the priority interleave. Their dirty
   outputs feed pass 2.
3. **Configurable cycle cap.** Add `RuleOptions.MaxCheckRulesPasses`, the number of pass-2 rounds
   allowed when `CascadeOnDirtyProperties` is on (default 100). When the cap is reached, throw
   `InvalidOperationException` naming the properties that are still changing. Throwing rather than
   stopping quietly is proposed (feedback welcome): stopping quietly would leave the object in an
   unsettled state that nobody sees. It only applies to the batch path. The property-changed
   recursion is unchanged.
4. **No v10.x backport.** v11 only.

## Leave room for priority-ordered async execution (future project)

The future goal: async rules honor priority, and each async rule says whether it can run in
parallel with others or must run alone. That turns priority groups into checkpoints: the next
group can't start until the awaited async rules in the current group finish. Their results can then
short-circuit later groups, and their outputs feed the dirty set. This plan shouldn't add that, but
it shouldn't get in its way either. Concretely:

- **Separate deciding what to run from running it.** The batch code builds a list of rules, grouped
  by priority. Sorting by priority first already lines these groups up with the future checkpoints.
  A separate driver walks that list. Today the driver is synchronous. Later, `CheckRulesAsync()` can
  get a driver that awaits at each group boundary. Synchronous `CheckRules()` would keep starting
  async rules without waiting, because it can't await.
- **Keep batch state in one object** (`CheckRulesBatchState`: per-property short-circuit state, the
  dirty set, affected names, the pass counter), not in local variables spread across loops. Both
  drivers share it. The future async driver updates it from rule completions, so access goes
  through methods that can take `_syncRoot`.
- **Route async completions through one place.** Today an async rule's completion callback reruns
  affected properties itself. Pull that out into a single "async completion handler" that `RunRule`
  receives as an argument. This plan keeps today's handler, which does the rerun. The future driver
  swaps in a handler that adds outputs and broken results to the batch state instead. That is the
  single change point.
- **Don't add async-scheduling public API now.** No `AsyncExecutionMode` or parallel/alone
  property on rules yet. Its shape (#4773 left the open question of whether rules in the same
  priority run together or one at a time) belongs to the async project. Keep the batch runner and
  `RunRule` `private`/`internal` so the async work can reshape them without breaking changes.
- **Name the option for its purpose.** `UseLegacyCheckRules` only selects legacy vs. batch. Async
  scheduling will get its own setting later, and won't be squeezed into this switch.
- **Tests to carry forward.** The PR 1 tests that record execution order use rule *start* order, so
  they still hold when async rules start awaiting at group boundaries.

## Out of scope

- Priority-gated or alone/parallel **async** execution (see the section above). In this plan,
  priority ordering only controls when async rules *start*, and async results don't short-circuit
  anything, same as today.
- Ordering of cascades inside `CheckRules(property)` (the property-changed path). It is unchanged.
  The `RunRule` extraction does make it easier to revisit along with the async work.
