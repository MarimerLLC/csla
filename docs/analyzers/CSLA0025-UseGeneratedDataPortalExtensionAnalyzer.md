# Use the generated data portal extension method.

## Issue

When a business class, or its assembly, is marked with `[DataPortalExtensions]`, the CSLA source generator creates strongly typed extension methods on `IDataPortal<T>` and `IChildDataPortal<T>` for each of the class's data portal operation methods. This analyzer is tripped when code calls the untyped, criteria-based data portal methods for such a type instead of the generated extension methods. For example:

```
using Csla;
using System;

[DataPortalExtensions]
[Serializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void GetById(int id) { }
}

public class CustomerService(IDataPortal<Customer> portal)
{
  public Task<Customer> GetCustomerAsync(int id) => portal.FetchAsync(id);
}
```

The untyped methods accept `params object[]` criteria, so mistakes in the number, order, or types of the arguments are only discovered at runtime. The generated extension methods have parameters that match the operation method, so the compiler verifies each call.

The following methods are reported when the business type `T`, or its assembly, has the `[DataPortalExtensions]` attribute:

* On `IDataPortal<T>` and `DataPortal<T>`: `Create`, `CreateAsync`, `Fetch`, `FetchAsync`, `Delete`, `DeleteAsync`, and the criteria overloads of `Execute` and `ExecuteAsync`
* On `IChildDataPortal<T>` and `DataPortal<T>`: `CreateChild`, `CreateChildAsync`, `FetchChild`, and `FetchChildAsync`

A call is only reported when the generator produces an extension method that can replace it: the business type has an operation method of the matching kind (for example `[Fetch]` for `FetchAsync`) that accepts the number of criteria values passed, and an extension method is generated for that operation method. No diagnostic is reported when no such extension method exists, for example when the operation method has `[NoDataPortalExtension]`, has `ref` or `out` parameters, has a parameter type that is private to the business type, or has a name that the data portal interface's own methods would hide. No diagnostic is reported for a class marked `[NoDataPortalExtension]`. Calls to the synchronous methods are not reported when the `CslaGenerateSyncDataPortalExtensions` MSBuild property is `false`, or the `CslaDataPortalExtensionsAsyncSuffix` property is `none`, because only async extension methods are generated then.

When the business class is declared in a referenced assembly, such as a business library used by a Blazor app, its private operation methods are not visible. The analyzer then uses the extension methods generated in that assembly, which the generator marks with `[GeneratedDataPortalExtension]`. A call is reported when that assembly has an accessible extension method that replaces the method called and accepts the criteria values passed. A business assembly built with an earlier CSLA 11 prerelease has no marked extension methods, so calls for its classes are reported only after it is rebuilt.

`Update`, `UpdateAsync`, `UpdateChild`, `UpdateChildAsync`, and `Execute`/`ExecuteAsync` called with a command object are not reported, because they do not take untyped criteria.

Calls in Razor files (`.razor` and `.cshtml`), both in markup and in `@code` blocks, are reported at their line in the Razor file. Calls in other generated code are not reported, because the generated extension methods themselves call the untyped data portal methods.

## Code Fix

The code fix replaces the call with the generated extension method, keeping the arguments, and adds a `using` directive for the business class's namespace when needed. When several generated extension methods accept the arguments, a fix is offered for each of them:

```
public Task<Customer> GetCustomerAsync(int id) => portal.FetchAsync(id);
```

becomes

```
public Task<Customer> GetCustomerAsync(int id) => portal.GetByIdAsync(id);
```

The code fix is not offered for calls in Razor files, because the diagnostic is reported in the C# code generated from the Razor file, which cannot be edited. Change the call to the generated extension method by hand.
