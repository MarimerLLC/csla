//-----------------------------------------------------------------------
// <copyright file="ActiveAuthorizer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of an authorizer that rechecks rules on the server</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using Csla.Core;
using Csla.Properties;
using Csla.Rules;
using Csla.Security;

namespace Csla.Server
{
  /// <summary>
  /// Implementation of the authorizer that checks business rules
  /// (authorization and validation) for each server-side request.
  /// </summary>
  public class ActiveAuthorizer : IAuthorizeDataPortal
  {
    /// <summary>
    /// Checks authorization rules for the request.
    /// </summary>
    /// <param name="clientRequest">
    /// Client request information.
    /// </param>
    public void Authorize(AuthorizeRequest clientRequest)
    {
      if (ApplicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server &&
          ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Server)
      {
        if (clientRequest.Operation == DataPortalOperations.Update ||
            clientRequest.Operation == DataPortalOperations.Execute)
        {
          // Per-Instance checks
          if (!BusinessRules.HasPermission(clientRequest.Operation.ToAuthAction(), clientRequest.RequestObject))
          {
            throw new SecurityException(
               string.Format(Resources.UserNotAuthorizedException,
                   clientRequest.Operation.ToSecurityActionDescription(),
                   clientRequest.ObjectType.Name)
               );
          }
        }

        // Per-Type checks
        if (!BusinessRules.HasPermission(clientRequest.Operation.ToAuthAction(), clientRequest.ObjectType))
        {
          throw new SecurityException(
             string.Format(Resources.UserNotAuthorizedException,
                 clientRequest.Operation.ToSecurityActionDescription(),
                 clientRequest.ObjectType.Name)
             );
        }

        // Recheck the business rules have been fulfilled
        Revalidate(clientRequest);
      }
    }

    /// <summary>
    /// Perform revalidation of business rules on any supporting type
    /// </summary>
    /// <param name="clientRequest">The client request passed to the DataPortal as part of the operation</param>
    private void Revalidate(AuthorizeRequest clientRequest)
    {
      ITrackStatus checkableObject;

      checkableObject = clientRequest.RequestObject as ITrackStatus;
      if (checkableObject is null) return;

      RevalidateObject(checkableObject);
      if (!checkableObject.IsValid)
      {
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);
      }
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
      if (!(checkableObject is null))
      {
        checkableObject.CheckRules();
      }

      // Cascade revalidation to any children
      parent = parameter as IManageProperties;
      if (!(parent is null))
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