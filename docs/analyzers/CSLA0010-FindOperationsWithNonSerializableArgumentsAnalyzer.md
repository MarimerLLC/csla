# Operation argument types should be serializable.

## Issue

This analyzer is tripped if type of an argument to a `DataPortal` operation is not serializable. For example:

```
using Csla;
using System;

public class NotSerializable { }

[Serializable]
public class Customer
  : BusinessBase<Customer>
{ 
  [Fetch]
  private void Fetch(NotSerializable a) { }
}
```

## Types with a custom serializer

`MobileFormatter` can serialize a type that isn't `IMobileObject` when a custom serializer (an `IMobileSerializer`) is registered for it. The analyzer doesn't report an argument whose type has a custom serializer declared with one of these attributes:

```csharp
using Csla.Serialization.Mobile;
using Csla.Serialization.Mobile.CustomSerializers;

// On a type you own: "use this serializer for me"
[MobileSerializer(typeof(MoneySerializer))]
public class Money { ... }

[MobileSerializer(typeof(PocoSerializer<Customer>))]
public class Customer { ... }

// On a serializer, for types you don't own: "I handle these types"
[MobileSerializerFor(typeof(ThirdParty.Widget))]
public class WidgetSerializer : IMobileSerializer { ... }

// For a type and a serializer you don't own, such as PocoSerializer<T>
[assembly: MobileSerializerRegistration(typeof(ThirdParty.Gadget), typeof(PocoSerializer<ThirdParty.Gadget>))]
```

The CSLA source generator turns `[MobileSerializer]` and `[MobileSerializerFor]` into `[assembly: MobileSerializerRegistration]` attributes. The analyzer reads those registrations in the current project and in every referenced assembly, so a serializer declared in a shared library covers the operation arguments of the projects that reference it. `ClaimsPrincipal` is registered by CSLA itself.

The attributes only declare the mapping. Register the serializers at app startup by scanning the assemblies that declare them:

```csharp
builder.Services.AddCsla(o => o
  .Serialization(s => s
    .UseMobileFormatter(m => m.AddCustomSerializers(typeof(Order).Assembly))));
```

> ⚠️ If an assembly with declared serializers isn't passed to `AddCustomSerializers`, the analyzer doesn't report its types but serialization fails at run time.

A registration covers exactly the declared type. An argument whose type derives from a type with a custom serializer is still reported, because `MobileFormatter` matches the exact type at run time.

A serializer registered only in code (`CustomSerializers.Add(new TypeMap<T, S>(...))`) isn't visible to the analyzer. Use the attributes, or suppress the diagnostic. The same applies to a serializer declared in a project the business library doesn't reference, such as the app project.

Invalid declarations are reported by [CSLA0028](CSLA0028-MobileSerializerAttributeUsageAnalyzer.md), and types mapped to more than one serializer by [CSLA0029](CSLA0029-MobileSerializerAttributeUsageAnalyzer.md).

## Code Fix

No code fix is available for this analyzer.
