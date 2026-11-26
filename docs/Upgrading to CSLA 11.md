# Upgrading to CSLA 11

CSLA 11 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 10 to CSLA 11.

If you are upgrading from a version of CSLA prior to 10, you should review the [Upgrading to CSLA 10](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%2010.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 10 to CSLA 11.

## Platform Support

CSLA 11 supports .NET 10 and .NET 11, and continues to support .NET Framework 4.6.2 and later. CSLA 11 no longer targets .NET 8 or .NET 9.

On .NET 11, Android apps that use `Csla.Maui` require Android API level 24 or later, because .NET 11 for Android requires it. The `net10.0-android` target still supports API level 21.

## Source Generators Bundled in the Csla Package

The CSLA source generators now ship inside the `Csla` NuGet package and run automatically in any C# project that references `Csla` ([#4359](https://github.com/MarimerLLC/csla/issues/4359), [#4925](https://github.com/MarimerLLC/csla/pull/4925)).

### Remove the retired generator package

The `Csla.Generator.AutoImplementProperties.CSharp` package is retired. Referencing it alongside `Csla` 11 would run the generators twice, so the build fails with `CSLABUILD001` while the reference remains. Remove its `PackageReference`, and its `PackageVersion` entry if you use central package management:

```xml
<!-- Remove -->
<PackageReference Include="Csla.Generator.AutoImplementProperties.CSharp" Version="10.*" />
```

The `[CslaImplementProperties]`, `[CslaImplementPropertiesInterface<T>]` and `[CslaIgnoreProperty]` attributes are now part of `Csla.dll`, in the same `Csla` namespace, so code that uses them doesn't change.

### Make business classes `partial`

For each `partial` class that declares data portal operation methods, the generator adds code that lets the data portal call those methods directly instead of using reflection. This also means IDEs, analyzers and the trimmer see private operation methods as used.

Classes that aren't `partial` still work through reflection, but the new `CSLA0024` analyzer reports a warning for each class (or containing type) with operation methods that isn't `partial`. If your projects treat warnings as errors, the build fails until you fix them. The analyzer's code fix adds the `partial` modifier to the class and its containing types. See [CSLA0024](analyzers/CSLA0024-TypeWithOperationsShouldBePartialAnalyzer.md).

```csharp
[Serializable]
public partial class Customer : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id)
  {
    // ...
  }
}
```

Because the data portal now calls operation methods through generated code, stack traces from operation methods go through generated `IDataPortalOperations` members, not reflection.

### Strongly typed data portal extension methods

You can mark a class (or an assembly) with `[DataPortalExtensions]` to generate strongly typed extension methods on `IDataPortal<T>` and `IChildDataPortal<T>`. See the [release notes](../releasenotes.md) for details. This is opt-in. Once you use it, the `CSLA0025` analyzer warns when code calls the untyped `params object[]` data portal methods for that type when a generated method could be used instead. See [CSLA0025](analyzers/CSLA0025-UseGeneratedDataPortalExtensionAnalyzer.md).

## Legacy Data Portal Operation Method Names Removed

In earlier versions, if the data portal couldn't find a method with an operation attribute (such as `[Fetch]`), it fell back to looking for a method by name (such as `DataPortal_Fetch` or `Child_Update`). This fallback was controlled by the `DataPortalOptions.UseLegacyOperationMethods` option.

In CSLA 11 this fallback is removed. The data portal only invokes methods that have an operation attribute. The `UseLegacyOperationMethods` option is also removed.

If your business classes have operation methods with no attribute, add the matching attribute:

| Legacy method name | Attribute |
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

The same applies to the `Async` variants of these names (for example `DataPortal_FetchAsync`).

Before:

```csharp
[Serializable]
public class Customer : BusinessBase<Customer>
{
  private void DataPortal_Fetch(int id)
  {
    // ...
  }
}
```

After:

```csharp
[Serializable]
public class Customer : BusinessBase<Customer>
{
  [Fetch]
  private void DataPortal_Fetch(int id)
  {
    // ...
  }
}
```

Once a method has an attribute, you can rename it to anything you like (for example `Fetch`).

A method with a legacy name and no attribute is never called. Usually the data portal then throws an exception at runtime because it can't find a matching operation method. But when a CSLA base class has its own operation method, the base class method runs instead and your method is silently skipped. For example:

* A child class with an attribute-less `Child_Create()` gets `BusinessBase.Child_Create()`, which only checks business rules.
* A list class that overrides `Child_Update(params object[])` without `[UpdateChild]` gets the base `Child_UpdateAsync`, which updates the child items but not anything else your override did.

Use the `CSLA0014` analyzer (below) to find these methods rather than relying on runtime errors.

### Finding affected methods

The `CSLA0014` analyzer finds methods that use a legacy operation name but have no operation attribute. In CSLA 11 it reports a warning instead of an informational message. The analyzer's code fix adds the correct attribute for you. See [CSLA0014](analyzers/CSLA0014-DoesOperationHaveAttributeAnalyzer.md).

### Base class changes

* **Default child create:** `BusinessBase`, `BusinessListBase`, and `BusinessBindingListBase` each have a `protected virtual Child_Create()` method that is now marked with `[CreateChild]`. It's still called when you create a child with no criteria and your class has no better matching `[CreateChild]` method. `BusinessBase.Child_Create()` checks the object's business rules. If you override `Child_Create()`, the override inherits the attribute, so you don't need to add one.
* **"Not supported" operations:** `CommandBase`, `ReadOnlyBase`, `ReadOnlyListBase`, `ReadOnlyBindingListBase`, `NameValueListBase`, `DynamicListBase`, and `DynamicBindingListBase` no longer have private `DataPortal_XYZ` methods that throw `NotSupportedException` (for example, calling update on a read-only object). Those calls now fail with the data portal's normal "method not found" error.
* **ObjectFactory child create:** creating a child of a type that uses `[ObjectFactory]` no longer looks for a method named `Child_Create` on the factory or the business class. The data portal calls the business class's `[CreateChild]` method (or the base class default described above).
* **Sync `Child_Update`:** the synchronous `Child_Update(params object[])` helper on `BusinessListBase`, `BusinessBindingListBase`, and `BusinessDocumentBase` has no operation attribute. You can still call it from your own synchronous `[Update]`, `[Insert]`, or `[DeleteSelf]` methods. The data portal itself calls `Child_UpdateAsync`, which is marked with `[UpdateChild]`. If you override `Child_Update` so the data portal calls your override, add `[UpdateChild]` to it, or override `Child_UpdateAsync` instead.

## `LazySingleton<T>` parameterless constructor removed

The parameterless constructor of `Csla.LazySingleton<T>` has been removed ([#4891](https://github.com/MarimerLLC/csla/issues/4891)). It could not be used successfully:

* Creating an instance with `new LazySingleton<T>()` and reading `Value` threw a `NullReferenceException`, because the `ApplicationContext` it relied on was only set when the instance was created through the data portal activator.
* Creating the instance through DI (`ApplicationContext.CreateInstanceDI`) failed because the type had two public constructors, which `ActivatorUtilities` treats as ambiguous.
* Even when an `ApplicationContext` was available, creating `T` failed for any type with more than one public constructor (such as `Dictionary<TKey, TValue>`).

`LazySingleton<T>` no longer implements `IUseApplicationContext`. The only constructor is now `LazySingleton(Func<T>)`, so supply a factory delegate:

```csharp
// Before
var lazy = new LazySingleton<Dictionary<string, object>>();

// After
var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
```

If the value needs services from DI, create it in the delegate, for example `() => (MyType)applicationContext.CreateInstanceDI(typeof(MyType))`. Alternatively, use `System.Lazy<T>`, which covers the same use case.

## `AllowNew` and `AllowRemove` are enforced

Editable lists now enforce their `AllowNew` and `AllowRemove` properties ([#4848](https://github.com/MarimerLLC/csla/issues/4848)). Previously these properties only informed data binding, and code could still add or remove items when they were `false`. This applies to `BusinessListBase`, `BusinessBindingListBase`, `DynamicListBase` and `DynamicBindingListBase`.

* When `AllowNew` is `false`, `AddNew`, `AddNewAsync`, `Add`, `AddRange` and `Insert` throw `NotSupportedException`.
* When `AllowRemove` is `false`, `Remove`, `RemoveAt`, `Clear` and `RemoveItemAsync` throw `NotSupportedException`. For `DynamicBindingListBase` the exception is thrown before the item is deleted from the database.
* Replacing an item through the indexer (`list[i] = item`) is still allowed.
* N-level undo, deserialization and graph merging still add and remove items as needed.

`AllowRemove` now defaults to `true` for `BusinessListBase` and `DynamicListBase`, matching `BusinessBindingListBase` and `DynamicBindingListBase`. `AllowNew` keeps its value when the data portal returns the list. Previously it was reset to `true` when the list's `ApplicationContext` was set.

If your data access code sets `AllowNew` or `AllowRemove` to `false` before loading items, move the assignment after the items are loaded:

```csharp
[Fetch]
private void Fetch([Inject] IChildDataPortal<LineItem> childPortal, [Inject] ILineItemDal dal)
{
  using (LoadListMode)
  {
    foreach (var data in dal.Fetch())
      Add(childPortal.FetchChild(data));
  }
  AllowNew = false;
  AllowRemove = false;
}
```

## Async Authorization Rules in View Models and `IsSavable`

Async authorization rules (`IAuthorizationRuleAsync`) are now supported in view models and in `IsSavable` ([#4707](https://github.com/MarimerLLC/csla/issues/4707), [#4947](https://github.com/MarimerLLC/csla/issues/4947), [#4948](https://github.com/MarimerLLC/csla/pull/4948)). Synchronous rules behave as before. If you use async rules, these behavior changes affect your UI code.

### View models

* The XAML `ViewModelBase` and `ObjectStatus`, and the Blazor `ViewModel`, evaluate authorization rules with `HasPermissionAsync`.
* With async rules, the permission-based properties (such as `CanCreateObject` and `CanEditObject`, and in XAML `CanCreate`, `CanDelete`, `CanAddNew` and `CanRemove`) start out `false` and update when the rules complete, raising `PropertyChanged`.
* The Blazor `ViewModel.RefreshAsync` waits for the object-level permissions, and the `Can*Object` getters no longer throw for async rules.

### `IsSavable`

`IsSavable` on `BusinessBase`, `BusinessListBase` and `BusinessBindingListBase` checks the per-type `CreateObject`, `EditObject` or `DeleteObject` rule:

* A rule that completes synchronously (a sync rule, or an async rule that doesn't await) is evaluated on every read, as before.
* A rule that is still running marks the object busy, and `IsSavable` returns `false`. Reading `IsSavable` can therefore raise `BusyChanged`.
* When the rule completes, the result is cached for that action (new, existing or deleted) and user. The object is marked idle, and `PropertyChanged` is raised for `IsSavable`. The result is re-evaluated when the action or the user changes, but not when other properties change.
* If the rule fails, `IsSavable` is `false` and `UnhandledAsyncException` is raised.
* Calling `Save` while the check is still running throws, the same as saving an object with running async business rules. Wait for the object to become idle first (for example with `WaitForIdle`). The XAML view model and the Blazor `SaveAsync` already wait for it.

### `HasPermission` with async rules

The synchronous `BusinessRules.HasPermission` now accepts an async rule if its task has already completed. If the rule is still running, `HasPermission` throws an `InvalidOperationException` that tells you to use `HasPermissionAsync`. Previously any async rule threw `ArgumentOutOfRangeException`.

## `CheckRules()` runs each rule once, then reruns only what changed

`BusinessRules.CheckRules()` (and `CheckRulesAsync()`) now runs rules as a batch ([#3736](https://github.com/MarimerLLC/csla/issues/3736)). Previously each property's rules ran in turn and recursively reran the rules of its affected properties and of every rule using it as an input, so objects with many interdependent rules ran some rules dozens or hundreds of times. Now:

1. Object-level rules run first, as before.
2. Every property rule runs once, in priority order across all properties ([#4235](https://github.com/MarimerLLC/csla/issues/4235)). Previously priority only ordered the rules within one property. Rules with the same priority run in the CSLA 10 order: by property, in the order the property's first rule was added, then in the order the rules were added.
3. If rules changed property values through output values (`context.AddOutValue`), the rules for those properties, and the rules that use them as input properties, run again, once.
4. With `CascadeOnDirtyProperties = true`, step 3 repeats until no rule changes a value.

`CheckRules(property)`, the rules that run when a property is set, and `CheckObjectRules()` are unchanged.

Behavior changes to check for:

* **Reruns depend on changed values.** Rules rerun only when a value changed through rule output values. A rule that changes a property some other way, such as writing to the business object directly, no longer causes the rules that depend on that property to rerun during `CheckRules()`.
* **Affected properties don't rerun on their own.** The rules of a rule's `AffectedProperties` no longer rerun during `CheckRules()` unless a value changed. They already ran once in the batch. The affected properties are still in the property names `CheckRules()` returns.
* **`CascadeIfDirty` has no effect in `CheckRules()`**, because every rerun already requires a changed value. It still applies to `CheckRules(property)`.
* **Rules that never settle throw.** With `CascadeOnDirtyProperties = true`, rules whose output values keep changing each other used to recurse until the process crashed with a `StackOverflowException`. `CheckRules()` now throws an `InvalidOperationException` naming the properties that kept changing after `MaxRerunPasses` rerun passes (default 100).
* Execution order and the number of times each rule runs change, so any code that depends on either may need updating. Short-circuiting (`ProcessThroughPriority` and `StopProcessing`) still applies per property, as before.

To configure the new behavior:

```csharp
services.AddCsla(o => o
  .Rules(r => r.UseBatchCheckRules(b => b.MaxRerunPasses = 200)));   // default 100
```

To go back to the CSLA 10 behavior while you update your rules:

```csharp
services.AddCsla(o => o
  .Rules(r => r.UseLegacyCheckRules()));
```

`UseLegacyCheckRules()` is temporary and will be removed in a future version. Each method takes only the settings that apply to that behavior. If you call both, the last call wins.

## Sequential async rules

Async business rules can now run in priority order like sync rules ([#5014](https://github.com/MarimerLLC/csla/issues/5014)). An async rule normally starts when the rules reach it and then runs in the background, so a rule that is async only because it reads a database can't stop later rules from running. Set `IsSequential` to run it in the rule stream instead:

```csharp
BusinessRules.AddRule(new CustomerExists(CustomerIdProperty) { Priority = 1, IsSequential = true });
```

For a sequential async rule, during `CheckRules()`, `CheckRules(property)` and `CheckObjectRules()`:

* The rules after it, for every property, wait until it completes. Before it starts, the async rules that started earlier in the same run complete, and their results count.
* Its result short-circuits later rules like a sync rule's: a broken rule with `Error` severity stops the rules of its property above `ProcessThroughPriority`, and `StopProcessing` stops them. Its output values feed the `CheckRules()` rerun pass, and its affected properties cascade in `CheckRules(property)`.
* While it runs, its affected properties and the properties whose rules wait for it are busy, and the object is busy until the rules after it have run. `WaitForIdle()`, `CheckRulesAsync()` and `ValidationComplete` wait for the whole run.
* The synchronous `CheckRules()` returns when the rules reach a sequential rule that is still running, with the properties affected so far. The rules after it run on the thread that completes the rule and raise `PropertyChanged` for the properties they affect.
* An async rule that is not followed by a sequential rule completes as it always did, in the background, however quickly it finishes. If it completes after the run ended, the rules of its affected properties run as their own run, so sequential rules among them still wait.
* If a property's rules run again (the property is set, or `CheckRules()` is called) while a run waits for a sequential rule, the waiting run is discarded: its remaining rules never run, and the results of the async rules it started are dropped when they complete, because the new run reruns them. A discarded `CheckRules()` or `CheckObjectRules()` run starts again after the new run, so the rules it had not reached still run.
* A sequential rule that throws completes with a broken rule (`RuleName:message`), which short-circuits like any error, instead of raising an unhandled exception.

`IsSequential` is `true` for sync rules and `false` by default for async rules, so existing rules behave as before. The legacy runner (`UseLegacyCheckRules()`) ignores it.

Behavior changes to check for:

* **`IBusinessRuleBase.IsSequential`.** If you implement `IBusinessRuleBase` (or `IBusinessRule`/`IBusinessRuleAsync`) without inheriting `BusinessRuleBase`, add the property: `true` for a sync rule, `false` for an async rule.
* **`CheckRulesAsync()` returns more property names.** With the batch runner, `CheckRulesAsync()` and `CheckRulesAsync(property)` also return the affected and output properties of the async rules they started, and of the rules those rules' completions ran, even without sequential rules. Before, they returned only what the synchronous part of the run affected. The synchronous `CheckRules()` result is unchanged.

## Async rule completion reports idle last

When an async business rule completes, the object now reports itself idle as the very last step ([#4991](https://github.com/MarimerLLC/csla/issues/4991)). Previously the object became idle partway through the completion, so `WaitForIdle()` and `CheckRulesAsync()` could return, and code that disposed resources (such as a DI scope) could run, while the rest of the completion notifications (which can run authorization rules and use DI services) were still to come.

Behavior changes to check for:

* **Event order.** For a completed async rule, `PropertyChanged` for the rule's properties, the `IsSelfValid`, `IsValid` and `IsSavable` property changes, and `ValidationComplete` are raised **before** `BusyChanged(property, false)`. Handlers for these events now see `IsBusy == true` and `IsPropertyBusy(property) == true`, where they used to see `false`.
* **Idle notifications.** When a property becomes idle, `PropertyChanged` is raised for `IsSavable` (it is always `false` while the object is busy), then for `IsSelfBusy` and `IsBusy`, then `BusyChanged`. `BusinessBase.OnBusyChanged` now raises the `IsSelfBusy` and `IsBusy` property changes before the `BusyChanged` event rather than after it. If a rule affects several properties, `IsBusy` becomes `false` with the last property's `BusyChanged`. A parent `BusinessBase` raises `PropertyChanged` for `IsSavable` whenever a child's busy state changes. These idle notifications are raised after `IsBusy` turns `false` (they report that change), so don't dispose resources the object uses from inside their handlers.
* **Continuations run asynchronously.** Code after `await WaitForIdle()` (`BusyHelper.WaitForIdle`) or `await CheckRulesAsync()` (`AsyncManualResetEvent`) always continues asynchronously, never inline on the thread that completed the rule. Code that relied on running before the remaining completion notifications now runs after them.
* **`IHostRules`.** `RuleComplete(IPropertyInfo)` no longer raises `BusyChanged`; the new `PropertyIdle(IPropertyInfo)` method is called when no async rules are running for the property any more. If you implement `IHostRules` yourself, add `PropertyIdle` and raise your busy notification there.
* **`CommandBase` waits for its child objects.** After a create, fetch or execute operation, the server-side data portal now waits until the objects a command holds in managed properties are idle. Previously it returned (and a remote data portal serialized) the command right away, so the result of a child's still-running async rule was lost. A command whose child has a long-running async rule now takes that long to return, and fails with a `TimeoutException` after `DefaultWaitForIdleTimeoutInSeconds`.

`WaitForIdle()` on an object covers its child objects. If a completion notification starts rules on an object outside that graph, wait for that object separately. For a `CommandBase`, the data portal covers the objects in the command's managed properties; an object the command keeps in a private field isn't covered, so wait for it in your operation method.

## Transaction settings resolved at runtime and `ITransactionScopeFactory`

Transaction settings from `TransactionalAttribute` are now resolved when the transaction is created, not when the attribute is constructed ([#4780](https://github.com/MarimerLLC/csla/issues/4780)). Previously the attribute copied the defaults from `DataOptions` into itself. Because CSLA caches the attribute, later configuration changes were ignored.

* `TransactionalAttribute` no longer reads `DataOptions`. Any value the attribute does not specify uses the configured default when the transaction is created:
  * `TransactionIsolationLevel` defaults to `TransactionIsolationLevel.Unspecified`.
  * `TimeoutInSeconds` is now `int?`, and is `null` when not specified.
  * `AsyncFlowOption` is now `TransactionScopeAsyncFlowOption?`, and is `null` when not specified.
* `DataOptions` values (`DefaultTransactionIsolationLevel`, `DefaultTransactionTimeoutInSeconds`, `DefaultTransactionAsyncFlowOption`) are now per-instance. They were previously stored in static fields shared by every `CslaOptions` instance.
* With `TransactionalTypes.EnterpriseServices` (.NET Framework only), an unspecified isolation level now uses `DataOptions.DefaultTransactionIsolationLevel`. If that is also unspecified, `Serializable` is used. Previously this threw an `ArgumentOutOfRangeException`.

The server-side data portal now gets the `TransactionScope` from an `ITransactionScopeFactory` service. The default implementation, `DefaultTransactionScopeFactory`, uses the attribute values and falls back to `DataOptions`. To control how transactions are created, implement `ITransactionScopeFactory` (or subclass `DefaultTransactionScopeFactory`) and register it:

```csharp
services.AddCsla(o => o
  .DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg
    .RegisterTransactionScopeFactory<MyTransactionScopeFactory>())));
```

The factory receives a `TransactionScopeFactoryContext` describing the operation:

* `ObjectType`, `Operation`, `Method` / `MethodName`, and the `TransactionalAttribute`.
* `BusinessObject`: the object being inserted, updated, deleted or executed. It is `null` for operations that use criteria.
* `Criteria`: the criteria values for create, fetch, execute and delete operations. The list is empty when there are no criteria.

These let a factory adjust the transaction to the data involved. For example, it can use a longer timeout for a large batch. `BusinessObject` is the server-side instance before the operation runs. Treat it as read-only.

The `TransactionalDataPortal` constructor now takes `(DataPortalBroker, ITransactionScopeFactory, TransactionScopeFactoryContext)` instead of `(DataPortalBroker, TransactionalAttribute)`.

## Obsolete XML serialization readers and writers removed

The `DataContractSerializer` based MobileFormatter readers and writers have been removed ([#3347](https://github.com/MarimerLLC/csla/issues/3347)):

* `CslaXmlReader` and `CslaXmlWriter`
* `CslaXmlBinaryReader` and `CslaXmlBinaryWriter`
* `CslaReaderWriterFactory.GetDataContractSerializer()`

They were marked `[Obsolete]` in CSLA 9 ([#4336](https://github.com/MarimerLLC/csla/pull/4336)), and `DataContractSerializer` isn't compatible with trimming or NativeAOT.

If your configuration selects one of these types through the `MobileReader<T>()` or `MobileWriter<T>()` options, remove those calls to use the default `CslaBinaryReader` and `CslaBinaryWriter`. The serialized format changes, so the client and the server must be updated together.

## `StateMode` is obsolete

`StateMode` and the `OnGetState(SerializationInfo, StateMode)` and `OnSetState(SerializationInfo, StateMode)` overloads are marked `[Obsolete]` ([#4582](https://github.com/MarimerLLC/csla/issues/4582)). Older versions of n-level undo took snapshots by calling these methods with `StateMode.Undo`. Undo now takes its snapshots through the configured `ISerializationFormatter`, so the methods are only ever called with `StateMode.Serialization`, and the parameter carries no information.

Override the new overloads that take only the `SerializationInfo`:

```csharp
protected override void OnGetState(SerializationInfo info)
{
  base.OnGetState(info);
  info.AddValue("_custom", _custom);
}

protected override void OnSetState(SerializationInfo info)
{
  base.OnSetState(info);
  _custom = info.GetValue<string>("_custom");
}
```

Existing overrides of the obsolete overloads keep working: they still run, and `base.OnSetState(info, mode)` still restores the base class state before your code runs. The compiler reports warnings `CS0672` and `CS0618` until you move them. If an override checks `mode == StateMode.Undo`, delete that branch, because that code hasn't run for several versions. The obsolete members will be removed in a future major version.

## `LazyGetPropertyAsync` and `LazyReadPropertyAsync` take a factory

`LazyGetPropertyAsync` and `LazyReadPropertyAsync` in `BusinessBase` and `ReadOnlyBase` have overloads that take a `Func<Task<P>>`, and the overloads that take a `Task<P>` are marked `[Obsolete]` ([#5018](https://github.com/MarimerLLC/csla/issues/5018)). A `Task<P>` is already running by the time it's passed in, so a getter that creates it starts another load on every read, and every result after the first is thrown away. The factory only runs when the value isn't loaded and isn't already loading.

Wrap the task in a lambda:

```csharp
// CSLA 10
public ShipmentInfo? Shipment =>
  LazyGetPropertyAsync(ShipmentProperty,
    ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));

// CSLA 11
public ShipmentInfo? Shipment =>
  LazyGetPropertyAsync(ShipmentProperty,
    () => ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));
```

A getter that checks `FieldManager.FieldExists` and `PropertyIsLoading` before calling `LazyGetPropertyAsync` can drop the check, along with the CSLA0007 suppression it needed.

The factory runs after the property is marked as loading. If it throws, returns `null`, or returns a task that fails, the object raises `UnhandledAsyncException` and the getter returns the default value; the next read calls the factory again. This also applies to the `Task<P>` overloads: a task that has already failed is reported through `UnhandledAsyncException` instead of making the getter throw `InvalidOperationException`. The compiler reports `CS0618` for calls to the `Task<P>` overloads until you change them. A call that passes `null` as the second argument no longer compiles (`CS0121`), because it matches both overloads.

## CSLA0007 reports more property getters

Analyzer [CSLA0007](analyzers/CSLA0007-EvaluatePropertiesForSimplicityAnalyzer.md) reports properties with managed backing fields that do more than call a property method. In CSLA 11 it checks more getters ([#5016](https://github.com/MarimerLLC/csla/issues/5016), [#5020](https://github.com/MarimerLLC/csla/issues/5020)):

* **New warnings.** Expression-bodied properties that call a property method inside another call are now reported, for example `public string Name => GetProperty(NameProperty).ToUpper();`. CSLA 10 missed them. If your projects treat warnings as errors, the build fails until you fix them. Move the extra logic into a separate property that reads the managed property, into a business rule, or suppress the warning with a justification.
* **Warning location.** For `get =>`, `get { ... }` and `set { ... }` accessors that call a property method inside another call, the warning is now reported on the accessor's expression or body rather than on the whole accessor.
* **Fewer false positives.** A getter that applies the null-forgiving operator to a property method call, such as `get => GetProperty(NameProperty)!;`, is no longer reported, so you can remove those suppressions. See [Nullable reference types and CSLA properties](Nullable-Reference-Types.md).
* **Generated properties.** Properties generated for `[CslaImplementProperties]` and `[CslaImplementPropertiesInterface<T>]` are no longer wrapped in `#pragma warning disable CSLA0007` ([#5019](https://github.com/MarimerLLC/csla/issues/5019)). They only call property methods, so they don't produce warnings.

## Custom `MobileFormatter` serializers can be declared with attributes

Analyzer CSLA0010 reports data portal operation arguments whose types aren't serializable. Before CSLA 11 it also reported types handled by a `MobileFormatter` custom serializer, because those were registered only in runtime code the analyzer can't see ([#4428](https://github.com/MarimerLLC/csla/issues/4428)). You can now declare the serializer, and the analyzer reads the declaration:

```csharp
[MobileSerializer(typeof(MoneySerializer))]       // on a type you own
public class Money { ... }

[MobileSerializerFor(typeof(ThirdParty.Widget))]   // on a serializer, for a type you don't own
public class WidgetSerializer : IMobileSerializer { ... }

builder.Services.AddCsla(o => o
  .Serialization(s => s
    .UseMobileFormatter(m => m.AddCustomSerializers(typeof(Money).Assembly))));
```

Existing code that registers serializers with `CustomSerializers.Add(new TypeMap<T, S>(...))` keeps working, and is still the way to register generic types. If you move to the attributes, pass each assembly that declares serializers to `AddCustomSerializers`; an assembly that isn't passed satisfies the analyzer but fails at run time. See [Declaring custom serializers with attributes](serialization.md#declaring-custom-serializers-with-attributes-csla-11).

## Async `SafeDataReader`

`SafeDataReader` has async members ([#4975](https://github.com/MarimerLLC/csla/issues/4975)):

* `ReadAsync`, `NextResultAsync` and `CloseAsync`
* `IsDBNullAsync` and `GetFieldValueAsync<T>`, by ordinal or by column name
* Null-safe getters that return the same defaults as the sync getters, such as `GetStringAsync`, `GetInt32Async`, `GetDateTimeAsync` and `GetSmartDateAsync`
* `DisposeAsync`, so the reader works with `await using`

```csharp
await using var dr = new SafeDataReader(await cm.ExecuteReaderAsync());
while (await dr.ReadAsync())
{
  var name = await dr.GetStringAsync("Name");
  var age = await dr.GetInt32Async("Age");
}
```

These members use the underlying `DbDataReader`, which every ADO.NET provider returns. If the wrapped reader isn't a `DbDataReader`, they run synchronously. The null-safe getters check for null asynchronously and then read the value with the provider's typed getter, so they convert values the same way the sync getters do.

`SafeSqlDataReader` used to declare its own `ReadAsync`, `NextResultAsync` and `GetFieldValueAsync<T>`. It now inherits them from `SafeDataReader`:

* The signatures are the same, including the `ordinal` and `cancellationToken` parameter names and the overloads with and without a `CancellationToken`, so calls, named arguments and method group conversions still compile. Code compiled against an earlier version must be recompiled.
* They no longer throw `NotSupportedException` when the reader isn't a `SqlDataReader`.
* `IsDbNullAsync` is obsolete. Use `IsDBNullAsync`.

## `Microsoft.Data.SqlClient` 7 and Entra ID authentication

`Csla.Data.SqlClient` now references `Microsoft.Data.SqlClient` 7.1.2, up from 5.2.2, so your application gets SqlClient 7 or later.

SqlClient 7 moved Microsoft Entra ID (Azure AD) authentication out of the core package into `Microsoft.Data.SqlClient.Extensions.Azure`. If a connection string uses an `Active Directory` authentication method, such as `Active Directory Default`, `Active Directory Managed Identity` or `Active Directory Interactive`, add that package to the application:

```
dotnet add package Microsoft.Data.SqlClient.Extensions.Azure
```

Without it the application still builds, but opening the connection fails at run time with an error that names the missing package. SQL Server authentication and Windows integrated authentication don't need it.

`Csla.Data.SqlClient` no longer references `Azure.Identity`. That reference only raised the version SqlClient 5 brought in, and SqlClient 7 doesn't depend on `Azure.Identity`. If your code uses `Azure.Identity` directly, reference it yourself.

See the [SqlClient 7.0 release notes](https://github.com/dotnet/SqlClient/releases/tag/v7.0.0) for its other changes.

## Expired Blazor sessions are purged automatically

When Blazor state management is on (`UseInMemoryApplicationContextManager = false`), the server keeps a session for each user in the `ISessionStore`. In CSLA 10, expired sessions were only removed if your app called `ISessionManager.PurgeSessions` on a schedule it set up.

In CSLA 11, `AddServerSideBlazor` registers a hosted background service that does this for you ([#4977](https://github.com/MarimerLLC/csla/issues/4977)). It's on by default whenever Blazor state management is on, and **removes sessions that haven't been touched for 1 hour**. Configure it on the `AddServerSideBlazor` options:

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor(b =>
  {
    b.UseInMemoryApplicationContextManager = false;
    b.SessionPurgeInterval = TimeSpan.FromMinutes(5);  // default: 5 minutes
    b.SessionExpiration = TimeSpan.FromHours(1);       // default: 1 hour
  }));
```

| `BlazorServerConfigurationOptions` property | Default | Purpose |
| --- | --- | --- |
| `PurgeExpiredSessions` | `true` | Runs the purging service. Set it to `false` to turn it off. |
| `SessionPurgeInterval` | 5 minutes | How often the service calls `PurgeSessions` |
| `SessionExpiration` | 1 hour | How long a session can go untouched before it's removed |

What to check when you upgrade:

* If users can be idle for more than an hour and should keep their session, raise `SessionExpiration`.
* If your app already calls `PurgeSessions` on its own schedule, either remove that code or set `PurgeExpiredSessions = false`. Running both is harmless, but the shorter expiration wins.
* With the default `UseInMemoryApplicationContextManager = true`, there are no stored sessions and the service isn't registered.

How the service behaves:

* The first purge runs one interval after the app starts. `SessionPurgeInterval` must be from 1 millisecond to 4,294,967,294 milliseconds (about 49.7 days), which is the range `PeriodicTimer` supports, and `SessionExpiration` must be greater than zero. Otherwise `AddServerSideBlazor` throws `ArgumentOutOfRangeException`.
* It works with every `ISessionStore`, including the in-memory default and the Azure Table Storage store (`Csla.Blazor.State.AzureTableStorage`). A shared store also refreshes the stored last-touched time of live sessions on each purge, so other servers don't remove them. Keep `SessionPurgeInterval` well below `SessionExpiration`.
* A failed purge is logged as a warning, and the next purge runs on schedule.
* If a custom session manager doesn't support purging (it throws `NotSupportedException` or `NotImplementedException`), the service logs a warning and stops. The app keeps running.
* To read the settings from configuration, set them in the delegate, for example `b.SessionExpiration = builder.Configuration.GetValue("Csla:SessionExpiration", TimeSpan.FromHours(1))`.
* It's a server-side feature. Blazor WebAssembly has nothing to purge.

## Calculated properties and `[DoNotSerialize]`

CSLA 11 adds `[CalculatedProperty]` for managed properties of `BusinessBase` types whose value is calculated by business rules from other properties ([#4201](https://github.com/MarimerLLC/csla/issues/4201)). A calculated property:

* isn't serialized, as if it had `[DoNotSerialize]`;
* never changes `IsSelfDirty` or `IsDirty`, whether a rule, `SetProperty` or `LoadProperty` changes its value. `PropertyChanged` is still raised and dependent rules still run;
* is recalculated after the object is deserialized (for example after a data portal call or `Clone()`), by running the rules whose primary property is the calculated property. This happens once the whole object graph has been deserialized, after any `Deserialized()` overrides, and child objects are recalculated before the objects that contain them, so a parent's calculation can use its children's calculated values. Async rules are started but not awaited.

```csharp
public static readonly PropertyInfo<decimal> TotalProperty = RegisterProperty<decimal>(nameof(Total));
[CalculatedProperty]
public decimal Total => GetProperty(TotalProperty);

protected override void AddBusinessRules()
{
  base.AddBusinessRules();
  // CalcSum (from the BusinessRuleDemo sample) sets TotalProperty with context.AddOutValue
  BusinessRules.AddRule(new CalcSum(TotalProperty, Line1Property, Line2Property));
  BusinessRules.AddRule(new Dependency(Line1Property, TotalProperty));
  BusinessRules.AddRule(new Dependency(Line2Property, TotalProperty));
}
```

This replaces calling `MarkOld()` or `MarkClean()` again after `CheckRules` just to undo the dirty state caused by calculated values. Analyzer **CSLA0026** warns when a calculated property has a setter that isn't `private`, and **CSLA0027** warns when `[CalculatedProperty]` is used outside a `BusinessBase` type, where it would stop the value from being serialized without recalculating it. The attribute is intended for simple values, not child objects or collections.

What to check when you upgrade:

* `IPropertyInfo` has a new `IsCalculated` member. If you implement `IPropertyInfo` yourself (rather than deriving from `PropertyInfo<T>`), add it. Return `false` to keep the CSLA 10 behavior.
* A managed property marked `[DoNotSerialize]` (or registered with `isSerializable: false`) that holds an object value, such as a `MobileList<T>` or a child object, is no longer serialized. In CSLA 10 only values of native types such as `int` and `string` were skipped, and object values were still sent. Values also stay excluded after `BeginEdit` and `CancelEdit`; in CSLA 10, cancelling an edit made every `[DoNotSerialize]` value serializable again. If you relied on either behavior, remove `[DoNotSerialize]` from the property.

## ASP.NET Core `CslaModelBinder` binds whole object graphs

In ASP.NET Core MVC and Razor Pages, `CslaModelBinder` now binds editable child objects and editable child lists at any depth, for example `Children[0].GrandChildren[1].Name` ([#4858](https://github.com/MarimerLLC/csla/issues/4858)). Previously it only bound the root object's properties, plus the items of a root list. See [Binding editable object graphs](ASP.NET-MVC.md#binding-editable-object-graphs).

Behavior changes:

* `TryUpdateModelAsync(model)` now binds into the object you pass. Previously the binder created a separate object, so the posted values never reached your object. When binding into an existing object, only posted values are set; other properties and children are left alone.
* The binder now honors `[BindNever]`, `[Bind]` and the include list passed to `TryUpdateModelAsync`. Previously it bound every registered property.
* The binder now waits for async business rules anywhere in the graph to finish before it returns.
* The binder no longer sets a child property to `null` when the form has no fields for it.
* The binder now works out a posted list's items from the indexes in the field names. Previously it divided the number of posted fields by the number of properties, which gave the wrong count when items had child properties or fields weren't posted.
* A request that isn't a form post binds to a new object with no values set, rather than throwing.
* The binder binds only properties that have a public setter ([#5054](https://github.com/MarimerLLC/csla/issues/5054)). Properties with an `init` accessor aren't bound either. Previously it also set properties with a `private`, `protected` or `init` setter, get-only properties, and registered properties with no public property, using `LoadProperty` when there was no setter to call. To bind such a property, give it a public setter, or set it in your controller code. This includes a key with a `private` setter, such as `Id`, posted back in a hidden field: it now keeps its default value, so code that decides between insert and update from it, or saves with it, gets the wrong value. Take the key from the route instead, fetch or create the object, and bind the form into it with `TryUpdateModelAsync` (as `Samples/RazorPagesExample` does), or set the key with `LoadProperty` (as `Samples/MvcExample` does). Binding the key from the form also let a user post another object's key and save over that object.
* When a property setter throws for a posted value, the binder no longer stores the value with `LoadProperty` ([#5054](https://github.com/MarimerLLC/csla/issues/5054)). Previously a value that the setter rejected, including one denied by a write authorization rule, was stored anyway, and could then be saved. Now the value isn't stored and a ModelState error is added under the field's name, so `ModelState.IsValid` and the result of `TryUpdateModelAsync` are `false`. The message is the localized `PropertySetNotAllowed` resource for a denied write, `ModelBinderPropertySetFailed` with the exception's message for an `ArgumentException` or `ValidationException` from the setter, and `ModelBinderPropertyValueNotAccepted` for any other exception. If the setter fails after the new value was written, for example in a `PropertyChanged` handler, the previous value is restored. A property the user can't write that is posted back with its current value, for example from a `readonly` or hidden input, isn't an error, as long as the user can read the property. If they can't read it either, any posted value is an error, so the response doesn't reveal whether a guessed value was right. The comparison allows for what rendering loses: a date posted without its time or seconds, an empty string for a `null` string, and a number formatted in the invariant or current culture. If nothing was posted for a property of a new object and its setter throws anything other than an `ArgumentException` or `SecurityException` for `null`, that exception is no longer swallowed.
* A posted value that can't be converted to the property's type, such as `abc` for an `int`, adds a ModelState error (`ModelBinderPropertyValueNotAccepted`) instead of throwing an exception that failed the request ([#5054](https://github.com/MarimerLLC/csla/issues/5054)). Every binder error also records the posted value in ModelState, so the form shows it again. Fields the tag helpers list in `__Invariant`, such as `<input type="number">`, are parsed with the invariant culture, as MVC's own binders do; other fields still use the current culture.

## Trimming and NativeAOT annotations

CSLA now declares what it needs from the trimmer ([#3347](https://github.com/MarimerLLC/csla/issues/3347)). In a trimmed or NativeAOT app this can surface new `IL2026` warnings in your code.

The late-bound reflection APIs, which accept any object and find its members by name, are marked `[RequiresUnreferencedCode]`:

* `MethodCaller.CallPropertyGetter`, `CallPropertySetter`, `CallMethod(object, string, ...)`, `CallMethodIfImplemented`, `IsMethodImplemented`, `CallMethodTryAsync` and `IsAsyncMethod(object, string, ...)`
* `MethodCaller.GetType(string)` and `MethodCaller.GetType(string, bool)`, which resolve a type from its name
* `ServiceProviderMethodCaller.FindDataPortalMethod<T>(object, object[])`
* The `LateBoundObject` methods that call `MethodCaller` or `ServiceProviderMethodCaller`
* `Utilities.CallByName`

Their behavior is unchanged. The warning means the trimmer can't see which types or members these calls use. If you call them from a trimmed app, preserve the target types yourself, for example with `[DynamicallyAccessedMembers]` or `TrimmerRootAssembly`.

The trimmer preserves an object factory named in `[ObjectFactory("...")]` when the name is assembly qualified (`"MyApp.MyFactory, MyApp"`) or the factory is in the business object's assembly. `[ObjectFactory(typeof(MyFactory))]` always works. A custom `IObjectFactoryLoader` that maps other names to types must preserve those types itself.

### Other CSLA packages

Csla.Blazor, Csla.Blazor.WebAssembly, Csla.AspNetCore, Csla.Xaml (MAUI), Csla.Channels.Grpc, Csla.Channels.RabbitMq, Csla.Channels.Wcf and Csla.Testing are now analyzed for trimming and NativeAOT too ([#4962](https://github.com/MarimerLLC/csla/issues/4962)).

* **Data portal hosts** (`HttpPortal`, `GrpcPortal`, `RabbitMqPortal`, `WcfPortal`) resolve the business type names that clients send. The trimmer can't see those names, so a trimmed data portal server must preserve its business types, for example with `TrimmerRootAssembly` on the assemblies that contain them. Deserializing the objects and criteria already required this.
* **`Csla.Xaml.PropertyInfo`** walks a string binding path over its binding source, which can be any object, so it is marked `[RequiresUnreferencedCode]`. A trimmed MAUI or Avalonia app gets the warning where it uses the control, as it does for string-path bindings in MAUI itself.
* **The MAUI build of `Csla.Xaml`** is now strong-named with the same key as the other CSLA assemblies, so its assembly identity matches the WPF and Avalonia builds. .NET doesn't validate strong names at runtime, so MAUI apps are unaffected.
* **Blazor `SessionManager`** (WebAssembly) now uses source-generated JSON for the state controller messages. The JSON on the wire is unchanged.
* **Blazor `PropertyInfo`** (from `ViewModel<T>.GetPropertyInfo` or created directly) reads and writes the model's properties by name, and assumes the model is a CSLA business object, whose members the CSLA base classes preserve. If you use it with a model of another type, preserve that type's public properties yourself.
* **CSLA permission policies** built by `HasPermissionAttribute` or `CslaPolicy.GetPolicy` preserve the business type they name. If you write a `Csla:` policy name by hand, for example in `[Authorize(Policy = "...")]` or configuration, the type in that string must be preserved by your app.
* ASP.NET Core MVC and Razor Pages don't support trimming, so the tag helpers and `CslaModelBinder` aren't annotated.

Some `Type` parameters and generic type parameters now carry `[DynamicallyAccessedMembers]`. Source and binary compatibility are unaffected, but a generic wrapper of your own may need the same annotation to stay warning-free in a trimmed app:

* `PublicConstructors`: `PageModel<T>`, `CslaPolicy.GetPolicy`, `HasPermissionAttribute`, both `CslaPermissionRequirement` types, the `HtmlExtensions` permission helpers, `BlazorServerConfigurationOptions` and `BlazorWebAssemblyConfigurationOptions` session types, and `RabbitMqPortalOptions.SetPortalFactoryType<T>`
* `All`: the `Type` overloads of `ServiceProviderMethodCaller.FindDataPortalMethod<T>` and `TryFindDataPortalMethod<T>`, which reflect over the type's methods, and `CslaTestHost.GetDataPortal<T>` and `GetChildDataPortal<T>`

## Common `Csla.Mvvm.ViewModel<T>` base type

The view model types for each UI framework now share one implementation, `Csla.Mvvm.ViewModel<T>`, in the core `Csla` package ([#1766](https://github.com/MarimerLLC/csla/issues/1766)). The existing types keep their names:

* `Csla.Blazor.ViewModel<T>` inherits from `Csla.Mvvm.ViewModel<T>` and adds the Blazor-specific `GetPropertyInfo` methods.
* On MAUI and Avalonia, `Csla.Xaml.ViewModelBase<T>` inherits from `Csla.Mvvm.ViewModel<T>`.
* On WPF, `Csla.Xaml.ViewModelBase<T>` is still a `DependencyObject` with the `ModelProperty` and `ManageObjectLifetimeProperty` dependency properties. It delegates to a `Csla.Mvvm.ViewModel<T>` internally, so it has the same behavior.

You can use `Csla.Mvvm.ViewModel<T>` directly in other UI frameworks, such as Windows Forms. Pass an `ApplicationContext` to its constructor, or use the protected parameterless constructor and override `ResolveApplicationContext`.

Because every view model now has the same behavior, some of them behave differently than before.

### All view models

* `SaveAsync` returns `Task<T?>` (the saved `Model`) instead of `Task`. Code that awaits `SaveAsync()` is unaffected.
* When `ManageObjectLifetime` is `true`, `SaveAsync` saves a clone with its changes committed, and calls `ApplyEdit` on the `Model` only after the save succeeds. If the save fails, the `Model` keeps its undo snapshot, so `DoCancel` still restores the values from before the edit.
* The `Error` event is raised when `SaveAsync` or `RefreshAsync` fails. It's also raised when an authorization rule throws while the status properties (`CanSave`, `CanEditObject` and so on) are calculated. That exception is captured in `Exception` and `ViewModelErrorText`, and the status properties keep their previous values. A rule that keeps failing is reported when it starts failing, not on every model change. Before, the XAML view model posted it to the `SynchronizationContext`, which crashes a desktop app that has no global exception handler.
* If loading the object-level permissions (`CanCreateObject`, `CanGetObject`, `CanEditObject`, `CanDeleteObject`) fails, the next `RefreshAsync` evaluates the rules again.
* If the `Model` is still busy after `BusyTimeout`, `SaveAsync` captures a `TimeoutException`.
* `SaveAsync` does nothing if `Model` is `null`, and doesn't raise `Saved`. If the `Model` doesn't implement `ISavable`, the default `DoSaveAsync` throws an `InvalidOperationException`, which `SaveAsync` captures.
* `OnModelChanging(oldValue, newValue)` moves event handlers from the old model to the new model. `OnModelChanged(oldValue, newValue)` runs after the model changes and raises `ModelChanged`.

### XAML (WPF, MAUI and Avalonia)

* `SaveAsync` and `RefreshAsync` no longer throw. Exceptions are captured in the `Exception` and `ViewModelErrorText` properties, and both methods raise the new `Error` event. Code that wrapped these calls in `try`/`catch` should check `Exception` or handle `Error` instead. If `RefreshAsync` fails, `Model` is set to `null`.
* If the `Model` is invalid, `SaveAsync` doesn't save. It sets `ViewModelErrorText` to the model's error text, without setting `Exception` or raising `Error`.

  Code that navigates away after a save must now check for errors itself. Check both properties, because an invalid model sets only `ViewModelErrorText`:

  ```csharp
  // CSLA 10
  try
  {
    await vm.SaveAsync();
    await NavigateBack();
  }
  catch (Exception ex)
  {
    ShowError(ex);
  }

  // CSLA 11
  await vm.SaveAsync();
  if (vm.Exception is null && string.IsNullOrEmpty(vm.ViewModelErrorText))
    await NavigateBack();
  else
    ShowError(vm.ViewModelErrorText);
  ```
* `SaveAsync` waits up to `BusyTimeout` (30 seconds by default) for a busy `Model` to become idle. Use `SaveAsync(CancellationToken)` to control the wait yourself.
* There are new `Saved`, `ModelChanging` and `ModelChanged` events.
* `RefreshAsync<F>(factory)` is obsolete because the `F` type parameter was never used. Call `RefreshAsync(factory)`.
* On MAUI and Avalonia, `ManageObjectLifetime` now defaults to `true`, the same as WPF. Set it to `false` to keep the previous behavior.
* On MAUI and Avalonia, the public `ManageObjectLifetimeProperty` field is removed. Use the `ManageObjectLifetime` property.
* On MAUI, `BeginAddNew` is obsolete. Use `DoAddNew`.
* On WPF, override `OnModelChanging` instead of `OnModelChanged` if you need to run code before the event handlers are moved to the new model.

### Blazor

* `ViewModel<T>` implements `INotifyPropertyChanged`. It has the `IsDirty`, `IsValid`, `CanSave`, `CanCancel`, `CanCreate`, `CanDelete`, `CanFetch`, `CanAddNew` and `CanRemove` properties from the XAML view model, and the `DoCancel`, `DoAddNew`, `DoRemove` and `DoDelete` methods. These properties are updated whenever the model changes, which means instance-level authorization rules are evaluated on each change.
* The parameterless `OnModelChanged()` method is replaced by `OnModelChanged(T? oldValue, T? newValue)`. Update any override.
