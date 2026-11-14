# Interface properties cannot be generated.

## Issue

This analyzer is tripped when the type argument of `[CslaImplementPropertiesInterface<T>]` is not an interface declared in the same project. The source generator reads the properties to implement from the interface's declaration, so it generates nothing for the type, and code that uses those properties fails to compile. For example:

```csharp
using Csla;

// 'Customer' is a class, not an interface
[CslaImplementPropertiesInterface<Customer>]
public partial class CustomerEdit : BusinessBase<CustomerEdit> { }

// 'IServiceProvider' is declared in another assembly
[CslaImplementPropertiesInterface<System.IServiceProvider>]
public partial class OrderEdit : BusinessBase<OrderEdit> { }
```

Declare the interface in the same project as the business class. A private interface nested in the business class works well:

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
