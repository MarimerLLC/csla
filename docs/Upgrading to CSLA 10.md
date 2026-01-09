# Upgrading to CSLA 10

CSLA 10 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 9 to CSLA 10.

If you are upgrading from a version of CSLA prior to 8, you should review the [Upgrading to CSLA 9](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%209.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 9 to CSLA 10.

## Platform Support

CSLA 10 continues to support .NET Framework 4.6.2 and later, as well as modern .NET versions. CSLA 10 adds support for .NET 10.

## RevalidatingInterceptor

The constructor has changed and now expects an `IOptions<RevalidatingInterceptorOptions>` instance. With this new options object it is now possible to skip the revalidation of business rules during a `Delete` operation.
To configure the new options we are using the .Net [Options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options).

```csharp
services.Configure<RevalidatingInterceptorOptions>(opts =>
{
  opts.IgnoreDeleteOperation = true;
});
```

## New exception handler for asynchronous rules
A new API is added to make it possible to handle exceptions thrown by asynchronous rules.
The new interface to implement is `Csla.Rules.IUnhandledAsyncRuleExceptionHandler` which has two methods

* `bool CanHandle(Exception, IBusinessRuleBase)`
  * to decide whether this exception should be handled or not
* `ValueTask Handle(Exception, IBusinessRuleBase, IRuleContext)`
  * to handle the exception when `CanHandle(...) == true`

With these methods you can now decide whether to handle the exception and how or let the exception be unobserved bubble up and potentially cause a crash.

You can register your implementation in two ways

* Just add the implementation to your service collection `services.AddScoped<IUnhandledAsyncRuleExceptionHandler, YourImplementation>()`
  * Note: This has to be registered _after_ csla is added to the service collection.
* Use `services.AddCsla(o => o.UseUnhandledAsyncRuleExceptionHandler<YourImplementation>());`. The handler is registered as scoped.

The _default_ is still no handling of any exception thrown in an asynchronous rule.

## `IDataPortal` / `IChildDataPortal` breaking changes

Both interfaces `IDataPortal` and `IChildDataPortal` got the following changes to improve trimming support:
* Returning `ICslaObject`instead of `object`.
* `Update`/`Execute` now expect an `ICslaObject` parameter instead of `object`.

## `InjectAttribute` - AllowNull addition

The `InjectAttribute` got a new property called `AllowNull`. This property controls whether csla will use `GetService` or `GetRequiredService`.  
With `AllowNull == true` means it's using `GetService` which can return a `null` for a requested object.  
With `AllowNull == false` means it's using `GetRequiredService` which can not return `null` and will cause an exception.  
Furthermore you can ignore the new property if you are working with the nullable reference types. In that case the `AllowNull` is implicitly declared by your parameter declaration. For example:  
```csharp
#nullable enable // or set by your project

[Fetch]
private void Fetch([Inject] IService1 service1, [Inject] IService2? service2, [Inject(AllowNull = true)] IService3 service3)
{
  // ...
}
```
In the example above `service1` will be resolved with `GetRequiredService` while `service2` will be resolved with `GetService`. You can also set `AllowNull` which will override the nullable reference annotation. The other way around `[Inject(AllowNull = false)] IService4? service4` will _not_ change the parameter to be not-null. In this case the annotation takes precedence.

## `IDataErrorInfo` & `INotifyDataErrorInfo` extension points added
The `BusinessBase` implementation of [IDataErrorInfo](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.idataerrorinfo?view=net-10.0) and [INotifyDataErrorInfo](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifydataerrorinfo?view=net-10.0) now provide `virtual` methods to change the behavior.  
`IDataErrorInfo`
* `IDataErrorInfo.Error`-> `protected virtual string GetDataErrorInfoError()`
* `IDataErrorInfo.Item[string]`-> `protected virtual string GetDataErrorInfoIndexerError(string)`

`INotifyDataErrorInfo`
* `INotifyDataErrorInfo.GetErrors(string?)` -> `protected virtual IEnumerable GetNotifyDataErrorInfoGetErrors(string?)`
* `INotifyDataErrorInfo.HasErrors` -> `protected virtual bool GetNotifyDataErrorInfoHasErrors()`

## Nullable Reference Types

### Important breaking change/note
Due to this change it is **not** possible anymore to assign a csla property in a constructor. Be it a command or a business object.  
For example
```csharp
public class FooCommand : CommandBase<FooCommand> {
  public static PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
  public string Data
  {
    get { return GetProperty(DataProperty); }
    set { SetProperty(DataProperty, value); }
  }

  public FooCommand() {
    Data = ""; // This will now result in an ArgumentNullException because the needed ApplicationContext is not available yet.
  }
}
```

### Explanation

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


## `ViewModel<T>.SaveAsync` and `CslaDataProvider.Refresh` return `Task`

The `ViewModel<T>.SaveAsync(object sender, ExecuteEventArgs e)` method and `CslaDataProvider.Refresh<T>(Func<Task<T>> factory)` method now return `Task` instead of `async void`. This is a breaking change for any code that calls these methods directly.

If you were calling these methods without awaiting them, you will now need to handle the returned `Task` appropriately:

```csharp
// Before (async void - fire and forget)
viewModel.SaveAsync(sender, e);

// After (returns Task - must be handled)
await viewModel.SaveAsync(sender, e);
// or if you truly want fire-and-forget behavior:
_ = viewModel.SaveAsync(sender, e);
```

## OpenTelemetry Instrumentation

CSLA 10 adds OpenTelemetry instrumentation for the data portal. A new `OpenTelemetryDashboard` class provides metrics for data portal operations including total calls, completed calls, failed calls, and call duration.

To enable OpenTelemetry metrics, register the dashboard:

```csharp
services.AddCsla(o => o.DataPortal(dp =>
  dp.RegisterDashboard<OpenTelemetryDashboard>()));
```

The metrics integrate with standard OpenTelemetry consumers such as Prometheus, Grafana, Azure Monitor, and .NET Aspire dashboards.

## Virtual `Deserialized` Method

A new `protected virtual void Deserialized()` method has been added to `BusinessBase`, `ExtendedBindingList`, and `ObservableBindingList`. This method is called after deserialization completes and provides an extension point for custom post-deserialization logic.

```csharp
public class MyBusinessObject : BusinessBase<MyBusinessObject>
{
  protected override void Deserialized()
  {
    base.Deserialized();
    // Custom post-deserialization logic here
  }
}
```

## `IMobileObjectMetastate` Interface

A new `IMobileObjectMetastate` interface has been added to `Csla.Serialization.Mobile`. This interface is intended for developers creating custom serializers as an alternative to `MobileFormatter`.

The interface defines two methods:
* `byte[] GetMetastate()` - Gets lightweight serialization of field values (metastate) without child object references
* `void SetMetastate(byte[] metastate)` - Sets the metastate from a byte array

This interface is implemented on `MobileObject`, `MobileBindingList`, and `ReadOnlyListBase`. The metastate is intended for transitory data serialization, not long-term storage.

## Binary Serialization for Metastate

The internal serialization of metastate data now uses binary serialization instead of JSON. This provides improved performance for data portal operations. Since CSLA requires the same version on both ends of a network connection, this change is transparent to most applications.

## Principal Caching Removed

`ApplicationContextManager.GetUser()` no longer caches the current principal. It now always retrieves the principal from `Thread.CurrentPrincipal`. This improves behavior in multi-threaded scenarios and prevents stale principal references.

If your code relied on the previous caching behavior, you may need to review your authorization logic.

## `TransactionIsolationLevel.Snapshot`

A new `Snapshot` option has been added to the `TransactionIsolationLevel` enum. This allows use of snapshot isolation when using the `Transactional` attribute:

```csharp
[Transactional(TransactionIsolationLevel.Snapshot)]
private void DataPortal_Update()
{
  // Update logic with snapshot isolation
}
```

## `FriendlyName` Property on XAML `PropertyInfo`

The `Csla.Xaml.PropertyInfo` component now includes a `FriendlyName` property that can be used to display a user-friendly property name in WPF/XAML applications.

## `RuleContextModes` for Rule Execution Control

A new `RuleContextModes` enum has been added to provide control over when rules execute. The `RuleContext` now includes an `ExecuteContext` property with the following flags:
* `Any` - Rule executes in any context
* `CheckRules` - Rule executes during CheckRules calls
* `CheckObjectRules` - Rule executes during CheckObjectRules calls
* `PropertyChanged` - Rule executes when the property changes
* `AsAffectedProperty` - Rule executes as an affected property

## `ScanForDataAnnotations` Configuration Option

A new configuration option `CslaOptions.ScanForDataAnnotations(bool flag)` allows disabling the automatic scanning for DataAnnotations attributes on business objects.

```csharp
services.AddCsla(o => o.ScanForDataAnnotations(false));
```

> **Caution:** Setting this to `false` completely disables DataAnnotations attribute support within CSLA. Only use this option if you are certain your application does not use DataAnnotations attributes for validation and you need the performance benefit of skipping the attribute scan.

## Breaking changes

* `Csla.Server.DataPortal` constructor changed.
  * Removed unused parameters: `IDataPortalActivator activator`, `IDataPortalExceptionInspector exceptionInspector`.
* `Csla.Rules.BusinessRules` constructor changed.
  * New parameter `IUnhandledAsyncRuleExceptionHandler` added to support the new asynchronous rule exception handling.
  