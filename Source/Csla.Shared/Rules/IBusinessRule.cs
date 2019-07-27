//-----------------------------------------------------------------------
// <copyright file="IBusinessRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining a business/validation</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Csla.Rules
{
  /// <summary>
  /// Flags enum to define when rule is allowed or denied to run 
  /// </summary>
  [Flags]
  public enum RunModes
  {
    /// <summary>
    /// Default value, rule can run in any context
    /// </summary>
    Default=0,
    /// <summary>
    /// Deny rule from running in CheckRules 
    /// </summary>
    DenyCheckRules = 1,
    /// <summary>
    /// Deny rule from running as AffectedProperties from another rule.
    /// </summary>
    DenyAsAffectedProperty = 2,
    /// <summary>
    /// Deny rule from running on serverside portal
    /// </summary>
    DenyOnServerSidePortal = 4
  }

  /// <summary>
  /// Interface defining a business/validation
  /// rule implementation.
  /// </summary>
  public interface IBusinessRuleBase
  {
    /// <summary>
    /// Gets a list of secondary property values to be supplied to the
    /// rule when it is executed.
    /// </summary>
    List<Csla.Core.IPropertyInfo> InputProperties { get; }
    /// <summary>
    /// Gets a list of properties affected by this rule. Rules for these
    /// properties are executed after rules for the primary
    /// property.
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
    int Priority { get; }
    /// <summary>
    /// Gets a value indicating that the Target property should
    /// be set even for an async rule (note that using Target
    /// from a background thread will cause major problems).
    /// </summary>
    bool ProvideTargetWhenAsync { get; }
    /// <summary>
    /// Gets the context in which the rule is allowed to execute.
    /// </summary>
    /// <value>The run in context.</value>
    RunModes RunMode { get; }
    /// <summary>
    /// Gets a value indicating whether the Execute() method
    /// will run asynchronous code.
    /// </summary>
    bool IsAsync { get; }
  }

  /// <summary>
  /// Interface defining a business/validation
  /// rule implementation.
  /// </summary>
  public interface IBusinessRule : IBusinessRuleBase
  {
    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    void Execute(IRuleContext context);
  }

  /// <summary>
  /// Interface defining a business/validation
  /// rule implementation.
  /// </summary>
  public interface IBusinessRuleAsync : IBusinessRuleBase
  {
    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    Task ExecuteAsync(IRuleContext context);
  }
}