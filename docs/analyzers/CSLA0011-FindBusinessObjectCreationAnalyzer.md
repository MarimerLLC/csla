# CSLA business objects should not be created outside of a ObjectFactory instance.

## Issue

This analyzer is tripped if a business object is created outside of an `ObjectFactory` instance. For example:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> { }

// ...
var customer = new Customer();
```

## Code Fix

No code fix is available for this analyzer.