﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusinessRuleTest.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Helper class for creating unit tests on business rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System;
using System.Threading;
using Csla;
using Csla.Core;
using Csla.Reflection;
using Csla.Rules;
using Csla.Server;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;

namespace RuleTutorial.Testing.Common
{
  /// <summary>
  /// Base class for unit test of business rules
  /// </summary>
  public class BusinessRuleTest : Csla.Server.ObjectFactory
  {
    public BusinessRuleTest(ApplicationContext applicationContext)
      : base(applicationContext) { }

    protected RuleContext RuleContext { get; private set; }
    private EventWaitHandle _ruleContextCompleteWaitHandle;
    protected IBusinessRuleAsync AsyncRule { get; private set; }
    protected IBusinessRule Rule { get; private set; }
    protected ObjectAccessor Accessor;

    /// <summary>
    /// Initializes the test.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <param name="target">The target BO.</param>
    public void InitializeTest(IBusinessRuleAsync rule, object target)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      Accessor = new ObjectAccessor(applicationContext);

      _ruleContextCompleteWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      AsyncRule = rule;
      RuleContext = new RuleContext(applicationContext, 
        c => _ruleContextCompleteWaitHandle.Set(), rule, target, new Dictionary<IPropertyInfo, object>());
    }

    /// <summary>
    /// Initializes the test.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <param name="target">The target BO.</param>
    public void InitializeTest(IBusinessRule rule, object target)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      Accessor = new ObjectAccessor(applicationContext);

      _ruleContextCompleteWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      Rule = rule;
      RuleContext = new RuleContext(applicationContext,
        c => _ruleContextCompleteWaitHandle.Set(), rule, target, new Dictionary<IPropertyInfo, object>());
    }

    /// <summary>
    /// Executes the rule - will create Input Property Values from BO.
    /// Will wait for up to 30 seconds for Async rule to complete so test will look as if syncronous. 
    /// </summary>
    public async Task ExecuteRuleAsync()
    {
      if (!RuleContext.Rule.IsAsync)
        throw new InvalidOperationException("Rule is not async");
      if (RuleContext == null) throw new Exception("RuleContext is not set. Call InitializeTest before running ExecuteRule.");

      if (AsyncRule.InputProperties != null)
      {
        RuleContext.InputPropertyValues.Clear();
        foreach (var item in AsyncRule.InputProperties)
          RuleContext.InputPropertyValues.Add(item, Accessor.ReadProperty(RuleContext.Target, item));
      }

      await ExecuteRuleAsync(AsyncRule, RuleContext);
    }

    /// <summary>
    /// Executes the rule - will create Input Property Values from BO.
    /// Will wait for up to 30 seconds for Async rule to complete so test will look as if syncronous. 
    /// </summary>
    public void ExecuteRule()
    {
      if (RuleContext == null) throw new Exception("RuleContext is not set. Call InitializeTest before running ExecuteRule.");

      if (Rule.InputProperties != null)
      {
        RuleContext.InputPropertyValues.Clear();
        foreach (var item in Rule.InputProperties)
          RuleContext.InputPropertyValues.Add(item, Accessor.ReadProperty(RuleContext.Target,  item));
      }

      var t = ExecuteRule(RuleContext.Rule, RuleContext);
      if (RuleContext.Rule.IsAsync)
      {
        if (!_ruleContextCompleteWaitHandle.WaitOne(30000)) throw new Exception("Async BusinessRule timeout.");
      }
    }

    /// <summary>
    /// Executes the rule with supplied input properties.
    /// Will wait for up to 3 seconds for Async rule to complete so test will look as if syncronous. 
    /// </summary>
    /// <param name="inputPropertyValues">The input property values.</param>
    public void ExecuteRule(Dictionary<IPropertyInfo, object> inputPropertyValues)
    {
      if (RuleContext == null) throw new Exception("RuleContext is not set. Call InitializeTest before running ExecuteRule.");
      RuleContext.InputPropertyValues.Clear();
      foreach (var inputPropertyValue in inputPropertyValues)
      {
        RuleContext.InputPropertyValues.Add(inputPropertyValue.Key, inputPropertyValue.Value);
      }

      var t = ExecuteRule(RuleContext.Rule, RuleContext);
      if (RuleContext.Rule.IsAsync)
      {
        if (!_ruleContextCompleteWaitHandle.WaitOne(30000)) throw new Exception("Async BusinessRule timeout.");
      }
    }

    private async Task ExecuteRule(IBusinessRuleBase rule, RuleContext context)
    {
      if (rule is IBusinessRule sr)
        sr.Execute(context);
      else if (rule is IBusinessRuleAsync ar)
        await ar.ExecuteAsync(context);
    }

    private async Task ExecuteRuleAsync(IBusinessRuleAsync rule, RuleContext context)
    {
      await rule.ExecuteAsync(context);
    }

    /// <summary>
    /// Gets the output property value.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    public T GetOutputPropertyValue<T>(PropertyInfo<T> propertyInfo)
    {
      return (T)RuleContext.OutputPropertyValues[propertyInfo];
    }
  }

}
