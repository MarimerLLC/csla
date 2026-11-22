# Data portal operations should load not-null properties.

## Issue

A not-null property of a reference type without a default value must be loaded on every path that completes an object. The data portal checks this at the end of `[Create]`, `[Fetch]`, `[CreateChild]` and `[FetchChild]`, and at the end of `[Execute]` for a command, and throws if the property is still null.

This analyzer is tripped when such an operation doesn't visibly load one of these properties. It checks properties declared in the type or in a base type in source, including registrations emitted by the source generator. For example:

```
using Csla;

public class Order : BusinessBase<Order>
{
  public static readonly NotNullPropertyInfo<Customer> CustomerProperty = RegisterNotNullProperty<Customer>(nameof(Customer));
  public Customer Customer => GetProperty(CustomerProperty);

  [Fetch]
  private void Fetch(int id, [Inject] IOrderDal dal)
  {
    var data = dal.Get(id);
    // Customer is never loaded
  }
}
```

The warning is reported on the operation and names every property it doesn't load. Load the property in the operation, for example with `LoadProperty(CustomerProperty, ...)`. Or give its registration a default value.

The analyzer is also tripped by a `BusinessBase` child type that has these properties and child data portal operations but no `[CreateChild]` operation of its own. The inherited `[CreateChild]` loads nothing, so creating the child fails. The warning is reported on the type.

### What counts as a load

These count as loading the property anywhere in the operation, including in lambdas and local functions:

* a call to `LoadProperty`, `LoadPropertyMarkDirty`, `LoadPropertyConvert`, `SetProperty` or `SetPropertyConvert` with the property's field, typed or untyped;
* an assignment to the property of this object, including as part of a deconstruction such as `(Customer, Name) = ...`. Setting it in an object initializer of another instance (`new Order { Customer = ... }`) doesn't count.

The analyzer doesn't follow paths, so a load on only one branch still counts.

### When the analyzer stays silent

The check is best-effort, and it stays silent whenever it can't tell. The data portal check is the safety net. The analyzer stays silent for:

* types with `[ObjectFactory]`;
* operations whose body only throws, such as `=> throw new NotSupportedException()`, because they never complete an object;
* operations that pass `this` to a method or constructor;
* operations that call a method of their own type hierarchy declared in source, because a helper may load the property;
* operations that load through a property info that isn't a field;
* properties with a default value, and value types;
* properties registered in a base type from another assembly.

For a command, the data portal doesn't check `[Create]`. A property counts as loaded for `[Execute]` when the command's `[Create]` loads it, or when the property has a non-private setter, because the caller sets it before executing the command.

## Code Fix

There is no code fix for this analyzer.
