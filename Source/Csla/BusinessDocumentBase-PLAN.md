# BusinessDocumentBase Implementation Plan

## Overview

This document outlines the plan to create a new `BusinessDocumentBase<T, C>` class that combines the capabilities of both `BusinessBase<T>` and `BusinessListBase<T, C>`. This enables the "Document Pattern" where a single business object has its own properties AND contains a collection of child items.

### Use Case Example

```csharp
// An Invoice that has its own properties AND contains LineItems
public class Invoice : BusinessDocumentBase<Invoice, LineItem>
{
    // Invoice properties (InvoiceNumber, Date, Customer, etc.)
    public static readonly PropertyInfo<string> InvoiceNumberProperty = RegisterProperty<string>(nameof(InvoiceNumber));
    public string InvoiceNumber
    {
        get => GetProperty(InvoiceNumberProperty);
        set => SetProperty(InvoiceNumberProperty, value);
    }

    // LineItems are managed by the base class
    // Access via: invoice.Items, invoice[0], invoice.Add(lineItem), etc.
}
```

## Requirements

1. **Full BusinessBase functionality**: Properties, business rules, authorization, validation, n-level undo
2. **Full BusinessListBase functionality**: Child collection management, deleted list tracking, collection change notifications
3. **Interface compatibility**: Must be substitutable for both `BusinessBase` and `BusinessListBase` where appropriate
4. **Serialization**: Must serialize both object properties AND child items
5. **Coordinated state**: `IsDirty`, `IsValid`, `IsBusy` must aggregate both object and children state

---

## Interface Analysis

### Interfaces Implemented by BusinessBase<T>

#### From Inheritance Hierarchy
| Class | Interfaces |
|-------|------------|
| `MobileObject` | `IMobileObject`, `IMobileObjectMetastate` |
| `BindableBase` | `INotifyPropertyChanged`, `INotifyPropertyChanging` |
| `UndoableBase` | `IUndoableObject`, `IUseApplicationContext` |
| `Core.BusinessBase` | `IEditableBusinessObject`, `IEditableObject`, `ICloneable`, `IAuthorizeReadWrite`, `IParent`, `IDataPortalTarget`, `IManageProperties`, `IHostRules`, `ICheckRules`, `INotifyChildChanged`, `ISerializationNotification`, `IDataErrorInfo`, `INotifyDataErrorInfo`, `IUseFieldManager`, `IUseBusinessRules` |
| `BusinessBase<T>` | `ISavable`, `ISavable<T>`, `IBusinessBase` |

### Interfaces Implemented by BusinessListBase<T, C>

#### From Inheritance Hierarchy
| Class | Interfaces |
|-------|------------|
| `ObservableCollection<C>` | `IList<C>`, `ICollection<C>`, `IEnumerable<C>`, `INotifyCollectionChanged`, `INotifyPropertyChanged` |
| `MobileObservableCollection<C>` | `IMobileList`, `IMobileObject`, `IMobileObjectMetastate` |
| `ObservableBindingList<C>` | `IObservableBindingList`, `INotifyBusy`, `INotifyChildChanged`, `ISerializationNotification` |
| `BusinessListBase<T, C>` | `IContainsDeletedList`, `ISavable<T>`, `IDataPortalTarget`, `IBusinessListBase<C>`, `IUseApplicationContext` |

#### IBusinessListBase<C> Consolidates
- `IEditableCollection` (which includes `IBusinessObject`, `ISupportUndo`, `ITrackStatus`)
- `IUndoableObject`
- `ICloneable`
- `ISavable`
- `IParent`
- `IObservableBindingList`
- `INotifyChildChanged`
- `ISerializationNotification`
- `IMobileObject`
- `INotifyCollectionChanged`
- `INotifyPropertyChanged`
- `IList<C>`

---

## Interface Conflicts & Resolutions

### Conflict 1: ITrackStatus (IsNew, IsDeleted, IsDirty, IsValid)

