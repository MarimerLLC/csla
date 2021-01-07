//-----------------------------------------------------------------------
// <copyright file="NullPlaceholder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Placeholder for null child objects.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Placeholder for null child objects.
  /// </summary>
  [Serializable()]
  public sealed class NullPlaceholder : IMobileObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public NullPlaceholder()
    {
      // Nothing
    }

    #region IMobileObject Members

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its data. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    public void GetState(SerializationInfo info)
    {
      // Nothing
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its child references. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the serialization.
    /// </param>
    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      // Nothing
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    public void SetState(SerializationInfo info)
    {
      // Nothing
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should deserialize its child references. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the deserialization.
    /// </param>
    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      // Nothing
    }

    #endregion
  }
}