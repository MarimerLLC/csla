using System;
using System.ComponentModel;

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
    private FieldManager.FieldDataManager _fieldManager;

    #region Serialization

    protected override object GetValue(System.Reflection.FieldInfo field)
    {
      if (field.DeclaringType == typeof(BusinessBase))
        return field.GetValue(this);
      else
        return base.GetValue(field);
    }

    protected override void SetValue(System.Reflection.FieldInfo field, object value)
    {
      if (field.DeclaringType == typeof(BusinessBase))
        field.SetValue(this, value);
      else
        base.SetValue(field, value);
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

  }
}
