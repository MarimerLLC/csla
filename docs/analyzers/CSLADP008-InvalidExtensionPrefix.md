# Invalid DataPortalExtensions prefix

## Issue

This diagnostic is reported when the `Prefix` of a `[DataPortalExtensions]` attribute would not produce valid C# method names. For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions(Prefix = "My-")]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

No extension methods are generated for the class. When the prefix is set on an assembly attribute, `[assembly: DataPortalExtensions(Prefix = "...")]`, the diagnostic is reported once, on the assembly attribute, and no extension methods are generated for classes that use the assembly prefix.

## How to fix

Use a prefix that is a valid C# identifier, such as `Portal` or `My`.
