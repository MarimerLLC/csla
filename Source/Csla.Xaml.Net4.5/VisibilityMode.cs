//-----------------------------------------------------------------------
// <copyright file="VisibilityMode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Options controlling how the Authorizer</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Xaml
{
  /// <summary>
  /// Options controlling how the Authorizer
  /// control alters the visibility of a control
  /// when read access to the property is not allowed.
  /// </summary>
  public enum VisibilityMode
  {
    /// <summary>
    /// Specifies that the non-readable control
    /// should be collapsed.
    /// </summary>
    Collapsed,
    /// <summary>
    /// Specifies that the non-readable control
    /// should be hidden.
    /// </summary>
    Hidden,
    /// <summary>
    /// Specifies that the visibility of the 
    /// non-readable control should not be
    /// altered by the Authorizer control.
    /// </summary>
    Ignore
  }
}