| Property | BusinessBase Behavior | BusinessListBase Behavior | Resolution |
|----------|----------------------|--------------------------|------------|
| `IsNew` | True if object not yet persisted | Always `false` | **Use BusinessBase behavior** - the document itself can be new |
| `IsDeleted` | True if marked for deletion | Always `false` | **Use BusinessBase behavior** - the document itself can be deleted |
| `IsDirty` | True if object properties changed | True if any child is dirty or non-new deleted items exist | **Aggregate**: `ObjectIsDirty || ChildrenAreDirty || DeletedItemsExist` |
| `IsSelfDirty` | True if object's own properties changed | Same as `IsDirty` | **Use BusinessBase behavior** for object's own state |
| `IsValid` | True if no broken rules | True if all children valid | **Aggregate**: `ObjectIsValid && AllChildrenValid` |
| `IsSelfValid` | True if object's own rules pass | Same as `IsValid` | **Use BusinessBase behavior** for object's own state |
| `IsSavable` | Dirty && Valid && !Busy && Authorized | Dirty && Valid && !Busy && Authorized | **Same logic**, but using aggregated values |
| `IsBusy` | True if object is busy | True if any child is busy | **Aggregate**: `ObjectIsBusy || AnyChildIsBusy` |

### Conflict 2: IUndoableObject (EditLevel, CopyState, UndoChanges, AcceptChanges)

| Member | BusinessBase Behavior | BusinessListBase Behavior | Resolution |
|--------|----------------------|--------------------------|------------|
| `EditLevel` | Tracks object's edit depth | Tracks collection's edit depth | **Single EditLevel** - they must be synchronized |
| `CopyState` | Snapshots object properties | Cascades to all children + deleted | **Do both**: Snapshot object state AND cascade to children |
| `UndoChanges` | Restores object properties | Restores children, handles deletions | **Do both**: Restore object state AND cascade to children |
| `AcceptChanges` | Commits object properties | Commits children, clears deleted below level | **Do both**: Commit object state AND cascade to children |

### Conflict 3: ISavable / ISavable<T>

| Member | BusinessBase Behavior | BusinessListBase Behavior | Resolution |
|--------|----------------------|--------------------------|------------|
| `Save()` | Saves object via DataPortal.Update | Saves all children via DataPortal.Update | **Combined**: Save object (which triggers child saves in DataPortal methods) |
| `SaveAsync()` | Async version | Async version | Same |
| `SaveAndMergeAsync()` | Saves and merges result | Saves and merges result | Same |
| `Saved` event | Raised after save | Raised after save | Same |

### Conflict 4: IDataPortalTarget

| Member | BusinessBase Behavior | BusinessListBase Behavior | Resolution |
|--------|----------------------|--------------------------|------------|
| `MarkAsChild()` | Marks object as child | Marks collection as child | **Same** |
| `MarkNew()` | Sets `IsNew = true` | No-op | **Use BusinessBase behavior** |
| `MarkOld()` | Sets `IsNew = false` | No-op | **Use BusinessBase behavior** |
| `CheckRules()` | Executes business rules | No-op | **Use BusinessBase behavior** |
| `CheckRulesAsync()` | Async version | No-op (returns completed task) | **Use BusinessBase behavior** |

### Conflict 5: IEditableBusinessObject vs IEditableCollection

These are **different interfaces** for different purposes:
- `IEditableBusinessObject`: For single objects that can be children in a collection
- `IEditableCollection`: For collections that contain children

**Resolution**: `BusinessDocumentBase` implements **both** because it IS a business object that CAN be a child, AND it IS a collection that CONTAINS children.

### Conflict 6: IBusinessBase vs IBusinessListBase<C>

**Resolution**: Create a new consolidated interface `IBusinessDocumentBase<C>` that extends both:

```csharp
public interface IBusinessDocumentBase<C> : IBusinessBase, IBusinessListBase<C>
    where C : IEditableBusinessObject
{
    // Any additional members specific to document pattern
}
```

---

