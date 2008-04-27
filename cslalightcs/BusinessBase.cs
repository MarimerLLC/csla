using System;

namespace Csla
{
  [Serialization.Serializable]
  public class BusinessBase<T> : Core.BusinessBase
    where T: BusinessBase<T>
  {
    #region Serialization

    protected override object GetValue(System.Reflection.FieldInfo field)
    {
      if (field.DeclaringType == typeof(BusinessBase<T>))
        return field.GetValue(this);
      else
        return base.GetValue(field);
    }

    protected override void SetValue(System.Reflection.FieldInfo field, object value)
    {
      if (field.DeclaringType == typeof(BusinessBase<T>))
        field.SetValue(this, value);
      else
        base.SetValue(field, value);
    }

    #endregion

    #region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

  }
}
