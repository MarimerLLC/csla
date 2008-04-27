using System;
using Csla.Serialization;

namespace Csla.Silverlight
{
  [Serializable]
  public class CriteriaBase : MobileObject
  {
    #region Serialization

    protected override object GetValue(System.Reflection.FieldInfo field)
    {
      if (field.DeclaringType == typeof(CriteriaBase))
        return field.GetValue(this);
      else
        return base.GetValue(field);
    }

    protected override void SetValue(System.Reflection.FieldInfo field, object value)
    {
      if (field.DeclaringType == typeof(CriteriaBase))
        field.SetValue(this, value);
      else
        base.SetValue(field, value);
    }

    #endregion

    public string TypeName { get; protected set; }

    public CriteriaBase(Type objectType)
    {
      this.TypeName = objectType.FullName + "," + objectType.Assembly.FullName;
    }

    protected CriteriaBase()
    { }
  }
}
