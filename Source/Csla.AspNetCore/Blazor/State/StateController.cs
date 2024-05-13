//-----------------------------------------------------------------------
// <copyright file="StateController.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gets and puts the current user session data</summary>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Csla.State;
using Csla.Security;
using Csla.Blazor.State.Messages;

namespace Csla.AspNetCore.Blazor.State
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  public class StateController(ApplicationContext applicationContext, ISessionManager sessionManager) : ControllerBase
  {
    /// <summary>
    /// Gets or sets a value indicating whether to flow the
    /// current user principal from the Blazor server to the
    /// Blazor WebAssembly client.
    /// </summary>
    protected bool FlowUserIdentityToWebAssembly { get; set; } = true;

    /// <summary>
    /// Gets current user session data in a serialized
    /// format.
    /// </summary>
    /// <param name="lastTouched">Last touched value from session</param>
    [HttpGet]
    public virtual StateResult Get(long lastTouched)
    {
      var result = new StateResult();
      var session = sessionManager.GetSession();
      if (session.LastTouched == lastTouched)
      {
        result.ResultStatus = ResultStatuses.NoUpdates;
      }
      else
      {
        var message = (SessionMessage)applicationContext.CreateInstanceDI(typeof(SessionMessage));
        message.Session = session;
        if (FlowUserIdentityToWebAssembly)
        {
          var principal = new CslaClaimsPrincipal(applicationContext.Principal);
          message.Principal = principal;
        }
        var formatter = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
        var buffer = new MemoryStream();
        formatter.Serialize(buffer, message);
        result.ResultStatus = ResultStatuses.Success;
        result.SessionData = buffer.ToArray();
      }
      return result;
    }

    /// <summary>
    /// Sets the current user session data from a
    /// serialized format.
    /// </summary>
    /// <param name="updatedSessionData"></param>
    [HttpPut]
    public virtual void Put(byte[] updatedSessionData)
    {
      var formatter = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
      var buffer = new MemoryStream(updatedSessionData)
      {
        Position = 0
      };
      var session = (Session)formatter.Deserialize(buffer);
      sessionManager.UpdateSession(session);
    }
  }
}
