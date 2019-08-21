# Operations should have the appropriate operation attribute

## Issue
This analyzer is tripped if an operation uses the correct method name (e.g. `DataPortal_Fetch`), but doesn't have the correct attribute:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  private void DataPortal_Fetch() => /* ... */
}
```

For now (CSLA version 5), this is only an informational analyzer. Future versions may change this to a warning and subsequently an error.

## Code Fix
A code fix will show up to add the correct attribute to the operation.
