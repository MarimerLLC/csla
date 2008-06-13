using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  [Serializable]
  public class SingleCriteria<T> : CriteriaBase
  {
    static SingleCriteria()
    {
    }

    public static readonly PropertyInfo<T> ValueProperty = PropertyInfoManager.RegisterProperty<T>(
        typeof(SingleCriteria<T>),
        new PropertyInfo<T>("Value"));

    public T Value
    {
      get
      {
        return GetProperty<T>(ValueProperty);
      }
      private set
      {
        SetProperty<T>(ValueProperty, value);
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
