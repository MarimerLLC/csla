# Invalid data portal extensions async suffix

## Issue

This diagnostic is reported when the `CslaDataPortalExtensionsAsyncSuffix` MSBuild property is set to a value that cannot be used in a C# method name. For example:

```xml
<PropertyGroup>
  <CslaDataPortalExtensionsAsyncSuffix>-Async</CslaDataPortalExtensionsAsyncSuffix>
</PropertyGroup>
```

The value is ignored, and the generated async data portal extension methods use the default `Async` suffix. The diagnostic is reported only when the project generates data portal extension methods.

## How to fix

Set the property to a valid identifier, such as `Async` or `Task`, or to `none` for no suffix:

```xml
<PropertyGroup>
  <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
</PropertyGroup>
```

With no suffix, the synchronous extension methods are not generated; see [CSLADP011](CSLADP011-SyncExtensionsNotGenerated.md).
