# Do not ignore the result of Save().

## Issue

This analyzer is tripped if the return value from `Save()` is ignored. For example:
```
var customer = DataPortal.Fetch(123);
// ...
customer.Save();
```

## Code Fix

A code fix is available for this analyzer. It will offer to add an assigment to the variable that `Save()` was called on.