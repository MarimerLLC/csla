# Operation argument types should be serializable.

## Issue

This analyzer is tripped if type of an argument to a `DataPortal` operation is not serializable. For example:

```
using Csla;
using System;

public class NotSerializable { }

[Serializable]
public class Customer
  : BusinessBase<Customer>
{ 
  private void DataPortal_Fetch(NotSerializable a);
}
```

## Code Fix

No code fix is available for this analyzer.