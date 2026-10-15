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

## Obsolete XML serialization readers and writers removed

The `DataContractSerializer` based MobileFormatter readers and writers have been removed ([#3347](https://github.com/MarimerLLC/csla/issues/3347)):

* `CslaXmlReader` and `CslaXmlWriter`
* `CslaXmlBinaryReader` and `CslaXmlBinaryWriter`
* `CslaReaderWriterFactory.GetDataContractSerializer()`

They were marked `[Obsolete]` in CSLA 9 ([#4336](https://github.com/MarimerLLC/csla/pull/4336)), and `DataContractSerializer` isn't compatible with trimming or NativeAOT.

If your configuration selects one of these types through the `MobileReader<T>()` or `MobileWriter<T>()` options, remove those calls to use the default `CslaBinaryReader` and `CslaBinaryWriter`. The serialized format changes, so the client and the server must be updated together.
