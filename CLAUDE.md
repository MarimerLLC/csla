# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

CSLA .NET is a business object framework for .NET. `main` is version **11.0.0** in development (set via Nerdbank.GitVersioning in `Source/version.json`); version 10 maintenance happens on the `v10.x` branch. The core package targets `netstandard2.0`, `net462`, `net472`, `net48`, `net10.0`, and `net11.0`. Requires **.NET SDK 11.0.100+** (currently a prerelease; see `Source/global.json`), plus the .NET 10 runtime to run the tests.

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

### NativeAOT / trimming smoke test
`Source/tests/Csla.AotSmokeTest` is a console app that publishes CSLA with NativeAOT and runs data portal, rules, serialization and authorization scenarios (non-zero exit on failure). CI publishes it after the test step, fails on any `IL`/AOT warning, and runs the exe. NativeAOT needs the platform linker (on Windows, the Visual Studio C++ workload); without it, use the trimmed self-contained publish:
```
dotnet publish Source/tests/Csla.AotSmokeTest -c Release -r win-x64 -o aot-smoke                          # NativeAOT
dotnet publish Source/tests/Csla.AotSmokeTest -c Release -r win-x64 -o aot-smoke -p:CslaSmokeTrimOnly=true  # trimmed only
aot-smoke/Csla.AotSmokeTest.exe
```
The trimmed publish doesn't catch NativeAOT-only failures. Without the native toolchain, a real NativeAOT run also works in a Linux container (Docker), from the repo root:
```
docker run --rm -v "$PWD:/src" -w /src mcr.microsoft.com/dotnet/sdk:11.0 bash -c "apt-get update -qq && apt-get install -y -qq clang zlib1g-dev > /dev/null && dotnet publish Source/tests/Csla.AotSmokeTest -c Release -r linux-x64 -o /tmp/aot && /tmp/aot/Csla.AotSmokeTest"
```
Nerdbank.GitVersioning needs a real `.git` directory, so from a git worktree clone the branch inside the container first.

`Source/Csla/Csla.csproj` builds with the trim/AOT analyzers and no suppressed IL warnings; fix or annotate new reflection rather than adding `NoWarn`.

Public APIs that reflect over an arbitrary object or type name get `[RequiresUnreferencedCode]`, so consumers are warned. Don't suppress there. When CSLA's own code calls such an API on a type it knows is preserved (a business object or object factory), call an internal `*Core` variant that carries the `UnconditionalSuppressMessage` instead (for example `MethodCaller.CallMethodIfImplementedCore`). Keep all messages and justifications in `Core/TrimmingMessages.cs`.


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

### Testing Support
- `Csla.Testing` — supporting code for unit testing CSLA business classes and rules (test-only package)

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
- `Csla.AotSmokeTest` — NativeAOT/trimming smoke test (console app, not a unit test project)

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

- CI runs on `windows-latest` with the .NET 11 SDK (pinned by `Source/global.json`) and the .NET 10 SDK
- Tests use `--settings Source/test.runsettings` which limits `MaxCpuCount` to 1 (tests must not run in parallel)
- Tests with `[TestCategory("SkipOnCIServer")]` are excluded from CI runs
- MAUI build only triggers when `Source/Csla.Xaml.Maui/`, `Source/Csla.Xaml.Shared/`, or `Source/csla.maui.test.sln` are modified
