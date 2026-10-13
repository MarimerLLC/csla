# Upgrading to CSLA 11

CSLA 11 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 10 to CSLA 11.

If you are upgrading from a version of CSLA prior to 10, you should review the [Upgrading to CSLA 10](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%2010.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 10 to CSLA 11.

## Platform Support

CSLA 11 supports .NET 10 and .NET 11, and continues to support .NET Framework 4.6.2 and later. CSLA 11 no longer targets .NET 8 or .NET 9.

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

## Required properties

CSLA 11 adds a `RelationshipTypes.Required` flag ([#4351](https://github.com/MarimerLLC/csla/issues/4351)). It's opt-in, so existing code is not affected. Use it in place of guard code in each data portal operation method that checks a property has been set.

Register the property with the flag, using the existing `RegisterProperty` overloads:

```csharp
public static readonly PropertyInfo<Address> AddressProperty =
  RegisterProperty<Address>(nameof(Address), RelationshipTypes.Required);
```

When a create, fetch, insert, update or execute operation completes, the data portal checks each required property. If any of them is null, it throws `RequiredPropertyNotSetException`, wrapped in a `DataPortalException`. The exception's `ObjectType` and `PropertyNames` properties identify the object and properties, and the message includes them too, so they are still available when the exception comes from a remote data portal.

* Root objects, child objects (through the child data portal) and objects created by an `ObjectFactory` are all checked. Delete operations, including saving an object marked for deletion, are not checked.
* A property can be null while the operation runs. Only the value at the end of the operation is checked.
* `Required` can be combined with other flags. A `LazyLoad | Required` property is only checked after it has been loaded. A `PrivateField | Required` property is read through its property getter.
* `SetProperty` converts a null `string` to `string.Empty`, so a required `string` property can only be null if it is loaded with `LoadProperty(..., null)`.
* An object whose operation didn't read or write any managed property has no field manager yet, and is not checked.

### Data binding

Property setters never throw for a null value, so data binding isn't affected. Setting a required property to null is caught on the next save.

There is one case where the check can run inside data binding. When a Windows Forms grid adds a row to an editable list, `AddNew` creates the item through the child data portal. If the child's `[CreateChild]` method leaves a required property null, the exception is thrown inside the grid's `AddNew`. Child classes used this way must set their required properties in `[CreateChild]`:

```csharp
[CreateChild]
private void Create([Inject] IChildDataPortal<Address> addressPortal)
{
  LoadProperty(AddressProperty, addressPortal.CreateChild());
}
```
