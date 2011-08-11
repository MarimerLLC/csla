using System;
using System.Security;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzMethod
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    // use snippet cslamet to add methods with authorization to BusinessBase or ReadOnlyBase objects
    public static MethodInfo DoCalcMethod = RegisterMethod(c => c.DoCalc());
    public void DoCalc()
    {
      // Check authorization
      CanExecuteMethod(DoCalcMethod, true);

      // implementation of method goes here  
    }

    #region Business Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.ExecuteMethod, DoCalcMethod, "Role1", "Role2"));
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    private Root()
    { /* Require use of factory methods */ }

    #endregion
  }
}
