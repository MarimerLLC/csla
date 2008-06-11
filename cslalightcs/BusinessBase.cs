using System;

namespace Csla
{
  [Serialization.Serializable]
  public class BusinessBase<T> : Core.BusinessBase
    where T: BusinessBase<T>
  {

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
