//-----------------------------------------------------------------------
// <copyright file="PrimitiveCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class used as a wrapper for criteria based requests that use primitives</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Class used as a wrapper for criteria based requests that use primitives
  /// or types that have a registered custom serializer.
  /// </summary>
  [Serializable]
  public class PrimitiveCriteria : IMobileObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. It's okay to suppress because it can't be used by user code
    public PrimitiveCriteria() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// New instance of the criteria
    /// </summary>
    /// <param name="value">Criteria value</param>
    public PrimitiveCriteria(object value)
    {
      _value = value;
    }

    private const string _valueName = "_value";

    private object _value;
    /// <summary>
    /// Criteria value
    /// </summary>
    public object Value
    {
      get => _value;
      private set => _value = value;
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
      // when the value was serialized as a child by GetChildren
      // it must not also be written into the value list
      if (!info.Children.ContainsKey(_valueName))
        info.AddValue(_valueName, _value);
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
      if (_value is null)
        return;

      var valueType = _value.GetType();
      if (!SerializationInfo.IsNativeType(valueType) && formatter.IsTypeSerializable(valueType))
      {
        var childInfo = formatter.SerializeObject(_value);
        info.AddChild(_valueName, childInfo.ReferenceId);
      }
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
      // when the value was serialized as a child it is restored by SetChildren;
      // otherwise read it here, so a stream that is missing the value still fails fast
      if (!info.Children.ContainsKey(_valueName))
        _value = info.GetValue<object>(_valueName)!;
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
      if (info.Children.TryGetValue(_valueName, out var childData))
        _value = formatter.GetObject(childData.ReferenceId)!;
    }

    #endregion
  }
}