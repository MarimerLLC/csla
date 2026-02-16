# CSLA .NET Serialization

Serialization is the process of taking the state of an object graph and converting into a byte stream that can be transferred over a network or used for n-level undo. That byte stream can later be _deserialized_ to create a copy or clone of the orignal object graph.

CSLA .NET relies on deep serialization to do this work. Most serializers do shallow serialization: operating only on public properties. Deep serialization serializes at the field level, including non-public field values. Deep serialization also preserves the shape of the object graph upon deserialization, where shallow serialization usually changes the shape of the object graph on deserialization, often creating new instances of objects that didn't exist in the original graph.

## CSLA 9+

The default serializer for CSLA (version 6 and later) is `MobileFormatter`. This is a custom serializer that is optimized for CSLA .NET, and is the only serializer that is guaranteed to work on all .NET implementations.

### Using MobileFormatter

MobileFormatter supports many basic .NET types, including "primitive" types like `int`, `string`, `DateTime`, etc. It also supports any type that implements `IMobileObject`, which includes all CSLA base types.

> ⚠️ Standard .NET collection types such as `List<T>` and `Dictionary<K,V>` are _not_ directly serializable by `MobileFormatter`. Use `MobileList<T>` or `MobileDictionary<K,V>` from `Csla.Core` for serializable collections, or register a custom serializer for your collection type.

If you create your complex types by subclassing CSLA base types and using the managed property syntax, you don't need to do anything special to support serialization.

You can also serialize your own types that don't subclass from CSLA. In this case your type needs to implement `IMobileObject` and manage the serialization of your type's state, or create and register a custom serializer for your type.

### Custom Serializers

Starting with CSLA 9 it is possible to create your own custom serializers for specific types. On application startup you register your custom serializer with CSLA and the MobileFormatter will use your custom serializer for the specific type(s) you've registered.

CSLA includes some pre-built serializers in the [/Source/Csla/Serialization](https://github.com/MarimerLLC/csla/tree/main/Source/Csla/Serialization/Mobile/CustomSerializers) folder. The `ClaimsPrincipalSerializer` is registered by default, and you can register the `PocoSerializer` to serialize POCO types if you choose.

### Replacing MobileFormatter

It is possible to create your own equivalent to `MobileFormatter`, but this is a complex and error-prone task.

There are already some serializers you may consider using.

#### AutoSerialization

The CSLA project includes an optional NuGet package that brings in a code generator to optimize the use of `MobileFormatter`. This package is called `Csla.AutoSerialization` and is available on NuGet.

The code and readme for [AutoSerialization](https://github.com/MarimerLLC/csla/tree/main/Source/Csla.Generators/cs/AutoSerialization) are in the CSLA project.

#### CslaGeneratorSerialization

[Jason Bock](https://github.com/JasonBock) has created a serializer for CSLA 9 that may provide some insights: [CslaGeneratorSerialization](https://github.com/JasonBock/CslaGeneratorSerialization).

Jason's serializer offers substantial performance and memory consumption benefits over `MobileFormatter`.

## History

Prior to .NET 9, .NET provided two built-in deep serializers:

* `BinaryFormatter`
* `NetDataContractSerializer` (NDCS)

CSLA .NET provides its own serializer:

* `MobileFormatter`

> ⚠️ Microsoft has deprecated the use of the `BinaryFormatter` and `NetDataContractSerializer`, so starting with CSLA 6 only `MobileFormatter` is supported.

> ℹ️ CSLA 9 supports custom serializers, allowing you to write your own serializer if you choose. This is not an easy task, as deep serialization is complex and error-prone. But it is possible if you have a specific need.

Where possible, CSLA .NET supports all three serializers, but not all .NET implementations include `BinaryFormatter` or NDCS. The only serilizer available on all .NET implementations is `MobileFormatter`.

In CSLA 6 the default serializer is `MobileFormatter`. In previous versions, the default serializer used by CSLA .NET is different for .NET Framework and .NET Core.

* .NET Framework defaults to `BinaryFormatter`
* .NET Core defaults to `MobileFormatter`
* Mono (Xamarin/WebAssembly) defaults to `MobileFormatter`

### MobileFormatter Constraints

The `MobileFormatter` imposes some constraints on your code beyond `BinaryFormatter` or NDCS. These constraints are why `MobileFormatter` works on all .NET implementations and why we've been able to optimize it for CSLA .NET.

1. All types must be `[Serializable]` (same as other serializers)
1. All types must implement `Csla.Serialization.Mobile.IMobileObject`
   1. More commonly types subclass CSLA base types like `Csla.Core.MobileObject` or `BusinessBase` that already implement the interface
   1. Primitive types are a special case, in that `MobileFormatter` knows how to serialize primitive .NET types, plus some other "primitive" types like `string`
1. Properties must either use _managed backing fields_ or manually get/set private backing fields in the serialization stream
   1. It is recommended to use managed backing fields, as this allows CSLA .NET to do all the work on your behalf
   1. If you strongly desire to use private backing fields, you need to override `OnGetState` and `OnSetState` and get/set every private backing field value in the serialization stream

This information, with more detail, is covered in the [Using CSLA 4](https://store.lhotka.net/using-csla-4-all-books) book series.
