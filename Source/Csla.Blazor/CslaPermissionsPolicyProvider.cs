//-----------------------------------------------------------------------
// <copyright file="CslaPermissionsPolicyProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CSLA permissions policy provider</summary>
//-----------------------------------------------------------------------
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Csla.Blazor
{
  /// <summary>
  /// CSLA permissions policy provider.
  /// </summary>
  public class CslaPermissionsPolicyProvider : IAuthorizationPolicyProvider
  {
    /// <summary>
    /// Gets the fallback policy provider
    /// </summary>
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="options">Authorization options</param>
    public CslaPermissionsPolicyProvider(IOptions<AuthorizationOptions> options)
    {
      FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// Gets the default policy
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    /// <summary>
    /// Gets the fallback policy
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

    /// <summary>
    /// Gets the authorization policy
    /// </summary>
    /// <param name="policyName">String representing the policy</param>
    /// <returns></returns>
    /// <remarks>
    /// Gets a CSLA permissions policy if the policy name corresponds
    /// to a CSLA policy, otherwise gets a policy from the fallback
    /// provider.
    /// </remarks>
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
      if (CslaPolicy.TryGetPermissionRequirement(policyName, out CslaPermissionRequirement requirement))
      {
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(requirement);
        return Task.FromResult(policy.Build());
      }
      else
      {
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
      }
    }
  }
}
