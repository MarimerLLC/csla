//-----------------------------------------------------------------------
// <copyright file="NullAuthorizer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of the authorizer that</summary>
//-----------------------------------------------------------------------
namespace Csla.Server
{
  /// <summary>
  /// Implementation of the authorizer that
  /// allows all data portal calls to pass.
  /// </summary>
  public class NullAuthorizer : IAuthorizeDataPortal
  {
    /// <inheritdoc />
    public Task AuthorizeAsync(AuthorizeRequest clientRequest, CancellationToken ct)
    {
      return Task.CompletedTask;
    }
  }
}
