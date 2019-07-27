# Managed backing fields must be public, static and read-only.

## Issue

This analyzer is tripped if a managed backing field is not `public`, `static` and `readonly`. For example:
```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  public static PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  public string Data { get { return this.GetProperty(DataProperty); } }
}
```

## Code Fix

A code fix is available for this analyzer. It will offer to add `public`, `static`, and/or `readonly`, depending on what the field is missing.