# Upgrading to CSLA 10

CSLA 10 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 9 to CSLA 10.

If you are upgrading from a version of CSLA prior to 8, you should review the [Upgrading to CSLA 9](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%209.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 9 to CSLA 10.

## Platform Support

TBD

## RevalidatingInterceptor

The constructor has changed and now expects an `IOptions<RevalidatingInterceptorOptions>` instance. With this new options object it is now possible to skip the revalidation of business rules during a `Delete` operation.
To configure the new options we are using the .Net [Options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options).
```csharp
services.Configure<RevalidatingInterceptorOptions>(opts =>
{
  opts.IgnoreDeleteOperation = true;
});
```


## Nullable Reference Types

CSLA 10 supports the use of nullable reference types in your code. This means that you can use the `#nullable enable` directive in your code and the compiler will now tell you where CSLA does not expect any `null` values or returns `null`.

Supporting nullable types means that some APIs have changed to support nullable types.

* Many methods now throw an `ArgumentNullException` instead of a `NullReferenceException`. That means typically the methods didn't work so far with `null` anyway
* The `User` and `Principal` properties of `ApplicationContext` no longer return null
* `Csla.Configuration.ConfigurationManager.AppSettings` and `.ConnectionStrings` are no longer settable
* `Csla.Core.LoadManager.AsyncLoadException.Property` property set removed. It can now only be set by the constructor
* `Csla.Core.AddedNewEventArgs<T>` default constructor removed
* `Csla.Reflection.LateBoundObject(Type objectType)` constructor removed (hasn't worked so far anyway)
* `Csla.Core.UndoException` constructors now throw `ArgumentNullException` on necessary parameters and all public fields changed to readonly properties
* `Csla.Data.ObjectAdapter.Fill(DataTable dt, object source)` throws an `ArgumentNullException` for source instead of `ArgumentException`.
* `Csla.Reflection.ServiceProviderMethodInfo` 
  * Now has a constructor requiring a `MethodInfo` parameter
  * Property `MethodInfo` property set removed and replaced by the constructor
* `Csla.Reflection.DynamicMemberHandle` does not accept any `null` parameters now. That includes having no get/set for a property/field.
* `Csla.Rules.BrokenRule` can not be instantiated by user code. It wasn't useable before because all property setters were internal.
* `Csla.Rules.BrokenRulesCollection` does not accept any null, empty or white space values for methods which accepts a string for a property name.
* `Csla.Rules.IRuleContext.Add*Result(...)` methods now throw an `ArgumentException` when the provided `string description` is `IsNullOrWhiteSpace`.
* `Csla.Security.IAuthorizeReadWrite` all methods `ArgumentNullException` are now documented.
* `Csla.Web.Mvc.CslaModelBinder` now needs an `ApplicationContext` in it's constructor.
* `AddAspNetCore()` configuration method now adds the necessary services to support resolving `CslaModelBinder` from the DI container
* `Csla.Serialization.Mobile.SerializationInfo`
  * Now has a constructor requiring `int referenceId` and `string typeName`
  * Property `ReferenceId` and `TypeName` property set removed and replaced by the constructor
* `Csla.Server.InterceptArgs`
  * Now as two new constructors requiring necessary parameters
  * Property set for required parameters removed
* `Csla.Server.EmptyCriteria`
  * Public constructor removed (now private). Instead use `Csla.Server.EmptyCriteria.Instance`.
* `Csla.Server.ObjectFactory`
  * Protected methods now guard against `null` objects
* `SessionMessage` now inherits from `MobileObject` instead of `CommandBase`.
* `DataPortalResponse` now uses `[AutoSerializable]` to auto implement `IMobileObject` instead of inheriting from `ReadOnlyBase`.
* `UpdateRequest` now uses `[AutoSerializable]` to auto implement `IMobileObject` instead of inheriting from `ReadOnlyBase`.