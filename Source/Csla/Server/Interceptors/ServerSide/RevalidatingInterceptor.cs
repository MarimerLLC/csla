//-----------------------------------------------------------------------
// <copyright file="RevalidatingInterceptor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Initiates revalidation on business objects during data portal operations</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Csla.Core;
using Csla.Properties;

namespace Csla.Server.Interceptors.ServerSide
{
  /// <summary>
  /// DataPortal interceptor to perform revalidation on business objects
  /// </summary>
  public class RevalidatingInterceptor : IInterceptDataPortal
  {
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Public constructor, intended to be executed by DI
    /// </summary>
    /// <param name="applicationContext"></param>
    public RevalidatingInterceptor(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Interception handler run before a DataPortal operation
    /// </summary>
    /// <param name="e">The interception arguments from the DataPortal</param>
    /// <exception cref="Rules.ValidationException"></exception>
    public void Initialize(InterceptArgs e)
    {
      ITrackStatus checkableObject;

      if (_applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server 
        || _applicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server)
      {
        return;
      }

      checkableObject = e.Parameter as ITrackStatus;
      if (checkableObject is null) return;

      RevalidateObject(checkableObject);
      if (!checkableObject.IsValid)
      {
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);
      }
    }

    /// <summary>
    /// Interception handler run after a DataPortal operation
    /// </summary>
    /// <param name="e">The interception arguments from the DataPortal</param>
    public void Complete(InterceptArgs e)
    {
    }

    /// <summary>
    /// Perform revalidation of business rules on any supporting type
    /// </summary>
    /// <param name="parameter">The parameter that was passed to the DataPortal as part of the operation</param>
    private void RevalidateObject(object parameter)
    {
      ICheckRules checkableObject;
      IManageProperties parent;

      // Initiate re-execution of rules on any supporting business object
      checkableObject = parameter as ICheckRules;
      if (checkableObject is not null)
      {
        checkableObject.CheckRules();
      }

      // Cascade revalidation to any children
      parent = parameter as IManageProperties;
      if (parent is not null)
      {
        foreach (object child in parent.GetChildren())
        {
          RevalidateChild(child);
        }
      }
    }

    /// <summary>
    /// Initiate revalidation on a child, handling collections appropriately
    /// </summary>
    /// <param name="child">The child object on which to attempt to trigger revalidation</param>
    private void RevalidateChild(object child)
    {
      // Handle the child being a collection
      if (child is IEnumerable childCollection)
      {
        foreach (object childItem in childCollection)
        {
          RevalidateObject(childItem);
        }
        return;
      }

      // Handle the child being an individual object
      RevalidateObject(child);
    }

  }
}
