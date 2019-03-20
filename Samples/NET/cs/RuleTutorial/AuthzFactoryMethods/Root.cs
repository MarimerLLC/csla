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

namespace AuthzFactoryMethods
{
  /// <summary>
  /// The root.
  /// </summary>
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Rules

    /// <summary>
    /// The add object authorization rules.
    /// </summary>
    private static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.CreateObject, "role1", "role2"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.GetObject, "role1", "role2"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.DeleteObject, "role1"));
      BusinessRules.AddRule(typeof (Root), new IsInRole(AuthorizationActions.EditObject, "role1", "role2"));
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
    /// The get editable root.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// </returns>
    public static Root GetEditableRoot(int id)
    {
      return DataPortal.Fetch<Root>(id);
    }

    /// <summary>
    /// The delete editable root.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    public static void DeleteEditableRoot(int id)
    {
      DataPortal.Delete<Root>(id);
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="Root"/> class from being created.
    /// </summary>
    public Root()
    { /* Require use of factory methods */ }

    #endregion
  }
}
