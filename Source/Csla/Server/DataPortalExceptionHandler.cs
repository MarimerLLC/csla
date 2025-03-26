﻿//-----------------------------------------------------------------------
// <copyright file="DataPortalExceptionHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This class provides a hook for developers to add custom error handling in the DataPortal. </summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Server
{

  /// <summary>
  /// This class provides a hook for developers to add custom error handling in the DataPortal. 
  /// 
  /// Typical scenario is to handle non-serializable exception and exceptions from assemblies that 
  /// does not exist on the client side, such as 3rd party database drivers, MQ drivers etc.
  /// </summary>
  public class DataPortalExceptionHandler
  {
    private readonly IDataPortalExceptionInspector _exceptionInspector;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="exceptionInspector"></param>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionInspector"/> is <see langword="null"/>.</exception>
    public DataPortalExceptionHandler(IDataPortalExceptionInspector exceptionInspector)
    {
      _exceptionInspector = exceptionInspector ?? throw new ArgumentNullException(nameof(exceptionInspector));
    }

    /// <summary>
    /// Transforms the exception in a Fetch, Create or Execute method.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="ex">The exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="ex"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Exception InspectException(Type objectType, object criteria, string methodName, Exception ex)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (string.IsNullOrWhiteSpace(methodName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(methodName)), nameof(methodName));
      if (ex is null)
        throw new ArgumentNullException(nameof(ex));
      if (ex is CallMethodException)
      {
        if (CallExceptionInspector(objectType, null, criteria, methodName, ex.InnerException!, out var handledException))
        {
          return new CallMethodException(methodName + " " + Resources.MethodCallFailed, handledException!);
        }
      }
      else
      {
        if (CallExceptionInspector(objectType, null, criteria, methodName, ex, out var handledException))
        {
          return handledException!;
        }
      }

      return ex;
    }

    /// <summary>
    /// Transforms the exception in a Update, Delete method
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="businessObject">The business object if available.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="ex">The exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="ex"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Exception InspectException(Type objectType, object? businessObject, object? criteria, string methodName, Exception ex)
    {
      // The exception as parameter is always a CallMethodException containing the business exception as Inner exception 
      if (ex is CallMethodException)
      {
        if (CallExceptionInspector(objectType, businessObject, criteria, methodName, ex.InnerException!, out var handledException))
        {
          // developer should only transform and if rethrows a new we will wrap as new CallMethodException
          return new CallMethodException(methodName + " " + Resources.MethodCallFailed, handledException!);
        }
      }
      else
      {
        if (CallExceptionInspector(objectType, null, criteria, methodName, ex, out var handledException))
        {
          return handledException!;
        }
      }
      return ex;
    }

    /// <summary>
    /// Calls the custom exception inspector.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="businessObject">The business object.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="exception">The exception that was throw in business method.</param>
    /// <param name="handledException">The handled exception.</param>
    /// <returns>
    /// true if new exception was thrown else false
    /// </returns>
    private bool CallExceptionInspector(Type objectType, object? businessObject, object? criteria, string methodName, Exception exception, [NotNullWhen(true)] out Exception? handledException
    )
    {
      handledException = null;
      try
      {
        // This method should rethrow a new exception to be handled 
        _exceptionInspector?.InspectException(objectType, businessObject, criteria, methodName, exception);
      }
      catch (Exception ex)
      {
        handledException = ex;
        return true;  // new exception returned
      }
      return false;  // exception was not handled 
    }
  }
}