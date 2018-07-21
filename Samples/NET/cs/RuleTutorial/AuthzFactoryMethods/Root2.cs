// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root2.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzFactoryMethods
{
  /// <summary>
  /// The root 2.
  /// </summary>
  [Serializable]
  public class Root2 : BusinessBase<Root2>
  {
    #region Business Rules

    /// <summary>
    /// The add object authorization rules.
    /// </summary>
    private static void AddObjectAuthorizationRules()
    {
     BusinessRules.AddRule(typeof(Root2), new IsInRole(AuthorizationActions.EditObject, "nobody"));
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// The new editable root.
    /// </summary>
    /// <returns>
    /// </returns>
    public static Root2 NewEditableRoot()
    {
      return DataPortal.Create<Root2>();
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="Root2"/> class from being created.
    /// </summary>
    public Root2()
    { /* Require use of factory methods */ }

    #endregion
  }
}
