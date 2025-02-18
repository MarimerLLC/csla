//-----------------------------------------------------------------------
// <copyright file="ActiveAuthorizer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of the authorizer that</summary>
//-----------------------------------------------------------------------

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
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ActiveAuthorizer(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Checks authorization rules for the request.
    /// </summary>
    /// <param name="clientRequest">
    /// Client request information.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    public async Task AuthorizeAsync(AuthorizeRequest clientRequest, CancellationToken ct)
    {
      if (_applicationContext.LogicalExecutionLocation == ApplicationContext.LogicalExecutionLocations.Server &&
      _applicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Server)
      {
        if (clientRequest.Operation == DataPortalOperations.Update ||
            clientRequest.Operation == DataPortalOperations.Execute)
        {
          if (clientRequest.RequestObject is null)
            throw new InvalidOperationException(string.Format(Resources.NoInstanceProvidedForAuthorizationCheck, clientRequest.Operation.ToSecurityActionDescription(), clientRequest.ObjectType.Name));

          // Per-Instance checks
          if (!await BusinessRules.HasPermissionAsync(_applicationContext, clientRequest.Operation.ToAuthAction(), clientRequest.RequestObject, ct))
          {
            throw new SecurityException(
               string.Format(Resources.UserNotAuthorizedException,
                   clientRequest.Operation.ToSecurityActionDescription(),
                   clientRequest.ObjectType.Name)
               );
          }
        }

        // Per-Type checks
        if (!await BusinessRules.HasPermissionAsync(_applicationContext, clientRequest.Operation.ToAuthAction(), clientRequest.ObjectType, ct))
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
