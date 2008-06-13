using System;
using System.ComponentModel;
using Csla.Core.FieldManager;

namespace Csla.Core
{
  [Serialization.Serializable]
  public class BusinessBase : MobileObject, ICloneable
  {
    private bool _isNew = true;
    private bool _isDeleted;
    private bool _isDirty = true;
    private bool _neverCommitted = true;
    private bool _disableIEditableObject;
    private bool _isChild;
    private int _editLevelAdded;
    private Validation.ValidationRules _validationRules;

    #region FieldManager

    private FieldManager.FieldDataManager _fieldManager;

    public static PropertyInfo<T> RegisterProperty<T>(Type ownerType, PropertyInfo<T> property)
    {
      return PropertyInfoManager.RegisterProperty<T>(ownerType, property);
    }

    protected FieldDataManager FieldManager
    {
      get
      {
        if (_fieldManager == null)
        {
          _fieldManager = new FieldDataManager(this.GetType());
        }
        return _fieldManager;
      }
    }

    protected T GetProperty<T>(IPropertyInfo propertyInfo)
    {
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      return (T)(data != null ? data.Value : null);
    }

    protected void SetProperty<T>(IPropertyInfo propertyInfo, T value)
    {
      FieldManager.SetFieldData<T>(propertyInfo, value);
    }

    #endregion

    #region MobileFormatter

    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Core.BusinessBase._isNew", _isNew);
      info.AddValue("Csla.Core.BusinessBase._isDeleted", _isDeleted);
      info.AddValue("Csla.Core.BusinessBase._isDirty", _isDirty);
      info.AddValue("Csla.Core.BusinessBase._neverCommitted", _neverCommitted);
      info.AddValue("Csla.Core.BusinessBase._disableIEditableObject", _disableIEditableObject);
      info.AddValue("Csla.Core.BusinessBase._isChild", _isChild);
      info.AddValue("Csla.Core.BusinessBase._editLevelAdded", _editLevelAdded);
    }

    protected override void OnGetChildren(
      Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);
      var fieldManagerInfo = formatter.SerializeObject(_fieldManager);
      info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
    }

    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      var childData = info.Children["_fieldManager"];
      _fieldManager = (FieldDataManager)formatter.GetObject(childData.ReferenceId);
      base.OnSetChildren(info, formatter);
    }

    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
    }
    
    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    #endregion

    static BusinessBase() { }

  }
}
