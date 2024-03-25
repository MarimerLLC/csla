//-----------------------------------------------------------------------
// <copyright file="StateController.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gets and puts the current user session data</summary>
//-----------------------------------------------------------------------
using System.IO;
using Csla.Serialization.Mobile;
using Microsoft.AspNetCore.Mvc;
using Csla.State;
using Csla.Security;
using System;

namespace Csla.AspNetCore.Blazor.State
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class StateController(ApplicationContext applicationContext, ISessionManager sessionManager) : ControllerBase
  {
    private readonly ApplicationContext ApplicationContext = applicationContext;
    private readonly ISessionManager _sessionManager = sessionManager;

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
    /// <returns></returns>
    [HttpGet]
    public virtual StateResult Get(long lastTouched)
    {
      var result = new StateResult();
      var session = _sessionManager.GetSession();
      if (session.LastTouched == lastTouched)
      {
        result.ResultStatus = ResultStatuses.NoUpdates;
      }
      else
      {
        var message = (SessionMessage)ApplicationContext.CreateInstanceDI(typeof(SessionMessage));
        message.Session = session;
        if (FlowUserIdentityToWebAssembly)
        {
          var principal = new CslaClaimsPrincipal(ApplicationContext.Principal);
          message.Principal = principal;
        }
        var formatter = Csla.Serialization.SerializationFormatterFactory.GetFormatter(ApplicationContext);
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
    /// <returns></returns>
    [HttpPut]
    public virtual void Put(byte[] updatedSessionData)
    {
      var formatter = Csla.Serialization.SerializationFormatterFactory.GetFormatter(ApplicationContext);
      var buffer = new MemoryStream(updatedSessionData)
      {
        Position = 0
      };
      var session = (Session)formatter.Deserialize(buffer);
      _sessionManager.UpdateSession(session);
    }
  }

  /// <summary>
  /// Message type for communication between StateController
  /// and the Blazor wasm client.
  /// </summary>
  public class StateResult
  {
    /// <summary>
    /// Gets or sets the result status of the message.
    /// </summary>
    public ResultStatuses ResultStatus { get; set; }
    /// <summary>
    /// Gets or sets the serialized session data.
    /// </summary>
    public byte[] SessionData { get; set; }
  }

  /// <summary>
  /// Result status values for StateResult.
  /// </summary>
  public enum ResultStatuses
  {
    /// <summary>
    /// Success
    /// </summary>
    Success = 0,
    /// <summary>
    /// No updates
    /// </summary>
    NoUpdates = 1,
  }
}
