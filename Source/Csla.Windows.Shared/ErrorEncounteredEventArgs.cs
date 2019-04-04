//-----------------------------------------------------------------------
// <copyright file="ErrorEncounteredEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event args indicating an error.</summary>
//-----------------------------------------------------------------------
using System;

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
    public ErrorEncounteredEventArgs(string commandName, Exception ex)
      : base(commandName)
    {
      _ex = ex;
    }

    private Exception _ex;

    /// <summary>
    /// Gets a reference to the exception object.
    /// </summary>
    public Exception Ex
    {
      get { return _ex; }
    }
  }
}