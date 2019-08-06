CSLA .NET Serialization
-----------------------
Serialization is the process of taking the state of an object graph and converting into a byte stream that can be transferred over a network or used for n-level undo. That byte stream can later be _deserialized_ to create a copy or clone of the orignal object graph.

CSLA .NET relies on deep serialization to do this work. Most serializers do shallow serialization: operating only on public properties. Deep serialization serializes at the field level, including non-public field values. Deep serialization also preserves the shape of the object graph upon deserialization, where shallow serialization usually changes the shape of the object graph on deserialization, often creating new instances of objects that didn't exist in the original graph.

The .NET Framework provides two built-in deep serializers:

* `BinaryFormatter`
* `NetDataContractSerializer` (NDCS)

CSLA .NET provides its own highly optimized serializer:

* `MobileFormatter`

Where possible, CSLA .NET supports all three serializers, but not all .NET implementations include `BinaryFormatter` or NDCS. The only serilizer available on all .NET implementations is `MobileFormatter`.

**⚠ IMPORTANT:** The default serializer used by CSLA .NET is different for .NET Framework and .NET Core!

* .NET Framework defaults to `BinaryFormatter`
* .NET Core defaults to `MobileFormatter`
* Mono (Xamarin/WebAssembly) defaults to `MobileFormatter`

## MobileFormatter Constraints

The `MobileFormatter` imposes some constraints on your code beyond `BinaryFormatter` or NDCS. These constraints are why `MobileFormatter` works on all .NET implementations and why we've been able to optimize it for CSLA .NET.

1. All types must be `[Serializable]` (same as other serializers)
1. All types must implement `Csla.Serialization.Mobile.IMobileObject`
   1. More commonly types subclass CSLA base types like `Csla.Core.MobileObject` or `BusinessBase` that already implement the interface
   1. Primitive types are a special case, in that `MobileFormatter` knows how to serialize primitive .NET types, plus some other "primitive" types like `string`
1. Properties must either use _managed backing fields_ or manually get/set private backing fields in the serialization stream
   1. It is recommended to use managed backing fields, as this allows CSLA .NET to do all the work on your behalf
   1. If you strongly desire to use private backing fields, you need to override `OnGetState` and `OnSetState` and get/set every private backing field value in the serialization stream

This information, with more detail, is covered in the [Using CSLA 4](https://store.lhotka.net/using-csla-4-all-books) book series.
