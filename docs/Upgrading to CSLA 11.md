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
