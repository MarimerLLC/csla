//-----------------------------------------------------------------------
// <copyright file="PropertyLoadException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception indicating a failure to</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Exception indicating a failure to
  /// set a property's field.
  /// </summary>
  /// <remarks></remarks>
  [Serializable]
  public class PropertyLoadException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public PropertyLoadException(string message)
      : base(message)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
    public PropertyLoadException(string message, Exception ex)
      : base(message, ex)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      Guard.NotNull(ex);
    }
  }
}