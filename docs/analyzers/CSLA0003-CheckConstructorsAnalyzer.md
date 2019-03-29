# CSLA business objects must have a public constructor with no arguments.

## Issue

This analyzer is tripped if a business object does not have a public constructor with no arguments. For example:
```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  private Customer() { }
}
```

## Code Fix

A code fix is available for this analyzer. It will offer to change a non-`public` constructor if it exists, or create one for the developer.