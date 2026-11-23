# Nullable reference types and CSLA properties

With nullable reference types enabled (`<Nullable>enable</Nullable>`), a CSLA property registered with `RegisterProperty` whose type is a non-nullable reference type can produce `CS8603` ("Possible null reference return") in its getter:

```csharp
public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
public string Name
{
  get => GetProperty(NameProperty); // CS8603
  set => SetProperty(NameProperty, value);
}
```

CSLA 11 adds **not-null properties** ([#5047](https://github.com/MarimerLLC/csla/issues/5047)). A property registered with `RegisterNotNullProperty` has a `NotNullPropertyInfo<T>`, and its typed `GetProperty` and `ReadProperty` return `T` instead of `T?`, so the getter needs no `!`. In exchange, CSLA makes sure the property holds a value: it rejects null writes, and the data portal checks the property when it completes an object. This is the recommended way to declare a property that is never null.

This page explains how not-null properties work, when a property should be nullable instead, and how to declare each kind so the code builds cleanly, without `#pragma warning disable`.

The [NullablePropertiesExample](../Samples/NullablePropertiesExample) sample shows each case below in a project that treats nullable warnings as errors.

## Not-null properties (recommended)

Register the property with `RegisterNotNullProperty` and store the result in a `NotNullPropertyInfo<T>` field:

```csharp
public static readonly NotNullPropertyInfo<string> CustomerNameProperty =
  RegisterNotNullProperty<string>(nameof(CustomerName), null, string.Empty);
public string CustomerName
{
  get => GetProperty(CustomerNameProperty);
  set => SetProperty(CustomerNameProperty, value);
}

public static readonly NotNullPropertyInfo<LineItemList> LineItemsProperty =
  RegisterNotNullProperty<LineItemList>(nameof(LineItems));
public LineItemList LineItems
{
  get => GetProperty(LineItemsProperty);
  private set => LoadProperty(LineItemsProperty, value);
}
```

`RegisterNotNullProperty` has the same forms as `RegisterProperty` for a property name or lambda, a friendly name and a default value, plus forms that take a `NotNullPropertyInfo<T>` you create yourself. It is available on `BusinessBase` (and so `BusinessDocumentBase`), `ReadOnlyBase`, `CommandBase` and `ManagedObjectBase`.

* **A property with a default value** is satisfied by that default until something else is loaded. Give a `string` the default `string.Empty`, as above. Avoid a mutable object as a default value, because every instance shares it.
* **A property without a default value** must be loaded by the data portal operations, see [Loading not-null properties](#loading-not-null-properties). This is the usual form for a child object.
* **A value type** such as `NotNullPropertyInfo<int>` is never null, so it always satisfies the check.

A not-null property doesn't support lazy loading, a private backing field or `RelationshipTypes` other than `None`. Use `PropertyInfo<T>` for those (see [Choosing how to declare a property](#choosing-how-to-declare-a-property)).

### The source generator

In a project with nullable annotations enabled, `[CslaImplementProperties]` and `[CslaImplementPropertiesInterface<T>]` register each non-nullable property with `RegisterNotNullProperty`, and the generated getters have no `!`:

* a `string` gets `string.Empty` as its default value, `RegisterNotNullProperty<string>(nameof(Name), null, string.Empty)`, so it behaves as it did before;
* a value type, or any other reference type, is registered without a default value. **A child object property must then be loaded in every data portal operation that completes the object.**

These keep `RegisterProperty`, as before:

* a property declared nullable (`T?`, including `Nullable<T>`);
* a property, or one of its accessors, with `[AllowNull]` or `[MaybeNull]`;
* a property whose type is an unconstrained generic type parameter;
* every property in a project, or a part of the code, where nullable annotations are disabled.

To get the CSLA 10 output for every property, set the `CslaGenerateNotNullProperties` build property to `false` in the project file:

```xml
<PropertyGroup>
  <CslaGenerateNotNullProperties>false</CslaGenerateNotNullProperties>
</PropertyGroup>
```

## Loading not-null properties

A not-null property without a default value must hold a value by the time the data portal has finished with the object. Load it in every operation that completes an object: `[Create]`, `[Fetch]`, `[CreateChild]` and `[FetchChild]`, and `[Execute]` for a command:

```csharp
[Create]
private void Create([Inject] IChildDataPortal<LineItemList> itemsPortal)
{
  LoadProperty(LineItemsProperty, itemsPortal.CreateChild());
  BusinessRules.CheckRules();
}
```

The data portal checks the property:

* at the end of Create, Fetch, Execute and Update, and at the start of Update, so a null that arrives from the client isn't saved;
* for root objects, and for child objects that go through the child data portal (`CreateChild`, `FetchChild` and `UpdateChild`). A child that isn't dirty isn't saved, so it isn't checked;
* one object at a time. The check doesn't look into child objects; each child is checked when it goes through the child data portal.

If the property is still null, the data portal throws `InvalidOperationException`, naming the type and every property that failed. At the start of Update the message says that the operation wasn't started; at the end of an operation it says that the operation has already run. A caller of `IDataPortal<T>` gets it wrapped in a `DataPortalException`: catch that, and read its `BusinessException` or `InnerException`.

* **Transactions.** When the data portal provides the transaction (`[Transactional(TransactionalTypes.TransactionScope)]`, or `EnterpriseServices` on .NET Framework), the root object's check runs inside it, so a failure rolls the work back. In `Manual` mode (the default), the root object's exit check runs after your code returns, so a transaction your code committed stays committed. A child object is checked while your root code runs, when the child goes through the child data portal, so a child's failure is raised before your code commits.
* **Commands.** A command's `[Create]` isn't checked, and the inputs of a command sent with `ExecuteAsync(command)` aren't checked before `[Execute]` runs, so they can still be null inside `[Execute]`. A property that is still null when `[Execute]` returns fails the check. Pass required inputs as criteria, or declare them nullable.
* **Child objects.** Give a `BusinessBase` child with such properties its own `[CreateChild]` that loads them. The inherited one loads nothing.
* **Several fetch shapes.** If one type has several fetch operations and some of them don't load the property, load it anyway, declare it nullable, or split the type.

Analyzer [CSLA0033](analyzers/CSLA0033-NotNullPropertyNotLoadedAnalyzer.md) warns at build time about an operation that doesn't visibly load such a property, and about a child without its own `[CreateChild]`.

### Writing null

* A **`string`** property stores null as `string.Empty` on every write path: `SetProperty`, `LoadProperty`, the untyped and `*Convert` forms, business rule output and `AddOutValue`. Its setter never throws, and its getter never returns null once it holds a value.
* Any **other reference type** rejects null. `SetProperty`, `LoadProperty` and their untyped and `*Convert` forms throw `ArgumentNullException`, and the null isn't stored. In a business rule, `RuleContext.AddOutValue(value)` throws the same exception for a null value. `AddOutValue(property, null)` first substitutes the property's default value, so it throws only for a property without a default. A rule whose output value is still null gets an Error result, with nothing stored.

The `CslaModelBinder` in ASP.NET Core and ASP.NET MVC 5 doesn't store a posted null in a not-null property of a non-`string` type. It leaves the property alone.

### Cases that need a default value or a nullable property

* **A `string` without a default.** `RegisterNotNullProperty<string>(nameof(Name))` has a null default, not `string.Empty`. It reads null until it's loaded, it fails the data portal check if it isn't loaded, and `SetProperty(NameProperty, "")` on an unloaded property counts as a change. Register `string` properties with the `string.Empty` default, as the source generator and the migration code fix do. Analyzer [CSLA0035](analyzers/CSLA0035-NotNullPropertyEmptyFriendlyNameAnalyzer.md) warns when an empty second argument registers no default by mistake.
* **A read authorization rule.** When the user can't read the property, `GetProperty` returns its default value, which is null for a property without a default. Give it a default value, or declare the property nullable. See [CSLA0034](analyzers/CSLA0034-NotNullPropertyReadAuthorizationAnalyzer.md).
* **A property that isn't serialized** (`[DoNotSerialize]` or `[CalculatedProperty]`). Without a default it would be null after the object is serialized, for example after a data portal call. That's a build error, [CSLA0032](analyzers/CSLA0032-NotNullPropertyNotSerializedAnalyzer.md), and at runtime every instance of the type throws `InvalidOperationException` as soon as its properties are used.
* **Lazy loading.** `LazyGetProperty`, `LazyReadProperty`, their async forms and `LoadPropertyAsync` throw `NotSupportedException` for a not-null property, and [CSLA0036](analyzers/CSLA0036-NotNullPropertyDelayedLoadAnalyzer.md) is a build error. Use `PropertyInfo<T>` (see [Async lazy loading](#async-lazy-loading)).

## The guarantee and its scope

The guarantee is: **the typed getter of a not-null property doesn't return null on an object that the data portal has completed and checked.** It doesn't hold in these cases.

The object isn't complete yet:

* code that runs inside the operation that completes the object, before the property is loaded. For example, business rules that run before the `LoadProperty` call;
* a command between its `[Create]` and its `[Execute]`, and a command input inside `[Execute]` when the command was sent with `ExecuteAsync(command)`.

The object was checked, and the property is null anyway:

* a denied read of a property without a default value, because `GetProperty` returns the default instead of throwing (`NoAccessBehavior.SuppressException`);
* a new child object that was never saved and is removed by its own `IEditableObject.CancelEdit`. The parent's field becomes null, and the parent's next Update fails the check at its start.

The object is never checked:

* an object created with `new`, or with `ApplicationContext.CreateInstanceDI`, rather than through the data portal;
* an object that an `ObjectFactory` builds by hand, including the items of a list it builds, unless the factory calls `VerifyNotNullInvariant` on each one;
* every object when a custom `IDataPortalServer` replaces CSLA's server-side data portal.

To check an object yourself, call `NotNullInvariant.GetUnsatisfiedProperties(obj)`, which returns the properties that fail, or `NotNullInvariant.Verify(obj)`, which throws `InvalidOperationException` naming them. An `ObjectFactory` calls the protected `VerifyNotNullInvariant(obj)`. Each call checks one object and doesn't look into its children.

A few things never promised a non-null value in the first place, so they aren't exceptions: a value type, `NotNullPropertyInfo<int?>`, and the `*Convert` and untyped reads, which return `P?` or `object?`.

## Choosing how to declare a property

Decide for each property whether it can actually be null.

1. **It always has a value once the object is complete: make it a not-null property.** See [Not-null properties](#not-null-properties-recommended). This is the recommended choice.
2. **It can be null: register it with `RegisterProperty` and declare the property nullable.** See [The property can be null](#the-property-can-be-null-declare-it-nullable).
3. **Code that isn't migrated yet: keep `RegisterProperty` and use `!`.** See [Keeping `PropertyInfo<T>` with `!`](#keeping-propertyinfot-with-).

### Why `PropertyInfo<T>` getters return `P?`

For a `PropertyInfo<T>`, `GetProperty`, `ReadProperty`, `LazyGetProperty`, `LazyReadProperty`, `LazyGetPropertyAsync` and `LazyReadPropertyAsync` all return `P?`. The annotation is accurate, because each of them can return the property's default value instead of a stored one:

* **The user isn't allowed to read the property.** When a read authorization rule denies access, `GetProperty` returns the property's default value instead of throwing.
* **Nothing has been loaded.** A property that was never set returns its default value.
* **An async lazy load is still running.** `LazyGetPropertyAsync` and `LazyReadPropertyAsync` start the load and return the default value right away. The real value arrives later, along with a `PropertyChanged` event.
* **The value itself is null.** For example, a lazy-load value generator that returns `null`.

The default value of a `string` property is `string.Empty`, not `null`, unless you register a different default. For any other reference type, it is `null`.

The synchronous `LazyGetProperty` and `LazyReadProperty` don't return `null` just because it's the first access: they run the value generator right away and return its result.

### The property can be null: declare it nullable

Register the property with `RegisterProperty` and declare the property type as nullable when any of these apply:

* the value can legitimately be absent;
* the property has a read authorization rule, a non-`string` type, and no natural default value;
* the property is lazy loaded, for example with `LazyGetPropertyAsync` or `LazyReadPropertyAsync`;
* the type has several fetch shapes and only some of them load the property.

The `PropertyInfo<T>` type argument stays non-nullable. Only the property is declared nullable:

```csharp
public static readonly PropertyInfo<CreditSummary> CreditProperty = RegisterProperty<CreditSummary>(nameof(Credit));
public CreditSummary? Credit
{
  get => GetProperty(CreditProperty);
  private set => LoadProperty(CreditProperty, value);
}
```

Use `PropertyInfo<CreditSummary>`, not `PropertyInfo<CreditSummary?>`. The `PropertyInfo<T>` type argument has to match the type used with the property methods: with `PropertyInfo<ShipmentInfo?>`, a factory that returns `Task<ShipmentInfo>` (such as `FetchChildAsync`) doesn't match the `Func<Task<ShipmentInfo?>>` that `LazyGetPropertyAsync` expects, and produces a nullability warning.

The source generator and the migration analyzers leave this pattern alone: a property declared `T?`, or with `[AllowNull]` or `[MaybeNull]`, keeps `RegisterProperty`.

### Keeping `PropertyInfo<T>` with `!`

Code that doesn't move to not-null properties yet, such as code that also has to build with CSLA 10, can keep the property type non-nullable and apply the null-forgiving operator to the getter's result. This is what the `[CslaImplementProperties]` source generator emits when `CslaGenerateNotNullProperties` is `false`.

```csharp
public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
public string CustomerName
{
  get => GetProperty(CustomerNameProperty)!;
  set => SetProperty(CustomerNameProperty, value);
}
```

A synchronous lazy-loaded child whose fetch always returns an object works the same way. A not-null property can't be lazy loaded, so this is also the way to declare a lazy child that is never null:

```csharp
public static readonly PropertyInfo<OrderHistory> HistoryProperty =
  RegisterProperty<OrderHistory>(nameof(History), RelationshipTypes.LazyLoad);
public OrderHistory History
{
  get => LazyGetProperty(HistoryProperty,
    () => ApplicationContext.GetRequiredService<IChildDataPortal<OrderHistory>>().FetchChild(Id))!;
  private set => LoadProperty(HistoryProperty, value);
}
```

The `!` records your decision, and CSLA doesn't enforce it: if the property is never loaded, the getter returns null. Analyzer [CSLA0007](analyzers/CSLA0007-EvaluatePropertiesForSimplicityAnalyzer.md) accepts the `!` on a property helper call. CSLA 11.0.0-alpha.2 and earlier report CSLA0007 on these getters by mistake; you can ignore it there.

## Async lazy loading

Lazy-loaded properties use `PropertyInfo<T>`. Pass `LazyGetPropertyAsync` (or `LazyReadPropertyAsync`) a factory that starts the load. The factory only runs when the value isn't loaded and isn't already loading, so reading the property again doesn't start another fetch:

```csharp
public static readonly PropertyInfo<ShipmentInfo> ShipmentProperty =
  RegisterProperty<ShipmentInfo>(nameof(Shipment), RelationshipTypes.LazyLoad);
public ShipmentInfo? Shipment =>
  LazyGetPropertyAsync(ShipmentProperty,
    () => ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));
```

The property is nullable because the first read returns `null`. Bind to it, or handle `PropertyChanged`, so the UI updates when the value arrives. Use `await obj.WaitForIdle()` when code needs the loaded value.

The overloads that take a `Task<P>` instead of a factory are obsolete in CSLA 11. The task is already running by the time it's passed in, so a getter that creates it on every read starts another fetch on every read, even though only the first result is kept. In CSLA 10, which only has the `Task<P>` overloads, check whether the value is already loaded or loading before creating the task:

```csharp
get
{
  if (FieldManager.FieldExists(ShipmentProperty) || PropertyIsLoading(ShipmentProperty))
    return GetProperty(ShipmentProperty);
  return LazyGetPropertyAsync(ShipmentProperty,
    ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));
}
```

Because that getter does more than call a property method, CSLA0007 reports it, so it needs a suppression (for example `[SuppressMessage]` with a justification).

## Windows Forms

Bind a control that can produce null, such as a lookup that can be cleared, to a property declared nullable (`T?`, registered with `RegisterProperty`). The setter of a not-null property of a non-`string` type throws `ArgumentNullException` on null, and a setter that throws during data binding keeps the user in the field until the value is valid.

A not-null `string` property is safe to bind: a null from the control is stored as `string.Empty`, and the getter never returns null once the property holds a value.

## Migrating existing code

Two analyzers suggest moving `PropertyInfo<T>` registrations to `RegisterNotNullProperty`, where nullable annotations are enabled. Both are Info diagnostics with code fixes that change the field type, the registration and the getters' `!`:

* [CSLA0037](analyzers/CSLA0037-MigrateToNotNullPropertyAnalyzer.md) reports value types, `string` (the fix adds the `string.Empty` default) and properties with a non-null default value. Their migration has compile-time consequences only, so **Fix All** is offered.
* [CSLA0038](analyzers/CSLA0038-MigrateToNotNullPropertyAnalyzer.md) reports the other non-nullable reference types without a default value, typically child objects. After the fix, the property must be loaded on every Create and Fetch path, so review the type's data portal operations first and fix one property at a time. Fix All isn't offered.

Properties declared nullable, lazy-loaded properties and properties used with a delayed-load method aren't reported.

## Don't suppress `CS8603` everywhere

Avoid wrapping getters in `#pragma warning disable CS8603`, and don't make a property nullable only to silence the warning when it can't really be null. Both hide the information the annotations are there to give you. A not-null property records that the value is always there, and CSLA checks it. A `!` records the same decision without the check. A nullable type tells callers that they need to check.
