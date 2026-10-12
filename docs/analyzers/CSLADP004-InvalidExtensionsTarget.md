# Invalid target for DataPortalExtensions

## Issue

This diagnostic is reported when `[DataPortalExtensions]` is applied to a class that cannot have data portal extension methods. The message gives the reason, which is one of:

* The class does not implement `Csla.Core.ICslaObject`, so it is not a CSLA business type.
* The class is abstract, so the data portal cannot create it.
* The class is not accessible outside its containing type (for example, a `private` nested class), so extension methods in the containing namespace cannot refer to it.

For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions]
public abstract partial class CustomerBase
  : BusinessBase<CustomerBase>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

No extension methods are generated for the class.

When the assembly is marked with `[assembly: DataPortalExtensions]`, classes that cannot have extension methods are skipped without a diagnostic.

## How to fix

Remove `[DataPortalExtensions]` from the class, or apply it to the concrete business class instead.
