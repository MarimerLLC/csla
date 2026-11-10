# Calculated properties are only supported in `BusinessBase` types.

## Issue

This analyzer is tripped if a property marked with `[CalculatedProperty]` is declared in a type that does not derive from `BusinessBase`, such as a `ReadOnlyBase` type or a plain class. For example:

```
using Csla;
using System;

[Serializable]
public partial class OrderInfo
  : ReadOnlyBase<OrderInfo>
{
  public static readonly PropertyInfo<decimal> TotalProperty = RegisterProperty<decimal>(nameof(Total));
  [CalculatedProperty]
  public decimal Total => GetProperty(TotalProperty);
}
```

Only `BusinessBase` types recalculate calculated properties after deserialization. In other CSLA types the attribute still stops the value from being serialized, so the value would be lost when the object crosses the data portal or is cloned. Remove the attribute, or move the property to a `BusinessBase` type.

## Code Fix

There is no code fix for this analyzer.
