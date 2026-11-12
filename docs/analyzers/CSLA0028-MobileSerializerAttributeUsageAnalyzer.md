# Custom serializer declaration is not valid.

## Issue

This analyzer is tripped by a `[MobileSerializer]`, `[MobileSerializerFor]` or `[assembly: MobileSerializerRegistration]` declaration that can't be registered as a `MobileFormatter` custom serializer. The CSLA source generator skips these declarations, so the type isn't serializable at run time.

The serializer type must be:

* a class, not a struct, interface or static class
* non-abstract
* non-generic, or a closed generic type such as `PocoSerializer<Customer>` (not `PocoSerializer<>`, and not a generic class marked with `[MobileSerializerFor]`)
* an implementation of `IMobileSerializer`
* a type with a public constructor
* accessible to an assembly-level attribute (public or internal, not a private or protected nested type)

The type that is serialized must be:

* a class or struct
* non-generic, and not nested in a generic type
* accessible to an assembly-level attribute (public or internal, not a private or protected nested type)

For example:

```csharp
using Csla.Serialization.Mobile;

public class NotASerializer { }

// The serializer doesn't implement IMobileSerializer
[MobileSerializer(typeof(NotASerializer))]
public class Money { }

// Custom serializers can be registered only for non-generic types
[MobileSerializerFor(typeof(List<int>))]
public class ListSerializer : IMobileSerializer { ... }

public class Order
{
  // A private nested type can't be named in an assembly attribute
  [MobileSerializer(typeof(LineSerializer))]
  private class Line { }
}
```

## Code Fix

There is no code fix for this analyzer.
