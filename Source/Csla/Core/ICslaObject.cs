//-----------------------------------------------------------------------
// <copyright file="ICslaObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base interface for all CSLA objects</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Core
{
  /// <summary>
  /// This is the base interface for all CSLA objects.
  /// It has no members and provides the minimal type constraint
  /// for objects that can flow through the data portal.
  /// </summary>
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
  public interface ICslaObject
  {
  }
}
