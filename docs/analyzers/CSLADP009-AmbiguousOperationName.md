# Data portal operation methods are ambiguous

## Issue

This diagnostic is reported when two data portal operation methods on a `partial` class map to the same data portal operation and have the same number of `[Inject]` parameters. For example:

```
using Csla;
using System;

[Serializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id, [Inject] ICustomerDal dal) { }

  [Fetch]
  private void Fetch(int id, [Inject] IAuditLog log) { }
}
```

A call such as `portal.FetchAsync(42)` matches both methods, and the data portal has no way to choose between them. At run time the data portal throws an `AmbiguousMatchException`.

The code generated for the class does not dispatch these methods directly, so the data portal falls back to reflection-based dispatch and throws the same exception. Calling the operation through a generated data portal extension method fails the same way.

## How to fix

Remove one of the methods, or change the criteria parameters so that each method maps to a different operation. If the methods need different services, combine them into one method that injects every service it needs.

When the methods have different numbers of injected parameters, the data portal uses the method with more injected parameters, and [CSLADP002](CSLADP002-DuplicateOperationName.md) is reported instead.
