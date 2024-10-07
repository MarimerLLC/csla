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
using Csla.Serialization;
using System.Runtime.Serialization;
using Csla.Properties;

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
          message.Principal = applicationContext.Principal;
        }
        var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
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
      var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
      var buffer = new MemoryStream(updatedSessionData)
      {
        Position = 0
      };
      if (formatter.Deserialize(buffer) is not Session session)
      {
        throw new SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, nameof(Session)));
      }

      sessionManager.UpdateSession(session);
    }
  }
}
