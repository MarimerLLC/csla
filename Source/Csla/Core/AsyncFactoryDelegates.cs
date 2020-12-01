//-----------------------------------------------------------------------
// <copyright file="AsyncFactoryDelegates.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Delegate for an asynchronous business object </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Delegate for an asynchronous business object 
  /// factory method with n parameters.
  /// </summary>
  /// <typeparam name="R">
  /// Type of business object to be created.
  /// </typeparam>
  /// <param name="completed">
  /// Delegate pointer to callback method.
  /// </param>
  public delegate void AsyncFactoryDelegate<R>(EventHandler<DataPortalResult<R>> completed);
  /// <summary>
  /// Delegate for an asynchronous business object 
  /// factory method with n parameters.
  /// </summary>
  /// <typeparam name="R">
  /// Type of business object to be created.
  /// </typeparam>
  /// <typeparam name="T">Type of argument</typeparam>
  /// <param name="completed">
  /// Delegate pointer to callback method.
  /// </param>
  /// <param name="arg">Argument to method.</param>
  public delegate void AsyncFactoryDelegate<T, R>(T arg, EventHandler<DataPortalResult<R>> completed);
}