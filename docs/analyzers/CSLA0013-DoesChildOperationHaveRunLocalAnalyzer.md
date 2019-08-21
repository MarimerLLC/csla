# Child operations should not have [RunLocal]

## Issue
This analyzer is tripped if a child operation has `[RunLocal]` on it, as it makes no sense to mark child operations this way - they always run under the context of a root operation's locality:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  [RunLocal]
  private void Child_Fetch() => /* ... */
}
```

## Code Fix
A code fix will show up to remove the attribute from the method.