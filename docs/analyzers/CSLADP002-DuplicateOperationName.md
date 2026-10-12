# Data portal operation methods share an operation name

## Issue

This diagnostic is reported (as information) when two data portal operation methods on a `partial` class map to the same data portal operation. That happens when the methods have the same operation attribute and the same criteria parameter types, and differ only in their `[Inject]` parameters. For example:

```
using Csla;
using System;

[Serializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }

  [Fetch]
  private void Fetch(int id, [Inject] ICustomerDal dal) { }
}
```

A call such as `portal.FetchAsync(42)` matches both methods. The data portal picks the method with more injected parameters, which is `Fetch(int, ICustomerDal)` here. This is the same choice that reflection-based dispatch makes, and the code generated for the class makes it too. The diagnostic message names the method that is used.

The data portal extension methods generator also generates extension methods only for the method that is used, because both methods would produce the same extension method.

## How to fix

If the selected method is the one you intend to be called, no change is needed. Otherwise, remove the method that is not used, or change the criteria parameters so that each method maps to a different operation.

If two methods have the same number of injected parameters, the data portal cannot choose between them, and [CSLADP009](CSLADP009-AmbiguousOperationName.md) is reported instead.

You can suppress this diagnostic in source or in an `.editorconfig` file:

```
dotnet_diagnostic.CSLADP002.severity = none
```
