//-----------------------------------------------------------------------
// <copyright file="BusinessRuleAsync.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create business and validation</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Properties;

namespace Csla.Rules
{
  /// <summary>
  /// Base class used to create async 
  /// business and validation rules.
  /// </summary>
  public abstract class BusinessRuleAsync : BusinessRuleBase, IBusinessRuleAsync
  {
    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    public override bool IsAsync
    {
      get { return true; }
      protected set { }
    }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a business object as a whole.
    /// </summary>
    protected BusinessRuleAsync()
      : this(null)
    { }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a specfic property.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    protected BusinessRuleAsync(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected virtual Task ExecuteAsync(IRuleContext context)
    {
      return Task.CompletedTask;
    }

    Task IBusinessRuleAsync.ExecuteAsync(IRuleContext context)
    {
      PropertiesLocked = true;
      return ExecuteAsync(context);
    }
  }
}
