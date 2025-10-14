﻿using Csla;

namespace GraphMergerTest.Business
{
  [Serializable]
  public abstract class ChildFactoryBase<T, C>
    : ReadOnlyBase<T>
    where T : ChildFactoryBase<T, C>
  {
    public static readonly PropertyInfo<C> ResultProperty = RegisterProperty<C>(nameof(Result));

    public C Result
    {
      get => ReadProperty(ResultProperty);
      protected set => LoadProperty(ResultProperty, value);
    }
  }
}
