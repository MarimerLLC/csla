# The return type from an operation should be either void or Task.

## Issue
This analyzer is tripped if an operation with a business object returns anything else other than `void` or `Task`:

```
using Csla;
using System;

[Serializable]
public class Customer
  : BusinessBase<Customer> 
{ 
  private string DataPortal_Fetch(int id) => string.Empty;
}
```

## Code Fix
A code fix will show up to change the return type to `void` if the method isn't asynchronous, and `Task` if it is. It will also add the `System.Threading.Tasks` `using` statement in the code file if needed. It won't try to change the method to remove any `return` statements - that's up to the developer to fix.