# Synchronous data portal extensions are not generated without an async suffix

## Issue

This diagnostic is reported when `CslaDataPortalExtensionsAsyncSuffix` is `none` and `CslaGenerateSyncDataPortalExtensions` is not set to `false`:

```xml
<PropertyGroup>
  <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
</PropertyGroup>
```

Without an async suffix, the async method for a `Fetch` operation method is named `Fetch`. The synchronous method would have the same name and the same parameters, so it cannot be generated. Only the async extension methods are generated. The diagnostic is reported only when the project generates data portal extension methods.

## How to fix

If you only want async extension methods, which is the usual reason for removing the suffix, turn off the synchronous methods:

```xml
<PropertyGroup>
  <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
  <CslaGenerateSyncDataPortalExtensions>false</CslaGenerateSyncDataPortalExtensions>
</PropertyGroup>
```

If you need the synchronous methods, use an async suffix, such as the default `Async`.
