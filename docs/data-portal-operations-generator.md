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

## Inherited operation methods

The generated dispatch of a concrete class also calls the operation methods it inherits, so operation methods can live on a shared base class:

```csharp
public abstract partial class AppBase<T> : BusinessBase<T>
  where T : AppBase<T>
{
  [Create, RunLocal]
  protected virtual void Create(MyCriteria<T> criteria) { }

  [InsertChild]
  protected virtual void InsertChild() { }
}

public partial class Customer : AppBase<Customer>
{
  [Update]
  private void Update() { }
}
```

The generated code for `Customer` calls `Create`, `InsertChild` and `Update` directly, along with operation methods inherited from CSLA's base classes, such as `BusinessBase.Child_Create` and the `Child_UpdateAsync(params object[])` method of the editable list base classes. It chooses between methods the same way reflection-based dispatch does:

* Private methods of a base class are not included.
* A method declared closer to the class is preferred over one declared further up the inheritance chain.
* An override is called virtually, even when it does not repeat the operation attribute.
* If the class declares a member with the same name as an inherited operation method, the generated code calls the inherited method through `base`.

In a few cases the generated code leaves an inherited method to reflection-based dispatch, which still calls it as before:

* An intermediate base class declares another member with the same name, which would hide the method from the generated code.
* A method closer to the class takes the same number of criteria, and a less specific parameter type, such as `object`, means reflection could choose that closer method instead.
* The criteria also match a method further up the inheritance chain, and do not exactly match the types of a closer method with a less specific parameter type.
* The method is internal to another assembly.
* The method takes or returns a protected or private nested type, which the generated internal interface cannot name. The same applies to an operation method the class declares.

When a method the class declares and an inherited method map to the same operation, analyzers CSLADP002 and CSLADP009 report it at the declared method. If they tie, the data portal reports an ambiguous match, as reflection-based dispatch does.

Extension methods from the [data portal extensions generator](data-portal-extensions-generator.md) cover only the operation methods a class declares. An override of an inherited operation method is not a declared operation method, even though the data portal calls it.

## Operation methods that share a name

Two operation methods map to the same operation when they have the same attribute and the same criteria parameter types, and differ only in their `[Inject]` parameters. As with reflection-based dispatch, the data portal calls the method with more injected parameters, and analyzer [CSLADP002](analyzers/CSLADP002-DuplicateOperationName.md) reports which method is used. If the methods have the same number of injected parameters, the data portal cannot choose between them, and analyzer [CSLADP009](analyzers/CSLADP009-AmbiguousOperationName.md) reports the ambiguity.

## Related

* [Data portal extensions generator](data-portal-extensions-generator.md) - strongly typed extension methods for calling the data portal
* [Analyzers](analyzers/index.md)
