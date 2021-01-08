//-----------------------------------------------------------------------
// <copyright file="ICslaPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the base requirements for the interface of any</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;

namespace Csla.Security
{
  /// <summary>
  /// Defines the base requirements for the interface of any
  /// CSLA principal object.
  /// </summary>
  public interface ICslaPrincipal : IPrincipal, Csla.Serialization.Mobile.IMobileObject
  {
  }
}
