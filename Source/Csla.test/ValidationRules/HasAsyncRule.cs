//-----------------------------------------------------------------------
// <copyright file="HasAsyncRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;
using System.ComponentModel;
using System.Threading;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  public partial class HasAsyncRule : BusinessBase<HasAsyncRule>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rule1(NameProperty));
      BusinessRules.AddRule(new Rule2(NameProperty));
      BusinessRules.AddRule(new Rule3(NameProperty));
    }

    public class Rule1 : BusinessRule
    {
      public Rule1(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(IRuleContext context)
      {
        if (context.Target != null)
          throw new ArgumentOutOfRangeException("context.Target must be null");

        BackgroundWorker worker = new BackgroundWorker();
        // Using closures to access the context would be easier but this is not possible
        // in all languages. Below is an example of how to use the context without closures

        worker.DoWork += (s, e) =>
        {
          var avrc = (RuleContext)e.Argument;
          e.Result = avrc;
          System.Threading.Thread.Sleep(50);
          var name = avrc.InputPropertyValues[NameProperty];
          if (name != null && name.ToString() == "error")
            avrc.AddErrorResult("error detected");
        };
        worker.RunWorkerCompleted += (s, e) =>
        {
          var avrc = (RuleContext)e.Result;
          if (e.Error != null)
            avrc.AddErrorResult(e.Error.Message);
          avrc.Complete();
        };

        // simulating an asynchronous process.
        worker.RunWorkerAsync(context);
      }
    }

    public class Rule2 : BusinessRule
    {
      private Rule1 _innerRule;

      public Rule2(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        _innerRule = new Rule1(primaryProperty);
        InputProperties = _innerRule.InputProperties;
      }

      protected override void Execute(IRuleContext context)
      {
        if (context.Target != null)
          throw new ArgumentOutOfRangeException("context.Target must be null");

        ((IBusinessRule)_innerRule).Execute(context.GetChainedContext(_innerRule));
      }
    }

    public class Rule3 : BusinessRule
    {
      private Rule1 _innerRule;

      public Rule3(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        _innerRule = new Rule1(primaryProperty);
        InputProperties = _innerRule.InputProperties;
        ProvideTargetWhenAsync = true;
      }

      protected override void Execute(IRuleContext context)
      {
        if (context.Target == null)
          throw new ArgumentOutOfRangeException("context.Target must not be null");

        ((IBusinessRule)_innerRule).Execute(context.GetChainedContext(_innerRule));
      }
    }
  }
}