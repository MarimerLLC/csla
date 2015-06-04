//-----------------------------------------------------------------------
// <copyright file="PropertyChangingEventHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines the method signature for the</summary>
//-----------------------------------------------------------------------
#if (ANDROID || IOS) || __ANDROID__ || IOS || SILVERLIGHT4 || NETFX_CORE
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the method signature for the
  /// PropertyChanging event handler.
  /// </summary>
  /// <param name="sender">Object raising the event.</param>
  /// <param name="e">EventArgs object.</param>
  public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}
#endif