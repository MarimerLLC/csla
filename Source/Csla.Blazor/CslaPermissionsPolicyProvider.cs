//-----------------------------------------------------------------------
// <copyright file="CslaPermissionsPolicyProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CSLA permissions policy provider</summary>
//-----------------------------------------------------------------------

using System;
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
    private readonly AuthorizationOptions _options;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="options">Authorization options</param>
    public CslaPermissionsPolicyProvider(IOptions<AuthorizationOptions> options)
    {
      if (options == null)
      {
        throw new ArgumentNullException(nameof(options));
      }
      
      _options = options.Value;
    }

    /// <summary>
    /// Gets the default policy
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(_options.DefaultPolicy);
    /// <summary>
    /// Gets the fallback policy
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Task.FromResult(_options.FallbackPolicy);

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
      var policy = _options.GetPolicy(policyName);
      if (policy is null && CslaPolicy.TryGetPermissionRequirement(policyName, out CslaPermissionRequirement requirement))
      {
        var policyBuilder = new AuthorizationPolicyBuilder();
        policyBuilder.AddRequirements(requirement);
        policy = policyBuilder.Build();

        _options.AddPolicy(policyName, policy);
      }

      return Task.FromResult(policy);
    }
  }
}
