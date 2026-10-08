# Upgrading to CSLA 11

CSLA 11 is a major release and so there are a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 10 to CSLA 11.

If you are upgrading from a version of CSLA prior to 10, you should review the [Upgrading to CSLA 10](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%2010.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 10 to CSLA 11.

## Platform Support

CSLA 11 supports .NET 10 and .NET 11, and continues to support .NET Framework 4.6.2 and later. CSLA 11 no longer targets .NET 8 or .NET 9.

## Legacy Data Portal Operation Method Names Removed

In earlier versions, if the data portal couldn't find a method with an operation attribute (such as `[Fetch]`), it fell back to looking for a method by name (such as `DataPortal_Fetch` or `Child_Update`). This fallback was controlled by the `DataPortalOptions.UseLegacyOperationMethods` option.

In CSLA 11 this fallback is removed. The data portal only invokes methods that have an operation attribute. The `UseLegacyOperationMethods` option is also removed.

If your business classes have operation methods with no attribute, add the matching attribute:

| Legacy method name | Attribute |
|---|---|
| `DataPortal_Create` | `[Create]` |
| `DataPortal_Fetch` | `[Fetch]` |
| `DataPortal_Insert` | `[Insert]` |
| `DataPortal_Update` | `[Update]` |
| `DataPortal_Delete` | `[Delete]` |
| `DataPortal_DeleteSelf` | `[DeleteSelf]` |
| `DataPortal_Execute` | `[Execute]` |
| `Child_Create` | `[CreateChild]` |
| `Child_Fetch` | `[FetchChild]` |
| `Child_Insert` | `[InsertChild]` |
| `Child_Update` | `[UpdateChild]` |
| `Child_DeleteSelf` | `[DeleteSelfChild]` |
| `Child_Execute` | `[ExecuteChild]` |

The same applies to the `Async` variants of these names (for example `DataPortal_FetchAsync`).

Before:

```csharp
[Serializable]
public class Customer : BusinessBase<Customer>
{
  private void DataPortal_Fetch(int id)
  {
    // ...
  }
}
```

After:

```csharp
[Serializable]
public class Customer : BusinessBase<Customer>
{
  [Fetch]
  private void DataPortal_Fetch(int id)
  {
    // ...
  }
}
```

Once a method has an attribute, you can rename it to anything you like (for example `Fetch`).

A method with a legacy name and no attribute is never called. Usually the data portal then throws an exception at runtime because it can't find a matching operation method. But when a CSLA base class has its own operation method, the base class method runs instead and your method is silently skipped. For example:

* A child class with an attribute-less `Child_Create()` gets `BusinessBase.Child_Create()`, which only checks business rules.
* A list class that overrides `Child_Update(params object[])` without `[UpdateChild]` gets the base `Child_UpdateAsync`, which updates the child items but not anything else your override did.

Use the `CSLA0014` analyzer (below) to find these methods rather than relying on runtime errors.

### Finding affected methods

The `CSLA0014` analyzer finds methods that use a legacy operation name but have no operation attribute. In CSLA 11 it reports a warning instead of an informational message. The analyzer's code fix adds the correct attribute for you. See [CSLA0014](analyzers/CSLA0014-DoesOperationHaveAttributeAnalyzer.md).

### Base class changes

* **Default child create:** `BusinessBase`, `BusinessListBase`, and `BusinessBindingListBase` each have a `protected virtual Child_Create()` method that is now marked with `[CreateChild]`. It's still called when you create a child with no criteria and your class has no better matching `[CreateChild]` method. `BusinessBase.Child_Create()` checks the object's business rules. If you override `Child_Create()`, the override inherits the attribute, so you don't need to add one.
* **"Not supported" operations:** `CommandBase`, `ReadOnlyBase`, `ReadOnlyListBase`, `ReadOnlyBindingListBase`, `NameValueListBase`, `DynamicListBase`, and `DynamicBindingListBase` no longer have private `DataPortal_XYZ` methods that throw `NotSupportedException` (for example, calling update on a read-only object). Those calls now fail with the data portal's normal "method not found" error.
* **ObjectFactory child create:** creating a child of a type that uses `[ObjectFactory]` no longer looks for a method named `Child_Create` on the factory or the business class. The data portal calls the business class's `[CreateChild]` method (or the base class default described above).
* **Sync `Child_Update`:** the synchronous `Child_Update(params object[])` helper on `BusinessListBase`, `BusinessBindingListBase`, and `BusinessDocumentBase` has no operation attribute. You can still call it from your own synchronous `[Update]`, `[Insert]`, or `[DeleteSelf]` methods. The data portal itself calls `Child_UpdateAsync`, which is marked with `[UpdateChild]`. If you override `Child_Update` so the data portal calls your override, add `[UpdateChild]` to it, or override `Child_UpdateAsync` instead.
