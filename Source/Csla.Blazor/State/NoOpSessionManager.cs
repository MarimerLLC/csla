//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
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
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Session GetCachedSession()
    { throw new NotImplementedException(); }
    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Session GetSession()
    { throw new NotImplementedException(); }
    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void PurgeSessions(TimeSpan expiration)
    { throw new NotImplementedException(); }
    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Session> RetrieveSession()
    { throw new NotImplementedException(); }
    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SendSession()
    { throw new NotImplementedException(); }
    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void UpdateSession(Session newSession)
    { throw new NotImplementedException(); }
  }
}
