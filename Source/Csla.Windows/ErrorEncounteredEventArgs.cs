//-----------------------------------------------------------------------
// <copyright file="ErrorEncounteredEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event args indicating an error.</summary>
//-----------------------------------------------------------------------

namespace Csla.Windows
{
  /// <summary>
  /// Event args indicating an error.
  /// </summary>
  public class ErrorEncounteredEventArgs : CslaActionEventArgs
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="commandName">
    /// Name of the command.
    /// </param>
    /// <param name="ex">
    /// Reference to the exception.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="commandName"/> or <paramref name="ex"/> is <see langword="null"/>.</exception>
    public ErrorEncounteredEventArgs(string commandName, Exception ex)
      : base(commandName)
    {
      Ex = ex ?? throw new ArgumentNullException(nameof(ex));
    }

    /// <summary>
    /// Gets a reference to the exception object.
    /// </summary>
    public Exception Ex { get; }
  }
}