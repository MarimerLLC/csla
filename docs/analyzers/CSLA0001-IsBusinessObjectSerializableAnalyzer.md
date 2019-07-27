# CSLA business objects must be serializable.

## Issue

This analyzer is tripped if a business object is not marked with the `SerializableAttribute`. For example
```
using Csla;

public class Customer
  : BusinessBase<Customer> { }
```

## Code Fix

A code fix is available for this analyzer. It will add the attribute to the class, and also add `using System;` if it does not exist in the code file.