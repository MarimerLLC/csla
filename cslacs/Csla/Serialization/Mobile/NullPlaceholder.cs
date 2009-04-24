using System;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Placeholder for null child objects.
  /// </summary>
  [Serializable()]
  public sealed class NullPlaceholder : IMobileObject
  {
    #region Constructors

    public NullPlaceholder()
    {
      // Nothing
    }

    #endregion

    #region IMobileObject Members

    public void GetState(SerializationInfo info)
    {
      // Nothing
    }

    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      // Nothing
    }

    public void SetState(SerializationInfo info)
    {
      // Nothing
    }

    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      // Nothing
    }

    #endregion
  }
}
