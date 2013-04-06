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
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    public UndoException(string message)
      : base(message)
    {

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

  }
}
