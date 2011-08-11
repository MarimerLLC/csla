using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzFactoryMethods
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Rules

    private static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.CreateObject, "role1", "role2"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.GetObject, "role1", "role2"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.DeleteObject, "role1"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.EditObject, "role1", "role2"));
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    public static Root GetEditableRoot(int id)
    {
      return DataPortal.Fetch<Root>(id);
    }

    public static void DeleteEditableRoot(int id)
    {
      DataPortal.Delete<Root>(id);
    }

    private Root()
    { /* Require use of factory methods */ }

    #endregion
  }
}
