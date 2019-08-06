# CSLA operations should not be public.

## Issue

This analyzer is tripped if a `DataPortal` operation is public on a business object interface. For example:

```
using Csla;

public inteface ICustomer
  : IBusinessObject
{ 
  void DataPortal_Fetch();
}
```

## Code Fix

No code fix is available for this analyzer.