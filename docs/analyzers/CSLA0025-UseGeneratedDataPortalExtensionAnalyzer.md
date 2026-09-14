# Use the generated data portal extension method.

## Issue

When a business class is marked with `[DataPortalExtensions]`, the CSLA source generator creates strongly typed extension methods on `IDataPortal<T>` and `IChildDataPortal<T>` for each of the class's data portal operation methods. This analyzer is tripped when code calls the untyped, criteria-based data portal methods for such a type instead of the generated extension methods. For example:

```
using Csla;
using System;

[DataPortalExtensions]
[Serializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }
}

public class CustomerService(IDataPortal<Customer> portal)
{
  public Task<Customer> GetCustomerAsync(int id) => portal.FetchAsync(id);
}
```

The untyped methods accept `params object[]` criteria, so mistakes in the number, order, or types of the arguments are only discovered at runtime. The generated extension methods have parameters that match the operation method, so the compiler verifies each call.

The following methods are reported when the business type `T` has the `[DataPortalExtensions]` attribute:

* On `IDataPortal<T>` and `DataPortal<T>`: `Create`, `CreateAsync`, `Fetch`, `FetchAsync`, `Delete`, `DeleteAsync`, and the criteria overloads of `Execute` and `ExecuteAsync`
* On `IChildDataPortal<T>` and `DataPortal<T>`: `CreateChild`, `CreateChildAsync`, `FetchChild`, and `FetchChildAsync`

A call is only reported when the generator produces an extension method that can replace it: the business type has an operation method of the matching kind (for example `[Fetch]` for `FetchAsync`) that accepts the number of criteria values passed, and an extension method is generated for that operation method. No diagnostic is reported when no such extension method exists, for example when the operation method has `[NoDataPortalExtension]`, has `ref` or `out` parameters, has a parameter type that is private to the business type, or has a name that the data portal interface's own methods would hide.

`Update`, `UpdateAsync`, `UpdateChild`, `UpdateChildAsync`, and `Execute`/`ExecuteAsync` called with a command object are not reported, because they do not take untyped criteria.

Generated code is not analyzed, because the generated extension methods themselves call the untyped data portal methods.

## Code Fix

No code fix exists for this analyzer. Replace the call with the corresponding generated extension method.
