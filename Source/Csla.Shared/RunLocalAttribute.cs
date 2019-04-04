//-----------------------------------------------------------------------
// <copyright file="RunLocalAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Marks a DataPortal_XYZ method to</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Marks a DataPortal_XYZ method to
  /// be run on the client even if the server-side
  /// DataPortal is configured for remote use.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class RunLocalAttribute : Attribute
  {

  }
}