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

## Code Fix

No code fix is available for this analyzer.