using System;
using Csla.Serialization;

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
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    public UndoException(string message)
      : base(message)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    public UndoException(string message, Exception ex)
      : base(message, ex)
    { }
  }
}
