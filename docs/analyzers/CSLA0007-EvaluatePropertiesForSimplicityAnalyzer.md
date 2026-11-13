# Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else.

## Issue

This analyzer is tripped if a property using a managed backing field does anything more than calling a property helper method. For example:
```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  public string Data { get { return ""x"" + this.GetProperty(DataProperty); } }
}
```

The null-forgiving operator on the helper call doesn't count as extra logic, so this getter doesn't trip the analyzer:

```
public string Data => GetProperty(DataProperty)!;
```

See [Nullable reference types and CSLA properties](../Nullable-Reference-Types.md).

## Code Fix

No code fix is available for this analyzer.