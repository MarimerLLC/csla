# CSLA Avalonia Example Stub

This is a deliberately small Avalonia desktop/Linux shell intended as a test bed for a
`Csla.Xaml.Avalonia` port.

## Projects

- `DataAccess` — in-memory `IPersonDal`, patterned after CSLA's `Samples/MauiExample/DataAccess`.
- `BusinessLibrary` — small CSLA `PersonEdit`, `PersonInfo`, and `PersonList` object set.
- `CslaAvaloniaExample` — clean Avalonia desktop application with CSLA and DAL DI registration.

The UI project intentionally does **not** reference `Csla.Xaml.Avalonia`. Add your local
project reference after placing this sample where you want it.

Example:

```xml
<ProjectReference Include="..\..\Source\Csla.Xaml.Avalonia\Csla.Xaml.Avalonia.csproj" />
```

Adjust that relative path to match your checkout.

## Run

```bash
dotnet restore
dotnet run --project CslaAvaloniaExample/CslaAvaloniaExample.csproj
```

## Suggested first integration exercise

Keep `MainWindow` simple and add one view/view-model that resolves:

```csharp
IDataPortal<PersonEdit>
IDataPortal<PersonList>
```

from the application's DI container. That gives you a focused place to validate your
Avalonia `PropertyInfo`, converters, `ViewModel<T>`, and broken-rules behavior without
bringing the blog or management applications into the test.

## Source note

The project structure and DataAccess naming follow the public CSLA `Samples/MauiExample`
sample. The UI is a fresh Avalonia desktop shell, and the BusinessLibrary here is a compact
CSLA 10.1-compatible sample implementation rather than a byte-for-byte copy of the upstream
sample.
