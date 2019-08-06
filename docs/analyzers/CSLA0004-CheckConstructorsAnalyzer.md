# CSLA business objects should not have public constructors with parameters.

## Issue

This analyzer is tripped if a business object has a public constructor with parameters. For example:
```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  public Customer(int id) { }
}
```

## Code Fix

No code fix is available for this analyzer.