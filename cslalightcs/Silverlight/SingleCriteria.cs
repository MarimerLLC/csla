using System;
using Csla.Serialization;

namespace Csla.Silverlight
{
  [Serializable]
  public class SingleCriteria<T> : CriteriaBase
  {
    #region Serialization

    protected override object GetValue(System.Reflection.FieldInfo field)
    {
      if (field.DeclaringType == typeof(SingleCriteria<T>))
        return field.GetValue(this);
      else
        return base.GetValue(field);
    }

    protected override void SetValue(System.Reflection.FieldInfo field, object value)
    {
      if (field.DeclaringType == typeof(SingleCriteria<T>))
        field.SetValue(this, value);
      else
        base.SetValue(field, value);
    }

    #endregion

    public T Value { get; private set; }

    public SingleCriteria(Type objectType, T value)
      : base(objectType)
    {
      this.Value = value;
    }

    private SingleCriteria()
    { }
  }
}
