# Register the property as a not-null property after review.

## Issue

This analyzer reports a `PropertyInfo<T>` registration of a non-nullable reference type, typically a child object, that has no default value (or a default that is, or might be, null). For example:

```
public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(nameof(LineItems));
public LineItems LineItems
{
  get => GetProperty(LineItemsProperty)!;
  private set => LoadProperty(LineItemsProperty, value);
}
```

It can move to `RegisterNotNullProperty`, so that the getter needs no `!`. But a not-null property without a default value must be loaded on every Create and Fetch path: the data portal checks it at the end of the operation and throws if it is still null. So check the type's data portal operations before migrating. [CSLA0033](CSLA0033-NotNullPropertyNotLoadedAnalyzer.md) points out operations that don't visibly load it.

It is reported only where nullable annotations are enabled. It is an Info diagnostic: a suggestion, not a problem.

## Code Fix

The code fix changes the field type to `NotNullPropertyInfo<T>` and the call to `RegisterNotNullProperty`. It drops an explicit `RelationshipTypes.None` and a `null` default value. It also removes the `!` after `GetProperty(field)` and `ReadProperty(field)` in the same document:

```
public static readonly NotNullPropertyInfo<LineItems> LineItemsProperty = RegisterNotNullProperty<LineItems>(nameof(LineItems));
public LineItems LineItems
{
  get => GetProperty(LineItemsProperty);
  private set => LoadProperty(LineItemsProperty, value);
}
```

There is **no Fix All**: each property is a decision about its load paths. A property that can legitimately be absent should be declared nullable (`LineItems?`) instead, which this analyzer doesn't report.

## Not reported

Besides the cases listed for [CSLA0037](CSLA0037-MigrateToNotNullPropertyAnalyzer.md), a property with `[DoNotSerialize]` or `[CalculatedProperty]` isn't reported. Without a default value, it would be null after every data portal call ([CSLA0032](CSLA0032-NotNullPropertyNotSerializedAnalyzer.md)).

See [Nullable reference types and CSLA properties](../Nullable-Reference-Types.md#migrating-existing-code) for how the two migration analyzers divide the work.
