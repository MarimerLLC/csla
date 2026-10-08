# Types with data portal operation methods should be partial.

## Issue

This analyzer is tripped if a class declares one or more data portal operation methods (methods marked with `[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[Execute]`, `[Delete]`, `[DeleteSelf]`, `[CreateChild]`, `[FetchChild]`, `[InsertChild]`, `[UpdateChild]`, `[DeleteSelfChild]`, or `[ExecuteChild]`), and either the class itself or any of its containing types is not declared `partial`. For example:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

The CSLA source generator adds a `partial` declaration to each class that has data portal operation methods. That declaration implements an internal operations interface so the data portal can invoke the operation methods directly, without reflection. The generator can only do this when the class, and every type that contains it, is `partial`:

```
using Csla;
using System;

[Serializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

This applies to abstract and generic classes as well. For nested classes, every containing type must also be `partial`:

```
public partial class Outer
{
  [Serializable]
  public partial class Customer
    : BusinessBase<Customer>
  {
    [Fetch]
    private void Fetch(int id) { }
  }
}
```

## Code Fix

A code fix is available for this analyzer. It will add the `partial` modifier to the class and to all of its containing types that are not already `partial`.
