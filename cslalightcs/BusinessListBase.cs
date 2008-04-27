using System;
using System.ComponentModel;
using Csla.Serialization;
using Csla.Silverlight;

namespace Csla
{
  [Serializable]
  public class BusinessListBase<T, C> : MobileList<C>, ICloneable
    where T: BusinessListBase<T, C>
  {
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
