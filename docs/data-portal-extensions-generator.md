# Data Portal Extensions Generator

Starting with CSLA 11, the `Csla` package includes a source generator that creates strongly typed extension methods on `IDataPortal<T>` and `IChildDataPortal<T>` for the data portal operation methods of your business classes. Instead of passing untyped criteria:

```csharp
var person = await portal.FetchAsync(42);
```

you call a method whose parameters match the operation method, so the compiler checks the criteria:

```csharp
var person = await portal.PortalFetchAsync(42);
```

The generator and the related [CSLA0025](analyzers/CSLA0025-UseGeneratedDataPortalExtensionAnalyzer.md) analyzer are adapted from [Csla.DataPortalExtensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions) by [Stefan Ossendorf](https://github.com/StefanOssendorf), used under the MIT License. For CSLA 11 and later, use the generator included in the `Csla` package instead of the `Csla.DataPortalExtensions` package.

## Using the generator

Add `[DataPortalExtensions]` to a business class:

```csharp
[DataPortalExtensions(Prefix = "Portal")]
public partial class PersonEdit : BusinessBase<PersonEdit>
{
  [Create]
  private void Create() { }

  [Fetch]
  private async Task Fetch(int id, [Inject] IPersonDal dal) { /* ... */ }
}
```

The generator creates a `PersonEditDataPortalExtensions` class, in the namespace of `PersonEdit`, with these methods:

```csharp
Task<PersonEdit> PortalCreateAsync(this IDataPortal<PersonEdit> portal);
PersonEdit PortalCreate(this IDataPortal<PersonEdit> portal);
Task<PersonEdit> PortalFetchAsync(this IDataPortal<PersonEdit> portal, int id);
PersonEdit PortalFetch(this IDataPortal<PersonEdit> portal, int id);
```

Extension methods are generated for `[Create]`, `[Fetch]`, `[Execute]` and `[Delete]` methods on `IDataPortal<T>`, and for `[CreateChild]` and `[FetchChild]` methods on `IChildDataPortal<T>`.

* The method names come from the operation method name, without an `Async` suffix, plus the `Prefix`.
* The methods take only the criteria parameters. `[Inject]` parameters are resolved on the server as usual. Parameter default values are kept.
* The methods pass the operation name to the data portal, so the data portal calls the operation method directly instead of searching for it using reflection. With a custom or mock `IDataPortal<T>` implementation, the methods call the existing `params object[]` methods instead.

### Choosing a prefix

`IDataPortal<T>` already has instance methods such as `FetchAsync` and `Fetch`, and C# always prefers an instance method over an extension method. An operation method named `Fetch` would therefore produce extension methods that are hidden by the instance methods. These are not generated, and analyzer [CSLADP005](analyzers/CSLADP005-ExtensionNameHidden.md) reports them.

Use `Prefix` to give the generated methods distinct names, or give the operation methods more specific names, such as `FetchById`.

### Enabling extensions for an assembly

Add the attribute to the assembly to generate extension methods for every business class in the assembly that can have them:

```csharp
[assembly: DataPortalExtensions(Prefix = "Portal")]
```

This applies to non-abstract, non-generic classes that implement `ICslaObject` and are accessible outside their containing type. A `Prefix` on a class attribute takes precedence over the assembly `Prefix`.

### Excluding classes and methods

Add `[NoDataPortalExtension]` to a class or an operation method to exclude it.

## Build properties

MSBuild properties in the project file control the generated methods:

```xml
<PropertyGroup>
  <!-- Generate extensions for sync methods (default true) -->
  <CslaGenerateSyncDataPortalExtensions>false</CslaGenerateSyncDataPortalExtensions>
  <!-- Suffix of the async method names (default Async); none for no suffix -->
  <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
</PropertyGroup>
```

With no async suffix, the sync methods would have the same names as the async methods, so they are not generated; see [CSLADP011](analyzers/CSLADP011-SyncExtensionsNotGenerated.md).

## Diagnostics

The analyzers in the `Csla` package report extension methods that are not generated, and why:

* [CSLADP003](analyzers/CSLADP003-GenericExtensionsNotGenerated.md) - Data portal extensions are not generated for generic types
* [CSLADP004](analyzers/CSLADP004-InvalidExtensionsTarget.md) - Invalid target for `[DataPortalExtensions]`
* [CSLADP005](analyzers/CSLADP005-ExtensionNameHidden.md) - Generated data portal extension would be hidden
* [CSLADP006](analyzers/CSLADP006-InaccessibleParameterType.md) - Operation parameter type is not accessible
* [CSLADP007](analyzers/CSLADP007-DuplicateExtensionMethod.md) - Generated data portal extension would be duplicated
* [CSLADP008](analyzers/CSLADP008-InvalidExtensionPrefix.md) - Invalid `[DataPortalExtensions]` prefix
* [CSLADP010](analyzers/CSLADP010-InvalidAsyncSuffix.md) - Invalid data portal extensions async suffix
* [CSLADP011](analyzers/CSLADP011-SyncExtensionsNotGenerated.md) - Synchronous data portal extensions are not generated without an async suffix

Analyzer [CSLA0025](analyzers/CSLA0025-UseGeneratedDataPortalExtensionAnalyzer.md) suggests using a generated extension method instead of calling an `IDataPortal<T>` method with untyped criteria, and provides a code fix that replaces the call.

## Related

* [Data portal operations generator](data-portal-operations-generator.md)
* [Analyzers](analyzers/index.md)
