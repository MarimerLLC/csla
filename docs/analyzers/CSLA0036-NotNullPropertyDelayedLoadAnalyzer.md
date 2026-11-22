# Not-null properties don't support delayed loading.

## Issue

This analyzer is tripped when a not-null property is passed to one of the delayed-load methods: `LazyGetProperty`, `LazyGetPropertyAsync`, `LazyReadProperty`, `LazyReadPropertyAsync` or `LoadPropertyAsync`. For example:

```
using Csla;

public class Order : BusinessBase<Order>
{
  public static readonly NotNullPropertyInfo<LineItems> LineItemsProperty = RegisterNotNullProperty<LineItems>(nameof(LineItems));
  public LineItems LineItems => LazyGetProperty(LineItemsProperty, () => LineItems.Create())!;
}
```

A delayed-load property is null until its first read, or until an asynchronous load finishes. A not-null property promises that it is never null once the data portal has completed the object, so these methods throw `NotSupportedException` for it. Value types are included, and so are properties registered by the source generator.

Fix it in one of these ways:

* Register the property with `RegisterProperty` (or declare it as nullable, so the generator does), and keep the delayed load.
* Keep the not-null registration, and load the property in the data portal operation that creates or fetches the object.

## Code Fix

There is no code fix for this analyzer.
