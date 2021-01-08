//-----------------------------------------------------------------------
// <copyright file="ICslaIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a base class to simplify creation of</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base type to simplify creation of
  /// a .NET identity object for use with ICslaPrincipal.
  /// </summary>
  public interface ICslaIdentity
    : IReadOnlyBase, IIdentity, ICheckRoles 
  {
    /// <summary>
    /// Gets the list of roles for this user.
    /// </summary>
    MobileList<string> Roles { get; }
  }
}
