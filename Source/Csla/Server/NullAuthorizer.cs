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
    /// <summary>
    /// Checks authorization rules for the request.
    /// </summary>
    /// <param name="clientRequest">
    /// Client request information.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task AuthorizeAsync(AuthorizeRequest clientRequest, CancellationToken ct)
    {
      return Task.CompletedTask;
    }
  }
}
