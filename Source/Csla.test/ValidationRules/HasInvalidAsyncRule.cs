//-----------------------------------------------------------------------
// <copyright file="HasInvalidAsyncRule.cs" company="Marimer LLC">
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

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public partial class HasInvalidAsyncRule : BusinessBase<HasInvalidAsyncRule>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new InvalidAsyncValidationRule(NameProperty));
      base.AddBusinessRules();
    }

    public new Rules.BrokenRulesCollection GetBrokenRules()
    {
      return BusinessRules.GetBrokenRules();
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    public class InvalidAsyncValidationRule : BusinessRule
    {
      public InvalidAsyncValidationRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(IRuleContext context)
      {
        var bw = new System.ComponentModel.BackgroundWorker();
        bw.DoWork += (o, e) =>
          {
            throw new InvalidOperationException();
          };
        bw.RunWorkerCompleted += (o, e) =>
          {
            if (e.Error != null)
              context.AddErrorResult(e.Error.Message);
            context.Complete();
          };
        bw.RunWorkerAsync();
      }
    }
  }
}