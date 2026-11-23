# Register the property as a not-null property.

## Issue

This analyzer reports a `PropertyInfo<T>` registration that can move to `RegisterNotNullProperty`, so that its typed getter returns `T` instead of `T?` and needs no `!`. It reports only registrations whose migration has compile-time consequences only:

* a value type other than `Nullable<T>`;
* `string`. The fix gives it `string.Empty` as its default value, so an unloaded property reads as `string.Empty`, as it did before;
* a reference type with an explicit default value that isn't null.

For example:

```
public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
public string Name
{
  get => GetProperty(NameProperty)!;
  set => SetProperty(NameProperty, value);
}
```

It is reported only where nullable annotations are enabled (`<Nullable>enable</Nullable>` or `#nullable enable`). It is an Info diagnostic: a suggestion, not a problem.

## Code Fix

The code fix changes:

* the field type, from `PropertyInfo<T>` to `NotNullPropertyInfo<T>`;
* the call, from `RegisterProperty` to `RegisterNotNullProperty`. An explicit `RelationshipTypes.None` is dropped;
* the default value of a `string`. A missing or null default becomes `string.Empty`, which keeps an existing friendly name: `RegisterNotNullProperty<string>(nameof(Name), null, string.Empty)`;
* the getters in the same document. The `!` after `GetProperty(NameProperty)` and `ReadProperty(NameProperty)` is removed. A `GetPropertyConvert` or `ReadPropertyConvert` call keeps its `!`.

The example becomes:

```
public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
public string Name
{
  get => GetProperty(NameProperty);
  set => SetProperty(NameProperty, value);
}
```

**Fix All** is offered, in the containing type first, then the document, project or solution. It can't introduce a runtime failure, unless the app already writes null into a property that it now declares non-null. That null used to pass silently. After the migration, a not-null `string` stores it as `string.Empty`, and other types throw at the write or at the next data portal call. `SafeDataReader.GetString` already returns `string.Empty` for `DBNull`.

## Not reported

* Properties declared nullable, where the CLR property of the registered name is `T?` or has `[AllowNull]` or `[MaybeNull]`. This is the pattern [Nullable reference types](../Nullable-Reference-Types.md) recommends for a value that can be absent, and the source generator treats these properties the same way.
* `Nullable<T>`, `T?` type arguments, and unconstrained type parameters.
* Registrations with `RelationshipTypes.LazyLoad` or `RelationshipTypes.PrivateField`, and registrations that pass a `PropertyInfo<T>` object. Not-null properties have no such forms.
* Properties used with `LazyGetProperty`, `LazyGetPropertyAsync`, `LazyReadProperty`, `LazyReadPropertyAsync` or `LoadPropertyAsync`.
* A `string` whose default value might be null, other than the constant `null`.
* Fields declared together with others (`PropertyInfo<int> A = ..., B = ...;`), and generated code.

A non-nullable reference type without a default value is reported by [CSLA0038](CSLA0038-MigrateToNotNullPropertyAnalyzer.md) instead.
