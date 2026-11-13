# Nullable reference types and CSLA properties

With nullable reference types enabled (`<Nullable>enable</Nullable>`), a CSLA property whose type is a non-nullable reference type can produce `CS8603` ("Possible null reference return") in its getter:

```csharp
public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
public string Name
{
  get => GetProperty(NameProperty); // CS8603
  set => SetProperty(NameProperty, value);
}
```

This page explains where the warning comes from and how to declare properties so the code builds cleanly, without `#pragma warning disable`.

The [NullablePropertiesExample](../Samples/NullablePropertiesExample) sample shows each case below in a project that treats nullable warnings as errors.

## Why the getters return `P?`

`GetProperty`, `ReadProperty`, `LazyGetProperty`, `LazyReadProperty`, `LazyGetPropertyAsync` and `LazyReadPropertyAsync` all return `P?`. The annotation is accurate, because each of them can return the property's default value instead of a stored one:

* **The user isn't allowed to read the property.** When a read authorization rule denies access, `GetProperty` returns the property's default value instead of throwing.
* **Nothing has been loaded.** A property that was never set returns its default value.
* **An async lazy load is still running.** `LazyGetPropertyAsync` and `LazyReadPropertyAsync` start the load and return the default value right away. The real value arrives later, along with a `PropertyChanged` event.
* **The value itself is null.** For example, a lazy-load value generator that returns `null`.

The default value of a `string` property is `string.Empty`, not `null`, unless you register a different default. For any other reference type, it is `null`.

The synchronous `LazyGetProperty` and `LazyReadProperty` don't return `null` just because it's the first access: they run the value generator right away and return its result.

## Choosing between `!` and a nullable property

Decide for each property whether it can actually be null.

### The property can't be null in practice: use `!`

Keep the property type non-nullable and apply the null-forgiving operator to the getter's result. This is what the `[CslaImplementProperties]` source generator emits for non-nullable properties.

```csharp
public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
public string CustomerName
{
  get => GetProperty(CustomerNameProperty)!;
  set => SetProperty(CustomerNameProperty, value);
}
```

A synchronous lazy-loaded child whose fetch always returns an object works the same way:

```csharp
public static readonly PropertyInfo<LineItemList> LineItemsProperty =
  RegisterProperty<LineItemList>(nameof(LineItems), RelationshipTypes.LazyLoad);
public LineItemList LineItems
{
  get => LazyGetProperty(LineItemsProperty,
    () => ApplicationContext.GetRequiredService<IChildDataPortal<LineItemList>>().FetchChild(Id))!;
  private set => LoadProperty(LineItemsProperty, value);
}
```

Analyzer [CSLA0007](analyzers/CSLA0007-EvaluatePropertiesForSimplicityAnalyzer.md) accepts the `!` on a property helper call. CSLA 11.0.0-alpha.2 and earlier report CSLA0007 on these getters by mistake; you can ignore it there.

### The property can be null: declare it nullable

Declare the property type as nullable when any of these apply:

* the property has a read authorization rule, and a non-`string` type
* the property uses `LazyGetPropertyAsync` or `LazyReadPropertyAsync`
* the value can legitimately be absent

The `PropertyInfo<T>` type argument stays non-nullable. Only the property is declared nullable:

```csharp
public static readonly PropertyInfo<CreditSummary> CreditProperty = RegisterProperty<CreditSummary>(nameof(Credit));
public CreditSummary? Credit
{
  get => GetProperty(CreditProperty);
  private set => LoadProperty(CreditProperty, value);
}
```

Use `PropertyInfo<CreditSummary>`, not `PropertyInfo<CreditSummary?>`. The `PropertyInfo<T>` type argument has to match the type used with the property methods: with `PropertyInfo<ShipmentInfo?>`, passing a `Task<ShipmentInfo>` to `LazyGetPropertyAsync` produces warning `CS8620`.

## Async lazy loading

`LazyGetPropertyAsync` takes a `Task<P>`, and the task is already running by the time it's passed in. If the getter creates the task on every read, every read starts another fetch, even though only the first result is kept. Check whether the value is already loaded or loading first:

```csharp
public static readonly PropertyInfo<ShipmentInfo> ShipmentProperty =
  RegisterProperty<ShipmentInfo>(nameof(Shipment), RelationshipTypes.LazyLoad);
public ShipmentInfo? Shipment
{
  get
  {
    if (FieldManager.FieldExists(ShipmentProperty) || PropertyIsLoading(ShipmentProperty))
      return GetProperty(ShipmentProperty);
    return LazyGetPropertyAsync(ShipmentProperty,
      ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));
  }
}
```

The property is nullable because the first read returns `null`. Bind to it, or handle `PropertyChanged`, so the UI updates when the value arrives. Use `await obj.WaitForIdle()` when code needs the loaded value. Because this getter does more than call a property method, it needs a suppression for CSLA0007 (the sample uses `[SuppressMessage]` with a justification).

## Don't suppress `CS8603` everywhere

Avoid wrapping getters in `#pragma warning disable CS8603`, and don't make a property nullable only to silence the warning when it can't really be null. Both hide the information the annotations are there to give you: `!` records a decision about one property, and a nullable type tells callers they need to check.
