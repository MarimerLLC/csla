# Object authorization rules configuration should be declared as public.

## Issue
This analyzer is informing developer, that Object authorization rules method should be declared as `public`:

```
public class Customer
  : BusinessBase<Customer>
{
  [ObjectAuthorizationRules]
  private static void AddObjectAuthorizationRules()
  {
  }
}
```

Please change `private` to `public` to fix an issue.

## Code Fix

No code fix exists for this analyzer.