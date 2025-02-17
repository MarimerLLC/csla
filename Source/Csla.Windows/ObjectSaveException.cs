//-----------------------------------------------------------------------
// <copyright file="ObjectSaveException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception indicating a failure during an object</summary>
//-----------------------------------------------------------------------

using Csla.Properties;

namespace Csla.Windows
{
  /// <summary>
  /// Exception indicating a failure during an object
  /// save operation.
  /// </summary>
  public class ObjectSaveException : Exception 
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ObjectSaveException() { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">
    /// Exception message text.
    /// </param>
    public ObjectSaveException(string message) : base(message) { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">
    /// Exception message text.
    /// </param>
    /// <param name="innerException">
    /// Reference to an inner exception.
    /// </param>
    public ObjectSaveException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="innerException">
    /// Reference to an inner exception.
    /// </param>
    public ObjectSaveException(Exception innerException) : base(Resources.ExceptionOccurredDuringSaveOperation, innerException) { }
  }
}