//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
namespace Csla.Channels.Local
{
  /// <summary>
  /// Options for LocalProxy
  /// </summary>
  public class LocalProxyOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether the LocalProxy
    /// should create a DI scope for each data portal call.
    /// (default is true)
    /// </summary>
    public bool CreateScopePerCall { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether any
    /// synchronization context should be flowed to
    /// child tasks by LocalProxy. Setting this 
    /// to true may restrict or eliminate the 
    /// use of background threads by LocalProxy.
    /// (default is false)
    /// </summary>
    public bool FlowSynchronizationContext { get; set; } = false;
  }
}