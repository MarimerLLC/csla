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
  }
}
