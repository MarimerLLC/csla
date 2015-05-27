//-----------------------------------------------------------------------
// <copyright file="INotifyPropertyChanging.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines an object that raises the PropertyChanging</summary>
//-----------------------------------------------------------------------
#if WINDOWS_PHONE || __ANDROID__ || IOS || NETFX_CORE
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines an object that raises the PropertyChanging
  /// event.
  /// </summary>
  public interface INotifyPropertyChanging
  {
    /// <summary>
    /// Event indicating that a property is changing.
    /// </summary>
    event PropertyChangingEventHandler PropertyChanging;
  }
}
#endif