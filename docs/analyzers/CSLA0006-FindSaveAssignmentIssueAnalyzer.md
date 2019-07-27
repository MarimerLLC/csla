# Do not ignore the result of SaveAsync().

## Issue

This analyzer is tripped if the return value from `SaveAsync()` is ignored. For example:
```
var customer = DataPortal.Fetch(123);
// ...
await customer.SaveAsync();
```

## Code Fix

A code fix is available for this analyzer. It will offer to add an assigment to the variable that `SaveAsync()` was called on.