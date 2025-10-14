//-----------------------------------------------------------------------
// <copyright file="PermissionRootWithCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Test.Authorization 
{
  [Serializable]
  public class PermissionRootWithCriteria : BusinessBase<PermissionRootWithCriteria>
  {
    public static void AddObjectAuthorizationRules() {
      BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.CreateObject));
      BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.GetObject));
      BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.DeleteObject));
    }

    public class Criteria;
  }
}
