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

## Status

The package exists, but the helper types are still being added. Work in progress is
tracked by [#1225](https://github.com/MarimerLLC/csla/issues/1225) (helpers to
execute and test rules) and [#4882](https://github.com/MarimerLLC/csla/issues/4882)
(resetting the per-type rule cache between tests).

See the [unit testing](unit-testing.md) documentation for current guidance on testing
CSLA business code.

## Contributing

The project lives in `Source/Csla.Testing` and is part of both `Source/csla.build.sln`
(the packaging solution) and `Source/csla.test.sln` (the CI solution). Its unit tests
live in `Source/tests/Csla.Testing.Tests`. Please read the
[contribution guidelines](https://github.com/MarimerLLC/csla/blob/main/.github/CONTRIBUTING.md)
before submitting a pull request.
