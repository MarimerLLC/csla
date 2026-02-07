# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

CSLA .NET is a business object framework for .NET. Current version is **10.1.0** (set via Nerdbank.GitVersioning in `Source/version.json`). The core package targets `netstandard2.0`, `net462`, `net472`, `net48`, `net8.0`, `net9.0`, and `net10.0`. Requires **.NET SDK 10.0.100+** (see `Source/global.json`).

## Build and Test Commands

### Build everything (framework + tests)
```
dotnet build Source\csla.test.sln
```

### Run all tests
```
dotnet test Source\csla.test.sln --no-build --verbosity normal --filter TestCategory!=SkipOnCIServer --settings Source/test.runsettings
```

### Run a single test by name
```
dotnet test Source\csla.test.sln --no-build --filter "FullyQualifiedName~Csla.Test.ClassName.MethodName"
```

### Build only framework (no tests)
```
dotnet build Source\csla.build.sln
```

### Build MAUI projects (requires `dotnet workload install maui`)
```
dotnet build Source/csla.maui.build.sln
```

### Build analyzers/generators
```
dotnet build Source\Csla.Analyzers.sln
```

## Solution Files

| Solution | Purpose |
|---|---|
| `Source\csla.test.sln` | **Primary** — framework + all tests (used by CI) |
| `Source\csla.build.sln` | Framework libraries only (no tests) |
| `Source\csla.maui.test.sln` | MAUI-specific build + tests |
| `Source\Csla.Analyzers.sln` | Roslyn analyzers |
| `Source\csla.benchmarks.sln` | BenchmarkDotNet performance tests |

## Project Architecture

### Core Framework (`Source/Csla/`)
The main NuGet package. Key subsystems:
- **Business base classes** — `BusinessBase`, `ReadOnlyBase`, `CommandBase`, `BusinessListBase`, `ReadOnlyListBase`, `NameValueListBase`, `DynamicListBase` (root-level .cs files)
- **Data Portal** — Client-side in `DataPortalClient/` (proxies: `LocalProxy`, `HttpProxy`, `HttpCompressionProxy`), server-side in `Server/` (`SimpleDataPortal`, `DataPortalBroker`, `DataPortalSelector`, `ChildDataPortal`). `DataPortalT.cs` and `IDataPortalT.cs` define the generic `DataPortal<T>` / `IDataPortal<T>` interfaces.
- **Rules engine** — `Rules/` contains business rules (`BusinessRule`, `CommonRules`) and authorization rules (`AuthorizationRule`). `BrokenRulesCollection` tracks validation state.
- **Serialization** — `Serialization/` with `MobileFormatter` for CSLA's custom serialization. Source generators for auto-serialization in `Csla.Generators/cs/AutoSerialization/`.
- **Configuration** — `Configuration/` with DI registration via `CslaBuilder` and `AddCsla()` extension methods.
- **Core infrastructure** — `Core/` (property management, undo/n-level undo, `ManagedObjectBase`), `Security/`, `State/`, `Reflection/`

### UI Support Packages
- `Csla.AspNetCore` — ASP.NET Core integration (controllers, DI)
- `Csla.Blazor` / `Csla.Blazor.WebAssembly` — Blazor component support
- `Csla.Windows` — WinForms binding support
- `Csla.Xaml.Wpf` — WPF binding support
- `Csla.Xaml.Maui` — .NET MAUI support

### Channel Packages
- `Csla.Channels.Grpc` — gRPC data portal channel
- `Csla.Channels.RabbitMq` — RabbitMQ data portal channel

### Analyzers and Generators
- `Source/Csla.Analyzers/` — Roslyn analyzers shipped inside the Csla NuGet
- `Source/Csla.Generators/cs/AutoSerialization/` — Source generator for `[AutoSerialization]`
- `Source/Csla.Generators/cs/AutoImplementProperties/` — Source generator for auto-implementing CSLA properties
- `Source/Csla.Generator.DataPortalInterfaces.CSharp/` — Source generator for data portal interface generation

### Test Projects (under `Source/tests/`)
- `Csla.test` — Main test suite (MSTest + FluentAssertions)
- `csla.netcore.test` — .NET-specific tests (MSTest + AwesomeAssertions)
- `Csla.Blazor.Test` / `Csla.Blazor.WebAssembly.Tests` — Blazor tests
- `Csla.Windows.Tests` — WinForms tests
- `Csla.Analyzers.Tests` — Analyzer unit tests
- `Csla.Generator.*.Tests` — Generator unit tests
- `GraphMergerTest` — Object graph merge testing

## Coding Standards

- **Indent**: 2 spaces (set in `.editorconfig`)
- **Line endings**: CRLF
- **C# style**: Allman braces, `var` everywhere, expression-bodied members preferred
- **Field naming**: `_fieldName` for instance fields
- **Nullable**: Enabled in framework code (`WarningsAsErrors=nullable`), disabled in test projects
- **Language version**: `latest`
- **Implicit usings**: Enabled
- **Assemblies are strong-named** using `CslaKey.snk`

## Commit Message Format

Include the GitHub issue number: `#999 Description of change`

## CI Notes

- CI runs on `windows-latest` with .NET 10 SDK
- Tests use `--settings Source/test.runsettings` which limits `MaxCpuCount` to 1 (tests must not run in parallel)
- Tests with `[TestCategory("SkipOnCIServer")]` are excluded from CI runs
- MAUI build only triggers when `Source/Csla.Xaml.Maui/`, `Source/Csla.Xaml.Shared/`, or `Source/csla.maui.test.sln` are modified
