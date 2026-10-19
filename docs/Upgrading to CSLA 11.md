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
* The `Error` event is raised when `SaveAsync` or `RefreshAsync` fails. It's also raised when an authorization rule throws while the status properties (`CanSave`, `CanEditObject` and so on) are calculated. That exception is captured in `Exception` and `ViewModelErrorText`, and the status properties keep their previous values. Before, the XAML view model posted it to the `SynchronizationContext`, which crashes a desktop app that has no global exception handler.
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
