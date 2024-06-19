# Object authorization rules configuration should be marked with attribute.

## Issue
This analyzer is informing developer, that `AddObjectAuthorizationRules` method should be marked with `[ObjectAuthorizationRules]` attribute:

```
public class Customer
  : BusinessBase<Customer>
{
  public static void AddObjectAuthorizationRules()
  {
  }
}
```

Please add `[ObjectAuthorizationRules]` attribute to the method.

## Code Fix

No code fix exists for this analyzer.