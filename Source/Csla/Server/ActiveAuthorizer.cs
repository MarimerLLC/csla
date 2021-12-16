//-----------------------------------------------------------------------
// <copyright file="ActiveAuthorizer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of the authorizer that</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Properties;
using Csla.Rules;
using Csla.Security;

namespace Csla.Server
{
  /// <summary>
  /// Implementation of the authorizer that
  /// checks per-type authorization rules
  /// for each request.
  /// </summary>
  public class ActiveAuthorizer : IAuthorizeDataPortal
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public ActiveAuthorizer(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    private ApplicationContext ApplicationContext { get; set; }

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
          if (!BusinessRules.HasPermission(ApplicationContext, clientRequest.Operation.ToAuthAction(), clientRequest.RequestObject))
          {
            throw new SecurityException(
               string.Format(Resources.UserNotAuthorizedException,
                   clientRequest.Operation.ToSecurityActionDescription(),
                   clientRequest.ObjectType.Name)
               );
          }
        }

        // Per-Type checks
        if (!BusinessRules.HasPermission(ApplicationContext, clientRequest.Operation.ToAuthAction(), clientRequest.ObjectType))
        {
          throw new SecurityException(
             string.Format(Resources.UserNotAuthorizedException,
                 clientRequest.Operation.ToSecurityActionDescription(),
                 clientRequest.ObjectType.Name)
             );
        }
      }
    }
  }
}
