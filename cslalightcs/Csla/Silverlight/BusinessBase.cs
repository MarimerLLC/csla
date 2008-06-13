using System;
using Csla.Serialization;

namespace Csla.Silverlight
{
  [Serializable]
  public class BusinessBase<T> : Csla.BusinessBase<T>
    where T:BusinessBase<T>
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
  }
}
