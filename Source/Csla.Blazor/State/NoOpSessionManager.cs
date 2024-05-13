//-----------------------------------------------------------------------
// <copyright file="NoOpSessionManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>NoOp user session data implementation</summary>
//-----------------------------------------------------------------------

using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// If <see cref="Configuration.BlazorServerConfigurationOptions.UseInMemoryApplicationContextManager"/> is set to <see langword="true"/> then this class
  /// will be registered with DI as the default implementation of <see cref="ISessionManager"/>, because in that scenario there should be NO references to any
  /// of the state or session management mechanisms which support the flow of <see cref="ApplicationContext"/> between Server-Interactive and WASM-Interactive code.
  /// </summary>
  public class NoOpSessionManager : ISessionManager
  {
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Session GetCachedSession()
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Session GetSession()
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public void PurgeSessions(TimeSpan expiration)
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Task<Session> RetrieveSession()
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Task<Session> RetrieveSession(TimeSpan timeout)
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Task SendSession()
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Task SendSession(TimeSpan timeout)
    { throw new NotSupportedException(); }
    /// <summary>
    /// Not supported
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public void UpdateSession(Session newSession)
    { throw new NotSupportedException(); }
  }
}
