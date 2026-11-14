# Interface properties cannot be generated.

## Issue

This analyzer is tripped when the source generator can't implement the properties for a `[CslaImplementPropertiesInterface<T>]` attribute, so it generates nothing for the type, and code that uses those properties fails to compile. That happens when:

* the attribute is not on a `partial class` or `partial struct` (records and non-partial types are not supported), or
* `T` is not an interface declared in the same project. The generator reads the properties to implement from the interface's declaration.

For example:

```csharp
using Csla;

// Not partial
[CslaImplementPropertiesInterface<ICustomerEdit>]
public class CustomerEdit : BusinessBase<CustomerEdit> { }

// 'Customer' is a class, not an interface
[CslaImplementPropertiesInterface<Customer>]
public partial class CustomerInfo : ReadOnlyBase<CustomerInfo> { }

// 'IServiceProvider' is declared in another assembly
[CslaImplementPropertiesInterface<System.IServiceProvider>]
public partial class OrderEdit : BusinessBase<OrderEdit> { }
```

Make the type partial, and declare the interface in the same project as the business class. A private interface nested in the business class works well:

```csharp
using Csla;

[CslaImplementPropertiesInterface<ICustomerEdit>]
public partial class CustomerEdit : BusinessBase<CustomerEdit>
{
  private interface ICustomerEdit
  {
    string Name { get; set; }
  }
}
```

## Code Fix

There is no code fix for this analyzer.