## Complete Interface List for BusinessDocumentBase<T, C>

### Must Implement (Union of Both)

```csharp
public abstract class BusinessDocumentBase<T, C> : BusinessBase<T>,
    // From BusinessBase (inherited)
    // - IEditableBusinessObject (includes IBusinessObject, ISupportUndo, IUndoableObject, ITrackStatus)
    // - IEditableObject
    // - ICloneable
    // - IAuthorizeReadWrite
    // - IParent
    // - IDataPortalTarget
    // - IManageProperties
    // - IHostRules
    // - ICheckRules
    // - INotifyChildChanged
    // - ISerializationNotification
    // - IDataErrorInfo
    // - INotifyDataErrorInfo
    // - IUseFieldManager
    // - IUseBusinessRules
    // - ISavable, ISavable<T>
    // - IBusinessBase
    // - IMobileObject, IMobileObjectMetastate
    // - INotifyPropertyChanged, INotifyPropertyChanging
    // - IUndoableObject
    // - IUseApplicationContext

    // Additional from BusinessListBase (must implement explicitly)
    IEditableCollection,           // Collection-specific editing
    IContainsDeletedList,          // Deleted items tracking
    IObservableBindingList,        // AddNew, AllowEdit, AllowRemove
    INotifyCollectionChanged,      // Collection change notifications
    IList<C>,                      // Full list interface
    IBusinessDocumentBase<C>       // New consolidated interface

    where T : BusinessDocumentBase<T, C>
    where C : IEditableBusinessObject
```

---

## Architecture Design

### Class Hierarchy

```
System.Object
└── MobileObject (IMobileObject, IMobileObjectMetastate)
    └── BindableBase (INotifyPropertyChanged, INotifyPropertyChanging)
        └── UndoableBase (IUndoableObject, IUseApplicationContext)
            └── Core.BusinessBase (IEditableBusinessObject, IEditableObject, ICloneable,
                │                   IAuthorizeReadWrite, IParent, IDataPortalTarget,
                │                   IManageProperties, IHostRules, ICheckRules,
                │                   INotifyChildChanged, ISerializationNotification,
                │                   IDataErrorInfo, INotifyDataErrorInfo,
                │                   IUseFieldManager, IUseBusinessRules)
                └── BusinessBase<T> (ISavable, ISavable<T>, IBusinessBase)
                    └── BusinessDocumentBase<T, C> (IEditableCollection, IContainsDeletedList,
                                                     IObservableBindingList, INotifyCollectionChanged,
                                                     IList<C>, IBusinessDocumentBase<C>)
```

### Internal Components

```
BusinessDocumentBase<T, C>
├── Inherited from BusinessBase<T>:
│   ├── FieldManager (property storage)
│   ├── BusinessRules (validation engine)
│   ├── Authorization cache
│   └── State tracking (IsNew, IsDeleted, _isDirty)
│
└── New components for collection support:
    ├── _items : MobileBindingList<C>      // Active child items
    ├── _deletedItems : MobileList<C>      // Deleted child items
    ├── _allowNew : bool                    // IObservableBindingList
    ├── _allowEdit : bool                   // IObservableBindingList
    ├── _allowRemove : bool                 // IObservableBindingList
    └── _raiseListChangedEvents : bool      // Notification control
```

---

## Implementation Plan

### Phase 1: Interface Definitions

#### Task 1.1: Create IBusinessDocumentBase<C> Interface
**File**: `Source/Csla/IBusinessDocumentBase.cs`

```csharp
public interface IBusinessDocumentBase<C> :
    IBusinessBase,           // All single-object interfaces
    IBusinessListBase<C>     // All collection interfaces
    where C : IEditableBusinessObject
{
    // Document-specific members (if any)
}
```

#### Task 1.2: Review/Update IEditableCollection
Ensure it has all necessary members for collection editing support.

### Phase 2: Core Class Structure

#### Task 2.1: Create BusinessDocumentBase Class Shell
**File**: `Source/Csla/BusinessDocumentBase.cs`

