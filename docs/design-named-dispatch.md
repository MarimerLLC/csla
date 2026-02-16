# Design: Name-Based Data Portal Operation Dispatch (#4359)

## Context

The source generator for `IDataPortalOperationMapping` is complete - it generates code that dispatches data portal operations via criteria type pattern matching (e.g., `if (criteria is { Length: 1 } && criteria[0] is int p0_int)`). This eliminates reflection on the server.

The **next step** is to add **name-based dispatch**: the client computes a deterministic operation name (e.g., `"Fetch__Int32"`) from the resolved method's parameter types and sends it through the wire protocol. The server uses this name for direct, unambiguous method dispatch instead of pattern-matching on criteria types. This paves the way for future client-side strongly-typed extension methods.

**Naming convention**: `{OperationType}` for no-criteria, `{OperationType}__{Type1}_{Type2}` for criteria params. Only criteria params (not `[Inject]` params) contribute to the name. Uses `Type.Name`/`MetadataName` (e.g., `Int32`, `String`). Generic types: replace backtick with underscore + append type args (e.g., `List_1_Int32`). Arrays: element type + `Array` (e.g., `Int32Array`).

## New Interface

`IDataPortalOperationNamedMapping` - a separate interface (not modifying existing `IDataPortalOperationMapping`) to maintain backwards compatibility with pre-compiled assemblies:

```csharp
public interface IDataPortalOperationNamedMapping
{
  Task InvokeNamedOperationAsync(string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider);
}
```

## Generator Changes

The generator produces code implementing both `IDataPortalOperationMapping` and `IDataPortalOperationNamedMapping`. The named dispatch method uses a `switch` statement for O(1) lookup:

```csharp
async Task Csla.Server.IDataPortalOperationNamedMapping.InvokeNamedOperationAsync(
  string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider)
{
  switch (operationName)
  {
    case "Fetch__Int32":
      if (criteria is { Length: 1 } && criteria[0] is int p0_int)
      {
        Fetch(p0_int);
        return;
      }
      break;
    // ...
  }
  throw new DataPortalOperationNotSupportedException(operationName, criteria);
}
```

## Wire Protocol

- `CriteriaRequest` gains an `OperationName` property (nullable string, backwards compatible)
- `DataPortalContext` gains an `OperationName` property (nullable string, serialized in Get/SetState)

## Dispatch Order

**named** -> **criteria-based** -> **reflection**

## Backwards Compatibility

| Scenario | Behavior |
|----------|----------|
| Old client -> new server | `OperationName` is null; server skips name dispatch, uses criteria-based |
| New client -> old server | Old `CriteriaRequest` ignores unknown field; server uses criteria-based |
| Local proxy | `DataPortalContext.OperationName` flows directly (no serialization) |
| Pre-compiled libraries | Old code implements only `IDataPortalOperationMapping`; `_namedMapping` is null; criteria-based dispatch used |
