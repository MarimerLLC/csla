using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  [Serializable]
  public class SingleCriteria<T> : CriteriaBase
  {
    private T _value;

    public T Value
    {
      get
      {
        return _value;
      }
      private set
      {
        _value = value;
      }
    }

    public SingleCriteria() 
      : base()
    { }

    public SingleCriteria(Type objectType, T value)
      : base(objectType)
    {
      this.Value = value;
    }

    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Silverlight.SingleCriteria._value", _value);
    }

    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      _value = info.GetValue<T>("Csla.Silverlight.SingleCriteria._value");
    }
  }
}
