//-----------------------------------------------------------------------
// <copyright file="UndoException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception indicating a problem with the</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Exception indicating a problem with the
  /// use of the n-level undo feature in
  /// CSLA .NET.
  /// </summary>
  /// <remarks></remarks>
  [Serializable()]
  public class UndoException : Exception
  {
    /// <summary>
    /// The type name of the object that caused this issue
    /// </summary>
    public string TypeName;
    /// <summary>
    /// The parent's type name of the object that caused this issue or null
    /// </summary>
    public string ParentTypeName;
    /// <summary>
    /// Object EditLevel
    /// </summary>
    public int CurrentEditLevel;
    /// <summary>
    /// Expected object EditLevel
    /// </summary>
    public int ExpectedEditLevel;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="typeName">The type name of the object that caused this issue</param>
    /// <param name="parentTypeName">The parent's type name of the object that caused this issue or null</param>
    /// <param name="currentEditLevel">Object EditLevel</param>
    /// <param name="expectedEditLevel">Expected object EditLevel</param>
    public UndoException(string message, string typeName, string parentTypeName, int currentEditLevel, int expectedEditLevel)
      : base(message)
    {
      TypeName = typeName;
      ParentTypeName = parentTypeName;
      CurrentEditLevel = currentEditLevel;
      ExpectedEditLevel = expectedEditLevel;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    public UndoException(string message, Exception ex)
      : base(message, ex)
    {

    }

#if !NETFX_CORE && !IOS && !ANDROID
    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    protected UndoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {

    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {

      base.GetObjectData(info, context);

    }
#endif
  }
}