# Operations should have the appropriate operation attribute

## Issue
This analyzer is tripped if a method uses a legacy data portal operation method name (e.g. `DataPortal_Fetch` or `Child_Update`), but doesn't have an operation attribute:

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

Starting with CSLA 11, the data portal only invokes methods that have an operation attribute (such as `[Fetch]` or `[UpdateChild]`). It no longer finds operation methods by name, so a method like the one above is never called. For this reason, this analyzer is a warning in CSLA 11 (it was informational in earlier versions).

## Code Fix
A code fix will show up to add the correct attribute to the operation.
