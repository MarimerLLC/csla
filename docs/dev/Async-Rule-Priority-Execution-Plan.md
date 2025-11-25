# Plan: Async Rules Priority Execution

## Problem Statement

Currently in CSLA.NET, business rules are executed based on priority. However, there's a significant difference in how synchronous and asynchronous rules respect priority:

- **Sync rules**: Execute sequentially in priority order. Short-circuiting works correctly - if a sync rule at priority 0 fails, rules at priority 1 and higher won't execute.
- **Async rules**: All fire off in parallel regardless of priority. While they are sorted by priority when iterated, each async rule is started without waiting for completion, so all async rules effectively run concurrently.

This means:
1. An async rule at priority 0 could fail, but async rules at priority 1 would have already started running
2. Priority-based short-circuiting doesn't work for async rules
3. There's no way to enforce sequential execution of async rules when required

## Goals

1. Allow async rules to honor priority alongside sync rules
2. Provide a property on async rules indicating whether they can run serially or in parallel
3. Default to parallel execution (maintaining backward compatibility with current behavior)
4. Enable short-circuiting behavior for async rules when running serially

## Current Implementation Analysis

### Location
- `Source/Csla/Rules/BusinessRules.cs` - Main rule execution logic
- `Source/Csla/Rules/BusinessRuleBase.cs` - Base class for rules with `Priority` property
- `Source/Csla/Rules/BusinessRuleAsync.cs` - Base class for async rules
- `Source/Csla/Rules/IBusinessRule.cs` - Interface definitions including `IBusinessRuleAsync`

### Current Flow (BusinessRules.cs - RunRules method)

1. Rules are sorted by priority (`orderby r.Priority`)
2. Loop through each rule:
   - Sync rules: Execute, wait for completion, process results
   - Async rules: Fire off via `RunAsyncRule()` (async void), continue immediately to next rule
3. Short-circuiting check only happens between iterations (line 1004-1006), but async rules have already started

### Key Code Sections

```csharp
// Rules are ordered by priority
var rules = from r in TypeRules.Rules
            where r.PrimaryProperty == null
              && CanRunRule(_applicationContext, r, executionContext)
            orderby r.Priority
            select r;

// In RunRules method:
foreach (var rule in rules)
{
    // Short-circuit check - but async rules already started
    if (anyRuleBroken && rule.Priority > ProcessThroughPriority)
        break;
    
    // Async rules fire and continue immediately
    if (rule is IBusinessRuleAsync asyncRule)
        RunAsyncRule(asyncRule, context, handler);  // async void, doesn't wait
}
```

## Proposed Solution

### 1. New Property on IBusinessRuleBase

Add a new property to control async rule execution behavior:

```csharp
// In IBusinessRule.cs - add to the IBusinessRuleBase interface
// Note: The IsAsync property already exists in IBusinessRuleBase (see line 86 of IBusinessRule.cs)
public interface IBusinessRuleBase
{
    // ... existing properties including IsAsync ...
    
    /// <summary>
    /// Gets a value indicating whether this async rule should run serially
    /// (respecting priority and short-circuiting) or in parallel with other
    /// async rules. Only applies to async rules; sync rules always run serially.
    /// Default is Parallel for backward compatibility.
    /// </summary>
    AsyncRuleExecutionMode AsyncExecutionMode { get; }
}

/// <summary>
/// Defines how an async rule should be executed relative to other rules.
/// </summary>
public enum AsyncRuleExecutionMode
{
    /// <summary>
    /// Rule runs in parallel with other async rules (current behavior, default).
    /// The rule will be started and execution continues to the next rule
    /// without waiting for completion.
    /// </summary>
    Parallel = 0,
    
    /// <summary>
    /// Rule runs serially, respecting priority order. The rule engine will
    /// await completion of this rule before proceeding to rules with higher
    /// priority values. Enables short-circuiting behavior for async rules.
    /// </summary>
    Serial = 1
}
```

### 2. Implementation in BusinessRuleBase

```csharp
// In BusinessRuleBase.cs
private AsyncRuleExecutionMode _asyncExecutionMode = AsyncRuleExecutionMode.Parallel;

/// <summary>
/// Gets or sets the async execution mode for this rule.
/// </summary>
public AsyncRuleExecutionMode AsyncExecutionMode
{
    get => _asyncExecutionMode;
    set
    {
        CanWriteProperty(nameof(AsyncExecutionMode));
        _asyncExecutionMode = value;
    }
}
```

### 3. Modified RunRules Method

The `RunRules` method in `BusinessRules.cs` needs to be modified to:

1. Group rules by priority
2. For each priority group:
   - Execute all sync rules sequentially (current behavior)
   - Execute async rules based on their `AsyncExecutionMode`:
     - `Parallel`: Fire off without waiting (current behavior)
     - `Serial`: Await completion before moving to next rule
3. After processing a priority group, check for short-circuiting before moving to the next group
4. Wait for all serial async rules in current priority to complete before checking short-circuit

### 4. Conceptual Algorithm

