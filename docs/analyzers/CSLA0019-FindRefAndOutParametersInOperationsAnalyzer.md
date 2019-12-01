# Operations should not have ref or out parameters.

## Issue
This analyzer is tripped if an operation has parameters that are either `ref` or `out`:

```
public class Customer
  : BusinessBase<Customer>
{
  [Fetch]
  private void Fetch(string a, ref string b, out string c)
  {
    c = string.Empty
  }
}
```

In this case, the `b` and `c` parameters would be in error.

## Code Fix

No code fix exists for this analyzer.