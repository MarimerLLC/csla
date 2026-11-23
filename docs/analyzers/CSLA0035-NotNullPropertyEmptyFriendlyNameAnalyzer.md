# An empty friendly name registers no default value.

## Issue

This analyzer is tripped by a two-argument `RegisterNotNullProperty<string>` call whose second argument is a constant empty or whitespace string. For example:

```
public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), "");
```

This reads as if `string.Empty` were the default value, but the second parameter is `friendlyName`. The call registers `Name` with an empty friendly name and **no** default value. An unloaded `Name` then reads as null, and the data portal check fails if the operation that completes the object doesn't load it.

To use `string.Empty` as the default value, pass it as the third argument:

```
public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
```

This is the registration the source generator emits for a not-null `string` property.

## Code Fix

There is no code fix for this analyzer.
