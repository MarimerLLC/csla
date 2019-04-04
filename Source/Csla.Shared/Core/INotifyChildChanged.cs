//-----------------------------------------------------------------------
// <copyright file="INotifyChildChanged.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implemented by classes that notify when </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Implemented by classes that notify when 
  /// a child object has changed.
  /// </summary>
  public interface INotifyChildChanged
  {
    /// <summary>
    /// Event indictating that a child object
    /// has changed.
    /// </summary>
    event EventHandler<ChildChangedEventArgs> ChildChanged;
  }
}