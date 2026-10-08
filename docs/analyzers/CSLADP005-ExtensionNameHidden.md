# Generated data portal extension would be hidden

## Issue

This diagnostic is reported when the name of a generated data portal extension method is the same as the name of an instance method on `IDataPortal<T>` or `IChildDataPortal<T>`. For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

The generated method names come from the operation method name, so this class would get `FetchAsync(int id)` and `Fetch(int id)` extension methods. `IDataPortal<T>` already has `FetchAsync(params object?[]? criteria)` and `Fetch(params object?[]? criteria)` instance methods. C# always prefers an applicable instance method over an extension method, so a call such as `portal.FetchAsync(42)` would call the untyped instance method, and the extension method could never be called that way.

The extension method is not generated.

## How to fix

Set `Prefix` on the attribute to give the generated methods distinct names:

```
[DataPortalExtensions(Prefix = "Portal")]
public partial class Customer
  : BusinessBase<Customer>
```

The class now gets `PortalFetchAsync(int id)` and `PortalFetch(int id)`. Alternatively, give the operation method a more specific name, such as `FetchById`, which produces `FetchByIdAsync(int id)`.