```
For each priority level P (low to high):
    
    # Get all rules at this priority
    rulesAtP = rules.Where(r => r.Priority == P)
    syncRules = rulesAtP.Where(r => !r.IsAsync)
    asyncParallelRules = rulesAtP.Where(r => r.IsAsync && r.AsyncExecutionMode == Parallel)
    asyncSerialRules = rulesAtP.Where(r => r.IsAsync && r.AsyncExecutionMode == Serial)
    
    # Execute sync rules
    For each syncRule in syncRules:
        Execute(syncRule)
        Check for explicit short-circuit (StopProcessing)
    
    # Start all parallel async rules
    For each asyncRule in asyncParallelRules:
        FireAndForget(asyncRule)  # Current behavior
    
    # Execute serial async rules sequentially
    For each asyncRule in asyncSerialRules:
        await Execute(asyncRule)
        Check for explicit short-circuit (StopProcessing)
    
    # Check for implicit short-circuiting before next priority
    If anyRuleBroken AND nextPriority > ProcessThroughPriority:
        Break (don't process higher priority rules)
```

### 5. Alternative Approach: Priority-Group-Based Execution

A cleaner approach might be to process rules in priority groups:

```
Group rules by Priority into priorityGroups (ordered by Priority)

For each group in priorityGroups:
    currentPriority = group.Key
    
    # Check if we should stop before processing this group
    If anyRuleBroken AND currentPriority > ProcessThroughPriority:
        Break
    
    parallelTasks = new List<Task>()
    
    For each rule in group:
        If rule.IsAsync:
            If rule.AsyncExecutionMode == Serial:
                # Wait for previous parallel tasks first
                await Task.WhenAll(parallelTasks)
                parallelTasks.Clear()
                
                # Execute and wait
                await ExecuteAsyncRule(rule)
                ProcessResults(rule)
            Else:  # Parallel
                parallelTasks.Add(ExecuteAsyncRule(rule))
        Else:  # Sync
            # Wait for previous parallel tasks first
            await Task.WhenAll(parallelTasks)
            parallelTasks.Clear()
            
            ExecuteSyncRule(rule)
            ProcessResults(rule)
    
    # Wait for any remaining parallel tasks before moving to next priority
    await Task.WhenAll(parallelTasks)
```

## Breaking Changes Assessment

### Low Risk (Backward Compatible)
- Default value of `AsyncExecutionMode` is `Parallel` - existing behavior preserved
- Existing async rules continue to work without modification
- No changes to rule interfaces required for basic usage

### Medium Risk
- The internal `RunRules` method signature may need to become async
- Methods that call `RunRules` (like `CheckRulesForProperty`) may need modifications
- The callback pattern used for async rule completion may need adjustment

### Considerations
- Thread safety: Lock usage needs review for new async/await patterns
- Performance: Awaiting serial rules adds latency, but this is by design
- Completion callbacks: The current callback-based completion may need to coexist with await-based

## Implementation Steps

### Phase 1: Add New Types and Properties
1. Add `AsyncRuleExecutionMode` enum to `IBusinessRule.cs`
2. Add `AsyncExecutionMode` property to `IBusinessRuleBase` interface
3. Implement property in `BusinessRuleBase` class with default `Parallel`
4. Add XML documentation

### Phase 2: Modify Rule Execution Engine
1. Modify `RunRules` method to support the new execution modes
2. Add helper method to group rules by priority
3. Implement priority-group-based execution logic
4. Update callback handling for serial async rules
5. Ensure thread safety with existing locking mechanism

### Phase 3: Update Related Methods
1. Review and update `CheckRulesForProperty`
2. Review and update `CheckObjectRules`
3. Review async rule completion callback logic
4. Update `RunAsyncRule` helper method if needed

### Phase 4: Testing
1. Unit tests for new `AsyncExecutionMode` property
2. Integration tests for serial async rule execution
3. Tests for short-circuiting with serial async rules
4. Tests for mixed parallel/serial async rules
5. Backward compatibility tests
6. Performance benchmarks

### Phase 5: Documentation
1. Update XML documentation
2. Update user documentation
3. Add migration guide if needed
4. Add code samples showing usage

## Open Questions

1. **Should serial async rules at the same priority run in sequence or can they run in parallel with each other?**
   - Option A: All serial rules at same priority run one after another
   - Option B: Serial rules at same priority can run in parallel, but all must complete before next priority
   - Recommendation: Option A for maximum control

2. **How to handle a mix of sync and async rules at the same priority?**
   - Option A: Run sync first, then async
   - Option B: Maintain original order within priority group
   - Recommendation: Option B to preserve developer intent

3. **Should there be a timeout for serial async rules?**
   - Could prevent one slow rule from blocking the entire validation
   - Could use existing `CheckRulesAsync` timeout mechanism

4. **Should the `ProcessThroughPriority` property affect serial async rules?**
   - Current behavior allows continuing past broken rules up to a certain priority
   - Should serial async rules respect this?
   - Recommendation: Yes, maintain consistent behavior

## Timeline Estimate

- Phase 1: 1-2 days
- Phase 2: 3-5 days
- Phase 3: 2-3 days
- Phase 4: 3-5 days
- Phase 5: 1-2 days

**Total: 10-17 days**

## References

- Current implementation: `Source/Csla/Rules/BusinessRules.cs`
- Base classes: `Source/Csla/Rules/BusinessRuleBase.cs`, `Source/Csla/Rules/BusinessRuleAsync.cs`
- Interfaces: `Source/Csla/Rules/IBusinessRule.cs`
- Rule context: `Source/Csla/Rules/RuleContext.cs`
