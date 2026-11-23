# Not-serialized not-null properties need a default value.

## Issue

This analyzer is tripped if a property marked with `[DoNotSerialize]` or `[CalculatedProperty]` is registered as a not-null property of a reference type without a default value. The registration is found by name in the same type, including a registration emitted by the source generator, and the property is reported even when it isn't public. For example:

```
using Csla;

[CslaImplementProperties]
public partial class Order : BusinessBase<Order>
{
  [DoNotSerialize]
  public partial Customer Customer { get; private set; }
}
```

Here the generator registers `Customer` with `RegisterNotNullProperty<Customer>(nameof(Customer))`, which has no default value. A not-null property promises that its getter never returns null once the data portal has completed the object. A property that isn't serialized is null on the other side of every data portal call, so the data portal check fails with an exception the first time the object crosses it.

Fix it in one of these ways:

* Give the registration a default value: `RegisterNotNullProperty<Customer>(nameof(Customer), null, Customer.Empty)`. An unloaded property reads as its default value.
* Declare the property as nullable (`Customer?`), so it is registered with `RegisterProperty` and can be null.
* Remove `[DoNotSerialize]`, if the value should travel with the object.

The attribute on the `PropertyInfo` field itself doesn't trip the analyzer. Value types are never null, so they don't trip it either. A `string` property with no default value does trip it. The generator gives every not-null `string` property `string.Empty` as its default, so a generated `string` registration doesn't trip it.

## Code Fix

There is no code fix for this analyzer.
