#if !(ANDROID || IOS) && !NETFX_CORE
#endif

namespace Csla.Security
{
  /// <summary>
  /// Security exception.
  /// </summary>
  [Serializable]
  public class SecurityException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public SecurityException()
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Exception text.</param>
    public SecurityException(string message)
      : base(message)
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Exception text.</param>
    /// <param name="innerException">Inner exception.</param>
    public SecurityException(string message, Exception innerException)
      : base(message, innerException)
    { }
  }
}

