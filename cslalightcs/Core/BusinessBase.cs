using System;
using System.ComponentModel;
using Csla.Core.FieldManager;

namespace Csla.Core
{
  [Serialization.Serializable]
  public class BusinessBase : Silverlight.MobileObject, ICloneable
  {
    private bool _isNew = true;
    private bool _isDeleted;
    private bool _isDirty = true;
    private bool _neverCommitted = true;
    private bool _disableIEditableObject;
    private bool _isChild;
    private int _editLevelAdded;
    private Validation.ValidationRules _validationRules;
    //private FieldManager.FieldDataManager _fieldManager;

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

    public static PropertyInfo<T> RegisterProperty<T>(Type ownerType, PropertyInfo<T> property)
    {
      return PropertyInfoManager.RegisterProperty<T>(ownerType, property);
    }
  }
}
