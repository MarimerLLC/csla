//-----------------------------------------------------------------------
// <copyright file="ActiveAuthorizer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of the authorizer that</summary>
//-----------------------------------------------------------------------
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
      if (clientRequest.Operation == DataPortalOperations.Create)
      {
        if (!Rules.BusinessRules.HasPermission(
          ApplicationContext, Rules.AuthorizationActions.CreateObject, clientRequest.ObjectType))
          throw new SecurityException($"{clientRequest.ObjectType.Name}.{clientRequest.Operation}");
        return;
      }
      if (clientRequest.Operation == DataPortalOperations.Fetch)
      {
        if (!Rules.BusinessRules.HasPermission(
          ApplicationContext, Rules.AuthorizationActions.GetObject, clientRequest.ObjectType))
          throw new SecurityException($"{clientRequest.ObjectType.Name}.{clientRequest.Operation}");
        return;
      }
      if (clientRequest.Operation == DataPortalOperations.Update)
      {
        if (!Rules.BusinessRules.HasPermission(
          ApplicationContext, Rules.AuthorizationActions.EditObject, clientRequest.ObjectType))
          throw new SecurityException($"{clientRequest.ObjectType.Name}.{clientRequest.Operation}");
        return;
      }
      if (clientRequest.Operation == DataPortalOperations.Delete)
      {
        if (!Rules.BusinessRules.HasPermission(
          ApplicationContext, Rules.AuthorizationActions.DeleteObject, clientRequest.ObjectType))
          throw new SecurityException($"{clientRequest.ObjectType.Name}.{clientRequest.Operation}");
        return;
      }
      if (clientRequest.Operation == DataPortalOperations.Execute)
      {
        if (!Rules.BusinessRules.HasPermission(
          ApplicationContext, Rules.AuthorizationActions.EditObject, clientRequest.ObjectType))
          throw new SecurityException($"{clientRequest.ObjectType.Name}.{clientRequest.Operation}");
        return;
      }
    }
  }
}
