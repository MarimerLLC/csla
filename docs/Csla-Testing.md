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

### Rule testers

The `Csla.Testing.Rules` namespace contains helpers that execute a single rule in
isolation, so you can assert on what the rule did without creating a business object,
calling `CheckRules`, or writing your own `ApplicationContext` bootstrap.

#### `BusinessRuleTester`

`BusinessRuleTester` runs a business rule and returns a `BusinessRuleTestResult`:

```csharp
using Csla.Testing.Rules;

var result = await BusinessRuleTester
  .For(new MyRule(MyBO.NameProperty))
  .WithInput(MyBO.NameProperty, "")
  .ExecuteAsync();

result.HasErrors.Should().BeTrue();
result.ErrorMessages.Should().ContainSingle().Which.Should().Be("Name required");
```

`ExecuteAsync` runs both `IBusinessRule` (synchronous) and `IBusinessRuleAsync`
(asynchronous) rules, so your test does not need to branch on which kind of rule it is
testing. Use `Execute()` when you want a test for a synchronous rule to stay
synchronous; it throws `InvalidOperationException` if the rule is asynchronous.

The builder methods are:

| Method | Purpose |
| --- | --- |
| `For(rule)` | Creates the tester for the rule under test |
| `WithInput(property, value)` | Supplies an input property value to the rule |
| `OnTarget(target)` | Supplies the target business object |
| `InMode(mode)` | Sets the `RuleContextModes` value; defaults to `PropertyChanged` |
| `AsUser(name, roles)` | Runs the rule as an authenticated user |
| `AsUnauthenticated()` | Runs the rule as an anonymous user |
| `AsPrincipal(principal)` | Runs the rule as a specific principal |
| `ConfigureCsla(options)` | Configures CSLA options for the rule's `ApplicationContext` |
| `ConfigureServices(services)` | Registers services the rule resolves from `ApplicationContext` |
| `UsingApplicationContext(context)` | Uses an existing `ApplicationContext` instead of creating one |

When you supply a target with `OnTarget`, the properties in the rule's
`InputProperties` are read from the target using the same rules as the rules engine,
including skipping lazy loaded properties that have no field data. Values supplied
through `WithInput` take precedence over values read from the target. As in the rules
engine, the target is given to an asynchronous rule only when the rule sets
`ProvideTargetWhenAsync`.

`BusinessRuleTestResult` exposes `Context`, `Results`, `OutputPropertyValues`,
`DirtyProperties`, `HasErrors` / `HasWarnings` / `HasInformation` / `IsSuccess`,
`ErrorMessages` / `WarningMessages` / `InformationMessages`, and
`GetOutValue<T>` / `TryGetOutValue<T>`. Remember that a rule which adds no result of
its own is given a single success result when its context is completed, exactly as it
would be inside the rules engine.

The result owns the services created to run the rule, so `Context` stays usable after
the rule has run. It is disposable; disposing it releases those services while leaving
the recorded results readable.

#### `AuthorizationRuleTester`

`AuthorizationRuleTester` does the same for authorization rules, returning an
`AuthorizationRuleTestResult`:

```csharp
using Csla.Testing.Rules;

var result = await AuthorizationRuleTester
  .For(new IsInRole(AuthorizationActions.EditObject, "Admin"))
  .ForType<MyBO>()
  .AsUser("rocky", "Admin")
  .ExecuteAsync();

result.HasPermission.Should().BeTrue();
```

`ExecuteAsync` runs both `IAuthorizationRule` and `IAuthorizationRuleAsync` rules and
accepts an optional `CancellationToken` that is passed to an asynchronous rule.
`Execute()` runs a synchronous rule and throws `InvalidOperationException` for an
asynchronous one.

In addition to the principal, configuration and application context methods listed
above, the builder offers `OnTarget(target)`, `ForType(type)` / `ForType<T>()` and
`WithCriteria(criteria)`. A target type is required: it is inferred from `OnTarget`
when `ForType` is not used, and `ExecuteAsync` throws `InvalidOperationException` if
neither was supplied. `AsUser` matters more here than for business rules, because
authorization rules read the principal from `ApplicationContext`.

## Planned functionality

| Functionality | Tracking issue |
| --- | --- |
| Test host and `AddCslaTesting` API for standing up CSLA in a unit test | [#4883](https://github.com/MarimerLLC/csla/issues/4883) |
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
