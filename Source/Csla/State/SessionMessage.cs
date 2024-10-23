//-----------------------------------------------------------------------
// <copyright file="SessionMessage.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Serialization.Mobile;

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
    get => ReadProperty(SessionProperty)!;
    set => LoadProperty(SessionProperty, value ?? throw new ArgumentNullException(nameof(Session)));
  }

  /// <summary>
  /// User principal data
  /// </summary>
  public static readonly PropertyInfo<ClaimsPrincipal?> PrincipalProperty = RegisterProperty<ClaimsPrincipal?>(nameof(Principal));
  /// <summary>
  /// User principal data
  /// </summary>
  public ClaimsPrincipal? Principal
  {
    get => ReadProperty(PrincipalProperty);
    set => LoadProperty(PrincipalProperty, value);
  }

  /// <summary>
  /// Initializes a new instance of <see cref="SessionMessage"/>.
  /// </summary>
  /// <param name="session"></param>
  /// <exception cref="ArgumentNullException"><paramref name="session"/> is <see langword="null"/>.</exception>
  public SessionMessage(Session session)
  {
    Session = session ?? throw new ArgumentNullException(nameof(session));
  }

  /// <summary>
  /// Initializes a new instance of <see cref="SessionMessage"/>.
  /// </summary>
  [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
  public SessionMessage()
  {
  }
}