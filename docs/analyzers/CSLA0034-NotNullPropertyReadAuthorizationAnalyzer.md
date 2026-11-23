# Not-null properties with read authorization rules need a default value.

## Issue

This analyzer is tripped if a not-null property of a reference type without a default value has a read authorization rule. A rule counts when an authorization rule (a type that implements `IAuthorizationRule` or `IAuthorizationRuleAsync`) is created with `AuthorizationActions.ReadProperty` and the property's field as arguments, either by its constructor or by a method that returns the rule. Permission checks such as `BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.ReadProperty, SalaryBandProperty)` create no rule, so they aren't reported. For example:

```
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

public class Employee : BusinessBase<Employee>
{
  public static readonly NotNullPropertyInfo<string> SalaryBandProperty = RegisterNotNullProperty<string>(nameof(SalaryBand));
  public string SalaryBand => GetProperty(SalaryBandProperty);

  protected override void AddBusinessRules()
  {
    base.AddBusinessRules();
    BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, SalaryBandProperty, "HR"));
  }
}
```

`GetProperty` uses `NoAccessBehavior.SuppressException`, so a denied read returns the property's default value instead of throwing. A property without a default value has a default of null. A denied read then returns null through a getter declared non-nullable, and binding or UI code that trusts the declaration can fail.

Fix it in one of these ways:

* Give the registration a default value, which a denied read returns: `RegisterNotNullProperty<string>(nameof(SalaryBand), null, string.Empty)`.
* Declare the property as nullable (`string?`), so callers handle null.

The check is best-effort: a rule built inside a helper method from arguments it doesn't receive (for example a helper that takes only the property), or stored in a variable first, isn't detected. The warning is reported on the rule.

## Code Fix

There is no code fix for this analyzer.

See [Nullable reference types and CSLA properties](../Nullable-Reference-Types.md#cases-that-need-a-default-value-or-a-nullable-property) for the cases where a not-null property needs a default value.
