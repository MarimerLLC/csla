# Data portal extensions are not generated for generic types

## Issue

This diagnostic is reported (as information) when a class marked with `[DataPortalExtensions]` is generic, or is nested in a generic type. For example:

```
using Csla;
using System;

[Serializable]
[DataPortalExtensions]
public partial class LookupList<T>
  : ReadOnlyListBase<LookupList<T>, T>
{
  [Fetch]
  private void Fetch(int id) { }
}
```

Extension methods on `IDataPortal<LookupList<T>>` would need their own type parameters and constraints, so the data portal extension methods generator does not generate them for generic types.

The data portal still calls the operation methods of a generic `partial` class directly. Only the extension methods are not generated.

## How to fix

Call the `IDataPortal<T>` methods directly, for example `portal.FetchAsync(42)`, or remove `[DataPortalExtensions]` from the class.

You can suppress this diagnostic in source or in an `.editorconfig` file:

```
dotnet_diagnostic.CSLADP003.severity = none
```
