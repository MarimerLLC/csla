//-----------------------------------------------------------------------
// <copyright file="AuthRuleExpectsCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Linq;
using Csla.Rules;

namespace Csla.Test.Authorization 
{
  public class AuthRuleExpectsCriteria : AuthorizationRule 
  {
    public AuthRuleExpectsCriteria(AuthorizationActions action) : base(action) 
    {
    }
    
    protected override void Execute(IAuthorizationContext context) 
    {
      context.HasPermission = context.Criteria?.FirstOrDefault() is PermissionRootWithCriteria.Criteria;
    }
  }
}
