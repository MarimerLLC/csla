//-----------------------------------------------------------------------
// <copyright file="NoAccessBehavior.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options available for handling no</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Security
{
  /// <summary>
  /// Options available for handling no
  /// access to a property due to
  /// authorization rules.
  /// </summary>
  public enum NoAccessBehavior
  {
    /// <summary>
    /// Suppress exceptions.
    /// </summary>
    SuppressException,
    /// <summary>
    /// Throw security exception.
    /// </summary>
    ThrowException
  }
}