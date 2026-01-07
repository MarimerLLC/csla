//-----------------------------------------------------------------------
// <copyright file="MobileObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Inherit from this base class to easily</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.IO;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable class.
  /// </summary>
  [Serializable]
  public abstract class MobileObject : IMobileObject, IMobileObjectMetastate
  {
    #region Serialize

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info, StateMode.Serialization);
    }

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetState(SerializationInfo info, StateMode mode)
    { }

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

    #endregion

    #region Deserialize

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info, StateMode.Serialization);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetState(SerializationInfo info, StateMode mode)
    { }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

    #endregion

    #region IMobileObjectMetastate Members

    /// <summary>
    /// Override this method to write field values directly
    /// to a binary stream for metastate serialization.
    /// </summary>
    /// <param name="writer">Binary writer for the output stream.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetMetastate(BinaryWriter writer)
    { }

    /// <summary>
    /// Override this method to read field values directly
    /// from a binary stream for metastate deserialization.
    /// </summary>
    /// <param name="reader">Binary reader for the input stream.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetMetastate(BinaryReader reader)
    { }

    /// <inheritdoc />
    byte[] IMobileObjectMetastate.GetMetastate()
    {
      using var stream = new MemoryStream();
      using var writer = new BinaryWriter(stream);
      OnGetMetastate(writer);
      return stream.ToArray();
    }

    /// <inheritdoc />
    void IMobileObjectMetastate.SetMetastate(byte[] metastate)
    {
      if (metastate == null)
        throw new ArgumentNullException(nameof(metastate));

      using var stream = new MemoryStream(metastate);
      using var reader = new BinaryReader(stream);
      OnSetMetastate(reader);
    }

    #endregion
  }
}