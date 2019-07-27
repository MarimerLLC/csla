//-----------------------------------------------------------------------
// <copyright file="PrimitiveCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class used as a wrapper for criteria based requests that use primitives</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Class used as a wrapper for criteria based requests that use primitives
  /// </summary>
  [Serializable]
  public class PrimitiveCriteria : IMobileObject
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PrimitiveCriteria() { }

    /// <summary>
    /// New instance of the criteria
    /// </summary>
    /// <param name="value">Criteria value</param>
    public PrimitiveCriteria(object value)
    {
      _value = value;
    }

    private object _value;
    /// <summary>
    /// Criteria value
    /// </summary>
    public object Value
    {
      get { return _value; }
      private set { _value = value; }
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
      info.AddValue("_value", _value);
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
      _value = info.GetValue<object>("_value");
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
    }

    #endregion
  }
}