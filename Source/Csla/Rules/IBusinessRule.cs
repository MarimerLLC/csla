//-----------------------------------------------------------------------
// <copyright file="IBusinessRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Interface defining a business/validation</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace Csla.Rules
{
  /// <summary>
  /// Interface defining a business/validation
  /// rule implementation.
  /// </summary>
  public interface IBusinessRule
  {
    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    void Execute(RuleContext context);
    /// <summary>
    /// Gets a list of secondary property values to be supplied to the
    /// rule when it is executed.
    /// </summary>
    List<Csla.Core.IPropertyInfo> InputProperties { get; }
    /// <summary>
    /// Gets a list of properties affected by this rule.
    /// </summary>
    List<Csla.Core.IPropertyInfo> AffectedProperties { get; }
    /// <summary>
    /// Gets the primary property affected by this rule.
    /// </summary>
    Csla.Core.IPropertyInfo PrimaryProperty { get; }
    /// <summary>
    /// Gets a unique rule:// URI for the specific instance
    /// of the rule within the context of the business object
    /// where the rule is used.
    /// </summary>
    string RuleName { get; }
    /// <summary>
    /// Gets the rule priority.
    /// </summary>
    int Priority { get;  }
    /// <summary>
    /// Gets a value indicating whether the Execute() method
    /// will run asynchronous code.
    /// </summary>
    bool IsAsync { get; }
  }
}