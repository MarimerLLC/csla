# Operations attributes should be used correctly

## Issue
This analyzer is tripped if an operation attribute is used on a type that isn't a sterotype or an `ObjectFactory`, or the operation method is `static`:

```
using Csla;
using System;

public class NotAStereotype
{
	[Fetch]
	private void Fetch() => /* ... */
}

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  [Fetch]
  private static void Fetch() => /* ... */
}
```

## Code Fix

No code fix is available for this analyzer.