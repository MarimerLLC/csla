﻿//-----------------------------------------------------------------------
// <copyright file="BusinessRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create business and validation</summary>
//-----------------------------------------------------------------------
namespace Csla.Rules
{
  /// <summary>
  /// Base class used to create business and validation
  /// rules.
  /// </summary>
  public abstract class BusinessRule : BusinessRuleBase, IBusinessRule
  {
    private bool _isAsync;

    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    public override bool IsAsync
    {
      get { return _isAsync; }
      protected set
      {
        CanWriteProperty("IsAsync"); 
        _isAsync = value;
      }
    }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a business object as a whole.
    /// </summary>
    protected BusinessRule()
      : this(null)
    { }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a specfic property.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    protected BusinessRule(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected virtual void Execute(IRuleContext context)
    { }

    void IBusinessRule.Execute(IRuleContext context)
    {
      PropertiesLocked = true;
      Execute(context);
    }
  }
}