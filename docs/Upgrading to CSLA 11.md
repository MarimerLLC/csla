# Upgrading to CSLA 11

CSLA 11 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 10 to CSLA 11.

If you are upgrading from a version of CSLA prior to 10, you should review the [Upgrading to CSLA 10](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%2010.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 10 to CSLA 11.

## Platform Support

CSLA 11 continues to support .NET Framework 4.6.2 and later, as well as modern .NET versions. CSLA 11 adds support for .NET 11.

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
