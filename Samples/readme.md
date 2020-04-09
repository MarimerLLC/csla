# CSLA .NET Samples

There are three categories of samples

1. ProjectTracker - reference app for CSLA .NET and the sample used in the *Using CSLA* books on https://store.lhotka.net
2. *Example - small, simple, focused examples showing the most basic use of CSLA in different UI platforms/technologies
3. Other samples demonstrating specific features of CSLA .NET

## ProjectTracker

[ProjectTracker](https://github.com/MarimerLLC/csla/tree/master/Samples/ProjectTracker) is the most comprehensive sample, including numerous UI projects all built against a common business layer. It has a data portal appserver project, and provides both mock and SQL data access layers.

This sample is updated concurrent with the *Using CSLA* books, and so does not always use the latest features of CSLA. It turns out that it is harder to write and debug prose than code.

## *Example Samples

The `BlazorExample`, `RazorPagesExample`, and similarly named samples are small, simple, focused examples showing the basic use of CSLA .NET in different UI platforms/technologies.

Each example uses the same business layer code, though the code is duplicated into each sample solution for simplicity. The business layer includes the use of error, warning, and informational business rules. And each UI displays those rules to the user by using the appropriate UI technology and any CSLA UI helper types for that platform.

These samples are nearly all built using a local data portal, and so effectively are 1-tier deployments. This avoids any complexity with setting up app servers, databases, etc. The focus is on how each UI technology rests on a common business layer.

## Other Samples

Other samples include

### Business Rules
* BusinessRuleDemo - demo showing the use of business rules
* RuleTutorial - in-depth project showing all uses of the CSLA rules engine

### Data Portal

* SimpleNTier - very simple n-tier architecture with UI, appserver, and data access
* Csla.DiffGram - sample showing how to optimize transfer of only the data changed when the user edits a large collection
* CustomActivator - shows the use of a data portal custom activator
* CustomErrorHandling - shows the use of data portal custom error handling
* DataPortalFactoryExample - shows the use of data portal factories
* DataPortalInstrumentation - shows the use of the data portal dashboard instrumentation feature
* RoutedDataPortal - shows the use of the data portal routing capability for versioning and workload routing
* DataPortalPerf - app designed to test data portal performance

### Web API

* CslaFastStart - simple web API example

### Windows Forms

* RootChildGrandchildWinFormTest - demonstrates n-level undo with deep layering in the object graph
* WinSortFilter - shows how to use datagrid sorting and filtering

### WPF

* PropertyStatus - demonstrates the use of the PropertyStatus control
