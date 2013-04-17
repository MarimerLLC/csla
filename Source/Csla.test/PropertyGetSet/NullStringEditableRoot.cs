using System;
using Csla;

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class NullStringEditableRoot : BusinessBase<NullStringEditableRoot>
  {
    #region Business Methods

    #region SetProperty and SetPropertyConvert

    public static readonly PropertyInfo<string> UsesSetManagedProperty = RegisterProperty<string>(c => c.UsesSetManaged);
    public string UsesSetManaged
    {
      get { return GetProperty(UsesSetManagedProperty); }
      set { SetProperty(UsesSetManagedProperty, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    public static readonly PropertyInfo<string> UsesSetBackingProperty = RegisterProperty<string>(c => c.UsesSetBacking, RelationshipTypes.PrivateField);
    // [NotUndoable, NonSerialized]
    private string _usesSetBacking = UsesSetBackingProperty.DefaultValue;
    public string UsesSetBacking
    {
      get { return GetProperty(UsesSetBackingProperty, _usesSetBacking); }
      set { SetProperty("UsesSetBacking", ref _usesSetBacking, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    public static readonly PropertyInfo<Csla.SmartDate> UsesSetConvertManagedSmartDateProperty = RegisterProperty<Csla.SmartDate>(c => c.UsesSetConvertManagedSmartDate);
    public string UsesSetConvertManagedSmartDate
    {
      get { return GetPropertyConvert<Csla.SmartDate, string>(UsesSetConvertManagedSmartDateProperty); }
      set { SetPropertyConvert<Csla.SmartDate, string>(UsesSetConvertManagedSmartDateProperty, value); }
    }

    //    protected void SetPropertyConvert<P, V>(string propertyName, ref P field, V newValue, Security.NoAccessBehavior noAccess)
    public static readonly PropertyInfo<Csla.SmartDate> UsesSetConvertBackingSmartDateProperty = RegisterProperty<Csla.SmartDate>(c => c.UsesSetConvertBackingSmartDate, RelationshipTypes.PrivateField);
    private Csla.SmartDate _usesSetConvertBackingSmartDate = UsesSetConvertBackingSmartDateProperty.DefaultValue;
    public string UsesSetConvertBackingSmartDate
    {
      get { return GetProperty(UsesSetConvertBackingSmartDateProperty, _usesSetConvertBackingSmartDate); }
      set { SetPropertyConvert<Csla.SmartDate, string>("UsesSetConvertBackingSmartDate", ref _usesSetConvertBackingSmartDate, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    public static readonly PropertyInfo<object> UsesSetConvertManagedObjectProperty = RegisterProperty<object>(c => c.UsesSetConvertManagedObject);
    public string UsesSetConvertManagedObject
    {
      get { return GetPropertyConvert<object, string>(UsesSetConvertManagedObjectProperty); }
      set { SetPropertyConvert<object, string>(UsesSetConvertManagedObjectProperty, value); }
    }

    //    protected void SetPropertyConvert<P, V>(string propertyName, ref P field, V newValue, Security.NoAccessBehavior noAccess)
    public static readonly PropertyInfo<object> UsesSetConvertBackingObjectProperty = RegisterProperty<object>(c => c.UsesSetConvertBackingObject, RelationshipTypes.PrivateField);
    private object _usesSetConvertBackingObject = UsesSetConvertBackingObjectProperty.DefaultValue;
    public string UsesSetConvertBackingObject
    {
      get { return GetPropertyConvert<object, string>(UsesSetConvertBackingObjectProperty, _usesSetConvertBackingObject); }
      set { SetPropertyConvert<object, string>("UsesSetConvertBackingObject", ref _usesSetConvertBackingObject, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    #endregion

    #region LoadProperty and LoadPropertyConvert

    public static readonly PropertyInfo<string> UsesLoadProperty = RegisterProperty<string>(c => c.UsesLoad);
    public string UsesLoad
    {
      get { return GetProperty(UsesLoadProperty); }
      set { LoadProperty(UsesLoadProperty, value); }
    }

    public static readonly PropertyInfo<string> UsesLoadMarkDirtyProperty = RegisterProperty<string>(c => c.UsesLoadMarkDirty);
    public string UsesLoadMarkDirty
    {
      get { return GetProperty(UsesLoadMarkDirtyProperty); }
      set { LoadPropertyMarkDirty(UsesLoadMarkDirtyProperty, value); }
    }

    public static readonly PropertyInfo<object> UsesLoadConvertSmartDateProperty = RegisterProperty<object>(c => c.UsesLoadConvertSmartDate);
    public string UsesLoadConvertSmartDate
    {
      get { return GetPropertyConvert<object, string>(UsesLoadConvertSmartDateProperty); }
      set { LoadPropertyConvert<object, string>(UsesLoadConvertSmartDateProperty, value); }
    }

    public static readonly PropertyInfo<object> UsesLoadConvertObjectProperty = RegisterProperty<object>(c => c.UsesLoadConvertObject);
    public string UsesLoadConvertObject
    {
      get { return GetPropertyConvert<object, string>(UsesLoadConvertObjectProperty); }
      set { LoadPropertyConvert<object, string>(UsesLoadConvertObjectProperty, value); }
    }

    #endregion

    #endregion

    #region Factory Methods

    public static NullStringEditableRoot NewEditableRoot()
    {
      //return DataPortal.Create<NullStringEditableRoot>();
      return new NullStringEditableRoot();
    }

    //public static NullStringEditableRoot GetEditableRoot(int id)
    //{
    //  return DataPortal.Fetch<NullStringEditableRoot>(id);
    //}

    //public static void DeleteEditableRoot(int id)
    //{
    //  DataPortal.Delete<NullStringEditableRoot>(id);
    //}

    private NullStringEditableRoot()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.DataPortal_Create();
    }

    //private void DataPortal_Fetch(int criteria)
    //{
    //  // TODO: load values
    //}

    //[Transactional(TransactionalTypes.TransactionScope)]
    //protected override void DataPortal_Insert()
    //{
    //  // TODO: insert values
    //}

    //[Transactional(TransactionalTypes.TransactionScope)]
    //protected override void DataPortal_Update()
    //{
    //  // TODO: update values
    //}

    //[Transactional(TransactionalTypes.TransactionScope)]
    //protected override void DataPortal_DeleteSelf()
    //{
    //  DataPortal_Delete(this.Id);
    //}

    //[Transactional(TransactionalTypes.TransactionScope)]
    //private void DataPortal_Delete(int criteria)
    //{
    //  // TODO: delete values
    //}

    #endregion
  }
}
