using Csla;

// Generate strongly typed data portal extension methods, such as
// portal.PortalFetchAsync(id), for every business class in this assembly.
// The operation methods are named Create and Fetch, the same names as the
// IDataPortal<T> methods, so the extension methods need a prefix; without
// one they would be hidden by those methods (CSLADP005).
[assembly: DataPortalExtensions(Prefix = "Portal")]
