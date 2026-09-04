# CSLA Avalonia Example

This sample is a small Avalonia desktop application used to exercise the
`Csla.Xaml.Avalonia` implementation against real CSLA business objects.

## Projects

- `DataAccess` — in-memory `IPersonDal` implementation.
- `BusinessLibrary` — `PersonEdit`, `PersonInfo`, and `PersonList` CSLA objects.
- `CslaAvaloniaExample` — Avalonia desktop UI using the local `Csla.Xaml.Avalonia` project.
- `CslaAvaloniaExample.Tests` — Avalonia headless tests for CSLA/Avalonia integration.

## Run the sample

From `Samples/AvaloniaExample`:

```bash
dotnet restore
dotnet run --project CslaAvaloniaExample/CslaAvaloniaExample.csproj
```

The **Person Edit** tab creates a new `PersonEdit` through the local CSLA data portal.
`Name` has a required rule and a maximum length rule. `Csla.Xaml.PropertyInfo` displays
the current validation state next to the editor.

The **Person List** tab exercises the read-only list and the in-memory DAL.

## Run the tests

```bash
dotnet test CslaAvaloniaExample.Tests/CslaAvaloniaExample.Tests.csproj
```

The first integration test verifies that `PropertyInfo` tracks the required rule on
`PersonEdit.Name` across invalid → valid → invalid transitions.

The test uses `Avalonia.Headless.XUnit`, so it can run on Linux without a display server.

## Purpose

Keep this sample intentionally small. It is meant to prove the CSLA/Avalonia integration
layer rather than demonstrate a full application architecture.
