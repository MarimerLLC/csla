//-----------------------------------------------------------------------
// <copyright file="INotifyPropertyChanging.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines an object that raises the PropertyChanging</summary>
//-----------------------------------------------------------------------
#if PCL46 || PCL259
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