- Class declaration with all interfaces
- Generic constraints
- Constructor

#### Task 2.2: Implement Child Collection Storage
- `_items` field (active children)
- `_deletedItems` field (deleted children)
- `Items` property (public access)
- `DeletedList` property (protected access)

### Phase 3: Collection Interface Implementation

#### Task 3.1: IList<C> Implementation
- `Count`, `IsReadOnly`
- `this[int index]` indexer
- `Add`, `Insert`, `Remove`, `RemoveAt`, `Clear`
- `Contains`, `IndexOf`, `CopyTo`
- `GetEnumerator`

#### Task 3.2: INotifyCollectionChanged Implementation
- `CollectionChanged` event
- `OnCollectionChanged` method
- Raise events from Add/Remove/Clear/Set operations

#### Task 3.3: IObservableBindingList Implementation
- `AllowNew`, `AllowEdit`, `AllowRemove` properties
- `AddNew()`, `AddNewAsync()` methods
- `AddedNew` event

#### Task 3.4: IContainsDeletedList Implementation
- `DeletedList` property (IEnumerable<IEditableBusinessObject>)
- `ContainsDeleted(C item)` method

#### Task 3.5: IEditableCollection Implementation
- `RemoveChild(IEditableBusinessObject child)`
- `GetDeletedList()`
- `SetParent(IParent parent)`

### Phase 4: Child Item Management

#### Task 4.1: InsertItem Logic
- Validate item is marked as child
- Set parent reference to `this`
- Set ApplicationContext
- Set EditLevelAdded
- Ensure unique identity
- Hook child events
- Raise CollectionChanged

#### Task 4.2: RemoveItem Logic
- Move to deleted list (unless completely removing)
- Unhook child events
- Reset child edit level
- Mark child as deleted
- Raise CollectionChanged

#### Task 4.3: SetItem Logic
- Delete old item
- Insert new item
- Handle events appropriately

#### Task 4.4: ClearItems Logic
- Move all items to deleted list
- Unhook all events
- Raise Reset notification

### Phase 5: Status Property Overrides

#### Task 5.1: Override IsDirty
```csharp
public override bool IsDirty
{
    get
    {
        // Object's own dirty state
        if (base.IsDirty)
            return true;

        // Check deleted items (non-new deletions are dirty)
        foreach (var item in _deletedItems)
            if (!item.IsNew)
                return true;

        // Check active children
        foreach (var child in _items)
            if (child.IsDirty)
                return true;

        return false;
    }
}
```

#### Task 5.2: Override IsValid
```csharp
public override bool IsValid
{
    get
    {
        // Object's own validation
        if (!base.IsValid)
            return false;

        // Check all children
        foreach (var child in _items)
            if (!child.IsValid)
                return false;

        return true;
    }
}
```

#### Task 5.3: Override IsBusy
```csharp
public override bool IsBusy
{
    get
    {
        if (base.IsBusy)
            return true;

        foreach (var item in _deletedItems)
            if (item.IsBusy)
                return true;

        foreach (var child in _items)
            if (child.IsBusy)
                return true;

        return false;
    }
}
```

### Phase 6: N-Level Undo Support

#### Task 6.1: Override CopyState
```csharp
protected override void CopyState(int parentEditLevel, bool parentBindingEdit)
{
    // Copy object's own state
    base.CopyState(parentEditLevel, parentBindingEdit);

    // Cascade to all active children
    foreach (var child in _items)
        child.CopyState(EditLevel, false);

    // Cascade to deleted children
    foreach (var child in _deletedItems)
        child.CopyState(EditLevel, false);
}
```

#### Task 6.2: Override UndoChanges
- Restore object's own state
- Undo changes in all children
- Remove children added below current edit level
- Restore children deleted above current edit level

#### Task 6.3: Override AcceptChanges
- Accept object's own state
- Accept changes in all children
- Remove deleted children below current edit level
- Update EditLevelAdded for children

