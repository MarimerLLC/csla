# Operation parameter type is not accessible

## Issue

This diagnostic is reported when a criteria parameter of a data portal operation method has a type that is not accessible outside its containing type, such as a `private` nested class. For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions]
public partial class Customer
  : BusinessBase<Customer>
{
  [Serializable]
  private class Criteria
  {
    public int Id { get; set; }
  }

  [Fetch]
  private void Fetch(Criteria criteria) { }
}
```

Data portal extension methods are generated in a separate static class, so their parameters cannot use a type that only `Customer` can see. The extension methods for this operation method are not generated. Extension methods for the other operation methods of the class are still generated.

## How to fix

Make the criteria type `internal` or `public`, or pass the criteria values as separate parameters.
