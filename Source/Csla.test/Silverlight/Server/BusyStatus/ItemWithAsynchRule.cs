﻿//-----------------------------------------------------------------------
// <copyright file="ItemWithAsynchRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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

    private static PropertyInfo<string> IdProperty = RegisterProperty<string>(new PropertyInfo<string>("Id", "Id", string.Empty));

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

    private static PropertyInfo<string> OperationResultProperty = RegisterProperty<string>(new PropertyInfo<string>("OperationResult", "Operation Result", string.Empty));

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

    private static PropertyInfo<string> RuleFieldProperty = RegisterProperty<string>(new PropertyInfo<string>("RuleField", "Rule Field",string.Empty));

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

    public static void GetItemWithAsynchRule(string id, EventHandler<DataPortalResult<ItemWithAsynchRule>> completed)
    {
      DataPortal<ItemWithAsynchRule> dp = new DataPortal<ItemWithAsynchRule>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new SingleCriteria<ItemWithAsynchRule, string>(id));
    }

    public static void NewItemWithAsynchRule(EventHandler<DataPortalResult<ItemWithAsynchRule>> completed)
    {
      DataPortal<ItemWithAsynchRule> dp = new DataPortal<ItemWithAsynchRule>();
      dp.CreateCompleted += completed;
      dp.BeginCreate();
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

      protected override void Execute(RuleContext context)
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
    }

#if SILVERLIGHT

    public void DataPortal_Fetch(SingleCriteria<ItemWithAsynchRule, string> criteria)
    {
      this.Id = "fetch_" + criteria.Value;
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

    public void DoDataPortal_Update()
    {
      DataPortal_Update();
    }
#else
    internal void DataPortal_Fetch(SingleCriteria<ItemWithAsynchRule, string> criteria)
    {
      this.Id = "fetch_" + criteria.Value;
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
#endif

  }
}