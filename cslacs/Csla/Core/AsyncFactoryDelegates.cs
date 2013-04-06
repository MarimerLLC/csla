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
  /// <param name="completed">
  /// Delegate pointer to callback method.
  /// </param>
  /// <param name="parameters">
  /// Parameters passed to the factory method.
  /// </param>
  public delegate void AsyncFactoryDelegate(Delegate completed, params object[] parameters);

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
  public delegate void AsyncFactoryDelegate<R, T>(EventHandler<DataPortalResult<R>> completed, T arg);
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
  /// <typeparam name="T1">Type of argument</typeparam>
  /// <param name="arg1">Argument to method.</param>
  /// <typeparam name="T2">Type of argument</typeparam>
  /// <param name="arg2">Argument to method.</param>
  public delegate void AsyncFactoryDelegate<R, T1, T2>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2);
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
  /// <typeparam name="T1">Type of argument</typeparam>
  /// <param name="arg1">Argument to method.</param>
  /// <typeparam name="T2">Type of argument</typeparam>
  /// <param name="arg2">Argument to method.</param>
  /// <typeparam name="T3">Type of argument</typeparam>
  /// <param name="arg3">Argument to method.</param>
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3);
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
  /// <typeparam name="T1">Type of argument</typeparam>
  /// <param name="arg1">Argument to method.</param>
  /// <typeparam name="T2">Type of argument</typeparam>
  /// <param name="arg2">Argument to method.</param>
  /// <typeparam name="T3">Type of argument</typeparam>
  /// <param name="arg3">Argument to method.</param>
  /// <typeparam name="T4">Type of argument</typeparam>
  /// <param name="arg4">Argument to method.</param>
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3, T4>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
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
  /// <typeparam name="T1">Type of argument</typeparam>
  /// <param name="arg1">Argument to method.</param>
  /// <typeparam name="T2">Type of argument</typeparam>
  /// <param name="arg2">Argument to method.</param>
  /// <typeparam name="T3">Type of argument</typeparam>
  /// <param name="arg3">Argument to method.</param>
  /// <typeparam name="T4">Type of argument</typeparam>
  /// <param name="arg4">Argument to method.</param>
  /// <typeparam name="T5">Type of argument</typeparam>
  /// <param name="arg5">Argument to method.</param>
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3, T4, T5>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}
