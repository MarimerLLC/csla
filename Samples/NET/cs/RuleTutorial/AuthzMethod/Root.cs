// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzMethod
{
  /// <summary>
  /// The root.
  /// </summary>
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods

    // use snippet cslamet to add methods with authorization to BusinessBase or ReadOnlyBase objects
    /// <summary>
    /// The do calc method.
    /// </summary>
    public static MethodInfo DoCalcMethod = RegisterMethod(c => c.DoCalc());

    /// <summary>
    /// The do calc.
    /// </summary>
    public void DoCalc()
    {
      // Check authorization
      CanExecuteMethod(DoCalcMethod, true);

      // implementation of method goes here  
    }

    #endregion

    #region Business Rules

    /// <summary>
    /// The add business rules.
    /// </summary>
    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.ExecuteMethod, DoCalcMethod, "Role1", "Role2"));
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// The new editable root.
    /// </summary>
    /// <returns>
    /// </returns>
    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="Root"/> class from being created.
    /// </summary>
    public Root()
    { /* Require use of factory methods */ }

    #endregion
  }
}
