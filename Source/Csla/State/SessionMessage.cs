//-----------------------------------------------------------------------
// <copyright file="SessionMessage.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla.State;

/// <summary>
/// Session controller message
/// </summary>
[Serializable]
public class SessionMessage : MobileObject
{

  /// <summary>
  /// Session data
  /// </summary>
  public Session Session { get; private set; } = default!;

  /// <summary>
  /// User principal data
  /// </summary>
  public ClaimsPrincipal? Principal
  {
    get;
    set;
  }

  /// <summary>
  /// Initializes a new instance of <see cref="SessionMessage"/>.
  /// </summary>
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

  /// <inheritdoc />
  protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
  {
    info.AddChild(nameof(Session), formatter.SerializeObject(Session).ReferenceId);
    if (Principal is not null)
    {
      info.AddChild(nameof(Principal), formatter.SerializeObject(Principal).ReferenceId);
    }

    base.OnGetChildren(info, formatter);
  }

  /// <inheritdoc />
  protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
  {
    Session = (Session)formatter.GetObject(info.Children[nameof(Session)].ReferenceId)!;
    if (info.Children.TryGetValue(nameof(Principal), out var principalChildData))
    {
      Principal = (ClaimsPrincipal?)formatter.GetObject(principalChildData.ReferenceId);
    }
    base.OnSetChildren(info, formatter);
  }
}