//-----------------------------------------------------------------------
// <copyright file="INotifyUnhandledAsyncException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implemented by an object that perfoms asynchronous</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Implemented by an object that perfoms asynchronous
  /// operations that may raise exceptions.
  /// </summary>
  public interface INotifyUnhandledAsyncException
  {
    /// <summary>
    /// Event indicating that an exception occurred during
    /// an asynchronous operation.
    /// </summary>
    event EventHandler<ErrorEventArgs> UnhandledAsyncException;
  }
}