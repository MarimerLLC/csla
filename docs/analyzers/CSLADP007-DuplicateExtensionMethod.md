# Generated data portal extension would be duplicated

## Issue

This diagnostic is reported when two operation methods of a class would produce data portal extension methods with the same name and parameter types. For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions]
public partial class Customer
  : BusinessBase<Customer>
{
  [Create]
  private void Load(int id) { }

  [Fetch]
  private void Load(int id, [Inject] ICustomerDal dal) { }
}
```

Both methods would produce `LoadAsync(int id)` and `Load(int id)` extension methods on `IDataPortal<Customer>`, because injected parameters are not part of the extension method. The extension methods are generated for only one of the methods; the diagnostic names the method that does not get them.

## How to fix

Give the operation methods distinct names, such as `Create` and `Fetch`, so that each produces its own extension methods. Alternatively, mark the method that does not need an extension method with `[NoDataPortalExtension]`.
