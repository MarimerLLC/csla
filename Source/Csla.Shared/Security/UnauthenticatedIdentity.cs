//-----------------------------------------------------------------------
// <copyright file="UnauthenticatedIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a .NET identity object representing</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Implementation of a .NET identity object representing
  /// an unauthenticated user. Used by the
  /// UnauthenticatedPrincipal class.
  /// </summary>
  [Serializable()]
  public sealed class UnauthenticatedIdentity : CslaIdentityBase<UnauthenticatedIdentity>
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public UnauthenticatedIdentity()
    {
    }

    /// <summary>
    /// Initialize the object.
    /// </summary>
    protected override void Initialize()
    {
      base.Initialize ();
      IsAuthenticated = false;
      Name = string.Empty;
      AuthenticationType = string.Empty;
      Roles = new MobileList<string>();
    }
  }
}