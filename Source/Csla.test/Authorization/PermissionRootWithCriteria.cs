using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;

namespace Csla.Test.Authorization 
{
  [Serializable()]
  public class PermissionRootWithCriteria : BusinessBase<PermissionRootWithCriteria>
  {
    public static void AddObjectAuthorizationRules() {
      Csla.Rules.BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.CreateObject));
      Csla.Rules.BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.GetObject));
      Csla.Rules.BusinessRules.AddRule(
        typeof(PermissionRootWithCriteria),
        new AuthRuleExpectsCriteria(AuthorizationActions.DeleteObject));
    }

    public class Criteria { }
  }
}
