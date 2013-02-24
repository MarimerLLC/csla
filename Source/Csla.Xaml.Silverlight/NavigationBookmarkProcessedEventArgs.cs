﻿//-----------------------------------------------------------------------
// <copyright file="NavigationBookmarkProcessedEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Event arguments class that is part of Navigator event</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Xaml
{
  /// <summary>
  /// Event arguments class that is part of Navigator event
  /// that is fired after a bookmakr has been processed
  /// in response to browser's back or forward button processing.
  /// </summary>
  public class NavigationBookmarkProcessedEventArgs : EventArgs
  {
    bool _success;

    /// <summary>
    /// New instance of event arguments
    /// </summary>
    /// <param name="success">
    /// True if bookamrk was processed successfully.
    /// </param>
    public NavigationBookmarkProcessedEventArgs(bool success)
    {
      _success = success;
    }

    /// <summary>
    /// Indicates if bookmakr was processed successfully
    /// and control is displayed.
    /// </summary>
    public bool Success { get { return _success; } }
  }
}