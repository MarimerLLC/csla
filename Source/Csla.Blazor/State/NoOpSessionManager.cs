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
    /// Retrieves the session asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved session.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is called.</exception>
    public Task<Session> RetrieveSession(CancellationToken ct)
    { throw new NotImplementedException(); }

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
    /// Sends the session asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is called.</exception>
    public Task SendSession(CancellationToken ct)
    { throw new NotImplementedException(); }

    /// <summary>
    /// Not supported
    /// </summary>
    /// <param name="newSession">The new session.</param>
    /// <exception cref="NotSupportedException">Thrown when the method is called.</exception>
    public void UpdateSession(Session newSession)
    { throw new NotSupportedException(); }
  }
}
