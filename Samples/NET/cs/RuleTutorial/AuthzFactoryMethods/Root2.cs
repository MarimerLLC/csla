using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzFactoryMethods
{
  [Serializable]
  public class Root2 : BusinessBase<Root2>
  {
    #region Business Rules

    private static void AddObjectAuthorizationRules()
    {
     BusinessRules.AddRule(typeof(Root2), new IsInRole(AuthorizationActions.EditObject, "nobody"));
    }

    #endregion

    #region Factory Methods

    public static Root2 NewEditableRoot()
    {
      return DataPortal.Create<Root2>();
    }


    private Root2()
    { /* Require use of factory methods */ }

    #endregion
  }
}
