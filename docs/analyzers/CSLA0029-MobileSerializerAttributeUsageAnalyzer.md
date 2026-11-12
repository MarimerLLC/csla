# Type has more than one custom serializer.

## Issue

This analyzer is tripped when a type is mapped to more than one `MobileFormatter` custom serializer. The mappings come from `[MobileSerializer]` on the type, `[MobileSerializerFor]` on a serializer, and `[assembly: MobileSerializerRegistration]` in the current project or a referenced assembly. For example:

```csharp
using Csla.Serialization.Mobile;

[MobileSerializer(typeof(MoneySerializer))]
public class Money { }

[MobileSerializerFor(typeof(Money))]
public class OtherMoneySerializer : IMobileSerializer { ... }
```

At run time `MobileFormatter` uses the first registered serializer that can serialize the type, so which serializer is used depends on the order in which assemblies are passed to `AddCustomSerializers`. CSLA's own serializers, such as the one for `ClaimsPrincipal`, are registered before yours.

Declare a single serializer for each type.

The diagnostic is reported on your declarations. Two conflicting registrations that are both in referenced assemblies aren't reported.

## Code Fix

There is no code fix for this analyzer.
