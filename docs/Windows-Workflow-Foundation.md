# Windows Workflow Foundation

> ⚠️ **Deprecated Technology**: Windows Workflow Foundation (WF) is not supported in CSLA 4 or later. Microsoft has discontinued WF, and it is not available in modern .NET. This documentation is retained for historical reference only.

## Does CSLA .NET work with WF?

No, not in modern versions. CSLA 4 and later do not support WF.

You can create workflow activities using CSLA .NET objects. This is a very powerful way to create activities, because the business behaviors encapsulated within the activity are implemented as an object-oriented use case and can leverage the power of CSLA .NET (most notably the data portal).

You can also use the types in Csla.Workflow to invoke workflows. This is typically done within your DataPortal_XYZ or object factory methods on the application server in an n-tier deployment.

See the [Using CSLA .NET 3.0](http://store.lhotka.net) ebook for more details, as it has an entire chapter devoted to WF.
