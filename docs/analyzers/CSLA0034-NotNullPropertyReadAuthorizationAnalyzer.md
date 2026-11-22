# Not-null properties with read authorization rules need a default value.

## Issue

This analyzer is tripped if a not-null property of a reference type without a default value has a read authorization rule. A rule counts when it is created or added with `AuthorizationActions.ReadProperty` and the property's field as arguments. For example:

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

The check is best-effort: a rule built in a helper method, or stored in a variable first, isn't detected. The warning is reported on the rule.

## Code Fix

There is no code fix for this analyzer.
