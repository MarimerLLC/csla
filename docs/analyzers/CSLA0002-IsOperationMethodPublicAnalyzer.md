# CSLA operations should not be public.

## Issue

This analyzer is tripped if a `DataPortal` operation is public on a business object. For example:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  public void DataPortal_Fetch(int id) { }	
}
```

## Code Fix

A code fix is available for this analyzer. It will offer to change the accessibility of the method to `internal`, `private`, or `protected` (the last one will not be offered if the class is `sealed`).