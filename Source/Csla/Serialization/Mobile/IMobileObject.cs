//-----------------------------------------------------------------------
// <copyright file="IMobileObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface to be implemented by any object</summary>
//-----------------------------------------------------------------------
namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Interface to be implemented by any object
  /// that supports serialization by the
  /// SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public interface IMobileObject
  {
    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its data. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    void GetState(SerializationInfo info);

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
    void GetChildren(SerializationInfo info, MobileFormatter formatter);

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    void SetState(SerializationInfo info);

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
    void SetChildren(SerializationInfo info, MobileFormatter formatter);
  }
}