### Phase 7: Serialization Support

#### Task 7.1: Override OnGetState
```csharp
protected override void OnGetState(SerializationInfo info)
{
    base.OnGetState(info);
    // Add collection-specific state
    info.AddValue("_allowNew", _allowNew);
    info.AddValue("_allowEdit", _allowEdit);
    info.AddValue("_allowRemove", _allowRemove);
}
```

#### Task 7.2: Override OnSetState
```csharp
protected override void OnSetState(SerializationInfo info)
{
    base.OnSetState(info);
    _allowNew = info.GetValue<bool>("_allowNew");
    _allowEdit = info.GetValue<bool>("_allowEdit");
    _allowRemove = info.GetValue<bool>("_allowRemove");
}
```

#### Task 7.3: Override OnGetChildren
```csharp
protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
{
    base.OnGetChildren(info, formatter);

    // Serialize items collection
    var itemsInfo = formatter.SerializeObject(_items);
    info.AddChild("_items", itemsInfo.ReferenceId);

    // Serialize deleted items
    if (_deletedItems != null && _deletedItems.Count > 0)
    {
        var deletedInfo = formatter.SerializeObject(_deletedItems);
        info.AddChild("_deletedItems", deletedInfo.ReferenceId);
    }
}
```

#### Task 7.4: Override OnSetChildren
```csharp
protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
{
    base.OnSetChildren(info, formatter);

    if (info.Children.TryGetValue("_items", out var itemsChild))
        _items = (MobileBindingList<C>)formatter.GetObject(itemsChild.ReferenceId);

    if (info.Children.TryGetValue("_deletedItems", out var deletedChild))
        _deletedItems = (MobileList<C>)formatter.GetObject(deletedChild.ReferenceId);
}
```

#### Task 7.5: Override OnGetMetastate / OnSetMetastate
Handle binary metastate for collection-specific flags.

#### Task 7.6: Override Deserialized
- Call base
- Re-establish parent references for all children
- Hook child events

### Phase 8: Data Portal Integration

#### Task 8.1: Child_Update Implementation
```csharp
[EditorBrowsable(EditorBrowsableState.Advanced)]
protected virtual void Child_Update(params object[] parameters)
{
    var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();

    // Update deleted items first
    foreach (var child in _deletedItems)
        dp.UpdateChild(child, parameters);
    _deletedItems.Clear();

    // Update dirty active items
    foreach (var child in _items)
        if (child.IsDirty)
            dp.UpdateChild(child, parameters);
}
```

#### Task 8.2: Child_UpdateAsync Implementation
Async version of above.

#### Task 8.3: DataPortal_XYZ Event Handlers
Implement all DataPortal event handlers following same pattern as BusinessListBase.

### Phase 9: Additional Features

#### Task 9.1: Clone Support
Override `GetClone()` to ensure proper deep cloning of children.

#### Task 9.2: Child Event Handling
- Hook `PropertyChanged` on children
- Hook `BusyChanged` on children
- Hook `ChildChanged` on children
- Bubble events appropriately

#### Task 9.3: LoadListMode Support
Implement `LoadListMode` property/pattern for bulk loading without events.

#### Task 9.4: WaitForIdle Support
Override to wait for both object and all children.

### Phase 10: Testing & Documentation

#### Task 10.1: Unit Tests
- Status aggregation tests
- Serialization round-trip tests
- N-level undo tests
- Collection operation tests
- Data portal integration tests

#### Task 10.2: Integration Tests
- Test with actual child business objects
- Test serialization across data portal
- Test with UI data binding

#### Task 10.3: Documentation
- XML documentation on all public members
- Usage examples
- Migration guide for existing code

---

## File Locations

| File | Purpose |
|------|---------|
| `Source/Csla/IBusinessDocumentBase.cs` | New consolidated interface |
| `Source/Csla/BusinessDocumentBase.cs` | Main implementation class |
| `Source/Csla/Core/MobileBindingList.cs` | Internal collection class (if needed) |
| `Source/Csla.Tests/BusinessDocumentBaseTests.cs` | Unit tests |

