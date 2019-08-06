//-----------------------------------------------------------------------
// <copyright file="ItemWithAsynchRule.cs" company="Marimer LLC">
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
using Csla.Serialization;

namespace Csla.Testing.Business.BusyStatus
{
  [Serializable]
  public class ItemWithAsynchRule : BusinessBase<ItemWithAsynchRule>
  {
    public ItemWithAsynchRule() { }

    private static PropertyInfo<string> IdProperty = RegisterProperty<string>(c=>c.Id);

    public string Id
    {
      get
      {
        return GetProperty(IdProperty);
      }
      set
      {
        SetProperty(IdProperty, value);
      }
    }

    private static PropertyInfo<string> OperationResultProperty = RegisterProperty<string>(c=>c.OperationResult, "Operation Result", string.Empty);

    public string OperationResult
    {
      get
      {
        return GetProperty(OperationResultProperty);
      }
      set
      {
        SetProperty(OperationResultProperty, value);
      }
    }

    private static PropertyInfo<string> RuleFieldProperty = RegisterProperty<string>(c=>c.RuleField, "Rule Field", string.Empty);

    public string RuleField
    {
      get
      {
        return GetProperty(RuleFieldProperty);
      }
      set
      {
        SetProperty(RuleFieldProperty, value);
      }
    }

    internal static ItemWithAsynchRule GetOneItemForList(string id)
    {
      ItemWithAsynchRule returnValue = new ItemWithAsynchRule();
      returnValue.Id = id;
      returnValue.MarkAsChild();
      returnValue.MarkOld();
      return returnValue;
    }

    public bool IsRunningRules
    {
      get { return BusinessRules.RunningAsyncRules && BusinessRules.RunningRules; }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new FiveSecondsLongRule(RuleFieldProperty));
    }

    public class FiveSecondsLongRule : Rules.BusinessRule
    {
      public FiveSecondsLongRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(IRuleContext context)
      {
        if (Csla.ApplicationContext.LogicalExecutionLocation == Csla.ApplicationContext.LogicalExecutionLocations.Client)
        {
          BackgroundWorker worker = new BackgroundWorker();

          worker.DoWork += (s, e) =>
          {
            System.Threading.Thread.Sleep(2000);
            var value = context.InputPropertyValues[PrimaryProperty];
            if (value == null || value.ToString().ToUpper() == "ERROR")
              context.AddErrorResult("error detected");
          };

          worker.RunWorkerCompleted += (s, e) => context.Complete();

          // simulating an asynchronous process.
          worker.RunWorkerAsync();
        }
        else
        {
          var value = context.InputPropertyValues[PrimaryProperty];
          if (value == null || value.ToString().ToUpper() == "ERROR")
            context.AddErrorResult("error detected");
          context.Complete();
        }
      }
    }

    internal void DataPortal_Fetch(string criteria)
    {
      this.Id = "fetch_" + criteria;
      this.OperationResult = "DataPortal_Fetch/with parameters";
      this.MarkOld();
    }

    protected override void DataPortal_Create()
    {
      this.Id = "random_create";
      this.OperationResult = "DataPortal_Create/no parameters";
      this.MarkNew();
    }

    protected override void DataPortal_Insert()
    {
      this.Id = "random_insert";
      this.OperationResult = "DataPortal_Insert";
      this.MarkOld();
    }

    protected override void DataPortal_Update()
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
    }

    internal void Update()
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
    }
  }
}