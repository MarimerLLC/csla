//-----------------------------------------------------------------------
// <copyright file="IDashboard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Data portal dashboard</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Data portal server dashboard.
  /// </summary>
  public interface IDashboard
  {
    /// <summary>
    /// Gets the time the data portal was first invoked
    /// </summary>
    DateTimeOffset FirstCall { get; }
    /// <summary>
    /// Gets the most recent time the data portal
    /// was invoked
    /// </summary>
    DateTimeOffset LastCall { get; }
    /// <summary>
    /// Gets the total number of times the data portal
    /// has been invoked
    /// </summary>
    long TotalCalls { get; }
    /// <summary>
    /// Gets the failed number of times the data portal
    /// has been invoked
    /// </summary>
    long FailedCalls { get; }
    /// <summary>
    /// Gets the completed number of times the data portal
    /// has been invoked
    /// </summary>
    long CompletedCalls { get; }
    /// <summary>
    /// Called by the data portal to indicate a call
    /// has been inititalized.
    /// </summary>
    /// <param name="e">Interceptor arguments</param>
    void InitializeCall(InterceptArgs e);
    /// <summary>
    /// Called by the data portal to indicate a call
    /// has been completed.
    /// </summary>
    /// <param name="e">Interceptor arguments</param>
    void CompleteCall(InterceptArgs e);
  }
}