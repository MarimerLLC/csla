# Data Portal Operations Generator

Starting with CSLA 11, the `Csla` package includes a source generator that lets the data portal call your data portal operation methods directly, instead of finding and invoking them through reflection. There is nothing to install or configure: the generator runs in every C# project that references `Csla`.

## Using the generator

Declare your business class `partial`. Every class that declares data portal operation methods (methods marked with `[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[Execute]`, `[Delete]`, `[DeleteSelf]`, `[CreateChild]`, `[FetchChild]`, `[InsertChild]`, `[UpdateChild]`, `[DeleteSelfChild]` or `[ExecuteChild]`) is handled, as long as the class and every type that contains it are `partial`:

```csharp
[CslaImplementProperties]
public partial class PersonEdit : BusinessBase<PersonEdit>
{
  public partial int Id { get; private set; }
  public partial string Name { get; set; }

  [Create]
  private void Create() { }

  [Fetch]
  private async Task Fetch(int id, [Inject] IPersonDal dal)
  {
    var data = await dal.GetAsync(id);
    // ...
  }

  [Update]
  private async Task Update([Inject] IPersonDal dal) { /* ... */ }
}
```

Analyzer [CSLA0024](analyzers/CSLA0024-TypeWithOperationsShouldBePartialAnalyzer.md) reports classes with operation methods that are not `partial`, and provides a code fix that adds the `partial` modifier. Classes that are not `partial` continue to work through reflection.

Your operation methods do not change. They can still be `private`, take criteria and `[Inject]` parameters (including keyed services), and be synchronous or asynchronous.

## What is generated

For each `partial` class, the generator adds:

* An internal nested `IDataPortalOperations` interface with a member for each operation method, implemented explicitly by the class. This makes `private` operation methods visible to IDEs, analyzers and the trimmer as used code.
* For concrete classes, implementations of `Csla.Server.IDataPortalOperationMapping` and `Csla.Server.IDataPortalOperationNamedMapping`. The data portal calls these to invoke the operation methods.

The data portal dispatches an operation in this order:

1. **By name.** The client sends the name of the operation (for example `Fetch__Int32`), computed from the operation method's criteria parameter types. The generated code uses the name to call the method directly.
2. **By criteria.** If no name is available, for example because the call comes from an older client, the generated code matches the criteria values against the parameter types of each operation method.
3. **By reflection.** If the generated code cannot handle the call, the data portal falls back to the reflection-based dispatch used by earlier versions of CSLA.

The generated code resolves `[Inject]` parameters and reports errors the same way as reflection-based dispatch. An exception thrown by an operation method is wrapped in a `CallMethodException`, as before.

## Operation methods that share a name

Two operation methods map to the same operation when they have the same attribute and the same criteria parameter types, and differ only in their `[Inject]` parameters. As with reflection-based dispatch, the data portal calls the method with more injected parameters, and analyzer [CSLADP002](analyzers/CSLADP002-DuplicateOperationName.md) reports which method is used. If the methods have the same number of injected parameters, the data portal cannot choose between them, and analyzer [CSLADP009](analyzers/CSLADP009-AmbiguousOperationName.md) reports the ambiguity.

## Related

* [Data portal extensions generator](data-portal-extensions-generator.md) - strongly typed extension methods for calling the data portal
* [Analyzers](analyzers/index.md)
