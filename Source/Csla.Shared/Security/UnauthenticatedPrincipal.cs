//-----------------------------------------------------------------------
// <copyright file="UnauthenticatedPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implementation of a .NET principal object that represents</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
namespace Csla.Security
{
  /// <summary>
  /// Implementation of a .NET principal object that represents
  /// an unauthenticated user. Contains an UnauthenticatedIdentity
  /// object.
  /// </summary>
  [Serializable]
  public sealed class UnauthenticatedPrincipal : CslaPrincipal
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public UnauthenticatedPrincipal() : base(new UnauthenticatedIdentity()) { }

    /// <summary>
    /// Returns a value indicating whether the user is in the
    /// specified role.
    /// </summary>
    /// <param name="role">Role name.</param>
    /// <returns></returns>
    public override bool IsInRole(string role)
    {
      return base.IsInRole(role);
    }
  }
}