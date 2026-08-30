# Csla.Testing

`Csla.Testing` is a NuGet package containing supporting code for people writing unit
tests against their CSLA .NET business code: business classes, business rules,
authorization rules, and related types.

> This package is intended for use in _test_ projects only. It is not required by,
> and is not intended for use in, production application code.

## Installation

Add the package to your test project:

```
dotnet add package Csla.Testing
```

The package targets `netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`, and depends
only on the core `Csla` package. It is deliberately unopinionated about your test
framework, so it works with MSTest, NUnit, xUnit, or any other runner.

## What is in the package

Nothing yet. The package currently ships an empty assembly; the helper types are being
added incrementally, and this section is updated as each one lands.

## Planned functionality

| Functionality | Tracking issue |
| --- | --- |
| Test host and `AddCslaTesting` API for standing up CSLA in a unit test | [#4883](https://github.com/MarimerLLC/csla/issues/4883) |
| Helpers to execute and test business rules | [#1225](https://github.com/MarimerLLC/csla/issues/1225) |
| Resetting the per-type business rule cache between tests | [#4882](https://github.com/MarimerLLC/csla/issues/4882) |

See the [unit testing](unit-testing.md) documentation for current guidance on testing
CSLA business code.

## Contributing

The project lives in `Source/Csla.Testing` and is part of both `Source/csla.build.sln`
(the packaging solution) and `Source/csla.test.sln` (the CI solution). Its unit tests
live in `Source/tests/Csla.Testing.Tests`. Please read the
[contribution guidelines](https://github.com/MarimerLLC/csla/blob/main/.github/CONTRIBUTING.md)
before submitting a pull request.

When you add functionality to `Csla.Testing`, update this page in the same pull request:
document the new API under "What is in the package", and remove the corresponding row
from "Planned functionality". This page is the reference for what the package actually
contains, so it should never describe types that have not shipped.