---

## Risk Assessment

### High Risk Areas

1. **Serialization Coordination**: Must ensure both object state and children serialize correctly in all scenarios (clone, data portal, undo state)

2. **Edit Level Synchronization**: Object and children must maintain consistent edit levels through all operations

3. **Parent Reference Management**: Children must always have correct parent reference, especially after deserialization

4. **Event Cascade**: Child changes must properly bubble up without causing infinite loops

### Mitigation Strategies

1. **Comprehensive Testing**: Test all serialization paths (MobileFormatter, clone, data portal)

2. **Follow Existing Patterns**: Mirror BusinessListBase's edit level management exactly

3. **Deserialized Override**: Always re-establish parent references after deserialization

4. **Event Suppression**: Use flags to prevent event cascade during internal operations

---

## Success Criteria

1. ✅ Can create a document-style business object with properties AND children
2. ✅ `IsDirty`/`IsValid`/`IsBusy` correctly aggregate object and children state
3. ✅ N-level undo works correctly for both object properties and children
4. ✅ Serialization preserves both object state and children
5. ✅ Data portal save correctly persists object and children
6. ✅ Can be used anywhere a `BusinessBase` is expected (for single-object operations)
7. ✅ Can be used anywhere a `BusinessListBase` is expected (for collection operations)
8. ✅ Child changes bubble up through `ChildChanged` event
9. ✅ Collection changes raise `CollectionChanged` event
10. ✅ Works with UI data binding (Blazor, WPF, etc.)

---

## Appendix A: Interface Member Inventory

### IEditableCollection Members
```csharp
void RemoveChild(IEditableBusinessObject child);
object GetDeletedList();
void SetParent(IParent parent);
```

### IObservableBindingList Members
```csharp
bool AllowNew { get; set; }
bool AllowEdit { get; set; }
bool AllowRemove { get; set; }
object AddNew();
Task<object> AddNewAsync();
event EventHandler<AddedNewEventArgs> AddedNew;
```

### IContainsDeletedList Members
```csharp
IEnumerable<IEditableBusinessObject> DeletedList { get; }
```

### IList<C> Members
```csharp
C this[int index] { get; set; }
int Count { get; }
bool IsReadOnly { get; }
void Add(C item);
void Insert(int index, C item);
bool Remove(C item);
void RemoveAt(int index);
void Clear();
bool Contains(C item);
int IndexOf(C item);
void CopyTo(C[] array, int arrayIndex);
IEnumerator<C> GetEnumerator();
```

### INotifyCollectionChanged Members
```csharp
event NotifyCollectionChangedEventHandler CollectionChanged;
```

---

## Appendix B: Method Override Summary

| Method | Source | Override Required | Reason |
|--------|--------|-------------------|--------|
| `IsDirty` | BusinessBase | Yes | Aggregate children |
| `IsValid` | BusinessBase | Yes | Aggregate children |
| `IsBusy` | BusinessBase | Yes | Aggregate children |
| `CopyState` | UndoableBase | Yes | Cascade to children |
| `UndoChanges` | UndoableBase | Yes | Cascade to children |
| `AcceptChanges` | UndoableBase | Yes | Cascade to children |
| `OnGetState` | MobileObject | Yes | Add collection state |
| `OnSetState` | MobileObject | Yes | Restore collection state |
| `OnGetChildren` | MobileObject | Yes | Serialize children |
| `OnSetChildren` | MobileObject | Yes | Deserialize children |
| `OnGetMetastate` | MobileObject | Yes | Add collection metastate |
| `OnSetMetastate` | MobileObject | Yes | Restore collection metastate |
| `Deserialized` | Various | Yes | Re-establish parent refs |
| `GetClone` | BusinessBase | Yes | Clone children |
| `WaitForIdle` | BusinessBase | Yes | Wait for children |

---

## Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2024-XX-XX | TBD | Initial plan |
