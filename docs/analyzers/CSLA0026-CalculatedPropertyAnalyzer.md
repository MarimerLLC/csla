# Calculated properties should be read-only.

## Issue

This analyzer is tripped if a property marked with `[CalculatedProperty]` has a setter that is not `private`, including an `init` accessor. For example:

```
using Csla;
using System;

[Serializable]
public partial class Order
  : BusinessBase<Order>
{
  public static readonly PropertyInfo<decimal> TotalProperty = RegisterProperty<decimal>(nameof(Total));
  [CalculatedProperty]
  public decimal Total
  {
    get => GetProperty(TotalProperty);
    set => SetProperty(TotalProperty, value);
  }
}
```

The value of a calculated property is owned by the business rules that calculate it. Changing it never marks the object dirty, it is not serialized, and it is recalculated after the object is deserialized, so a value set from outside the object would be lost or overwritten. Remove the setter, or make it `private`:

```
  [CalculatedProperty]
  public decimal Total => GetProperty(TotalProperty);
```

## Code Fix

There is no code fix for this analyzer.
