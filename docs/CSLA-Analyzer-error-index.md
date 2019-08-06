# Index of CSLA .NET Analyzer Errors

## CSLA0010  C# Operation argument types should be serializable.
The data portal requires that parameters passed to its methods be serializable. This is true for the client-side `DataPortal` methods such as `FetchAsync`, and it is also true for the server-side methods you implement in your business classes, including `DataPortal_Fetch` and its siblings.

This does not apply to the `Child_XYZ` server-side data portal methods, as they can accept any type.