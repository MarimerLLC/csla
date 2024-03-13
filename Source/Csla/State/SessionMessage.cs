//-----------------------------------------------------------------------
// <copyright file="SessionMessage.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Security;

namespace Csla.State;

/// <summary>
/// Session controller message
/// </summary>
[Serializable]
public class SessionMessage : CommandBase<SessionMessage>
{
  /// <summary>
  /// Session data
  /// </summary>
  public static readonly PropertyInfo<Session> SessionProperty = RegisterProperty<Session>(nameof(Session));
  /// <summary>
  /// Session data
  /// </summary>
  public Session Session
  {
    get => ReadProperty(SessionProperty);
    set => LoadProperty(SessionProperty, value);
  }

  /// <summary>
  /// User principal data
  /// </summary>
  public static readonly PropertyInfo<CslaClaimsPrincipal> PrincipalProperty = RegisterProperty<CslaClaimsPrincipal>(nameof(Principal));
  /// <summary>
  /// User principal data
  /// </summary>
  public CslaClaimsPrincipal Principal
  {
    get => ReadProperty(PrincipalProperty);
    set => LoadProperty(PrincipalProperty, value);
  }
}
