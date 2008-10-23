using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Validation;
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

    protected override void AddBusinessRules()
    {
      AsyncRuleArgs args = new AsyncRuleArgs(RuleFieldProperty);
      ValidationRules.AddRule(FiveSecondsLongRule, args);
    }

    private static void FiveSecondsLongRule(AsyncValidationRuleContext context)
    {
      BackgroundWorker worker = new BackgroundWorker();

      worker.DoWork += (s, e) =>
      {
        AsyncValidationRuleContext avrc = (AsyncValidationRuleContext)e.Argument;
        System.Threading.Thread.Sleep(2000);
        avrc.OutArgs.Result = ((string)avrc.PropertyValues["RuleField"]).ToUpper() != "ERROR";
        if (avrc.OutArgs.Result == false)
          avrc.OutArgs.Description = "error detected";

        e.Result = avrc;
      };

      worker.RunWorkerCompleted += (s, e) =>
      {
        AsyncValidationRuleContext avrc = (AsyncValidationRuleContext)e.Result;
        avrc.Complete();
      };

      // simulating an asynchronous process.
      worker.RunWorkerAsync(context);
    }

#if SILVERLIGHT

    public void DataPortal_Fetch(SingleCriteria<ItemWithAsynchRule, string> criteria, Csla.DataPortalClient.LocalProxy<ItemWithAsynchRule>.CompletedHandler completed)
    {
      this.Id = "fetch_" + criteria.Value;
      this.OperationResult = "DataPortal_Fetch/with parameters";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    public void DataPortal_Create(Csla.DataPortalClient.LocalProxy<ItemWithAsynchRule>.CompletedHandler completed)
    {
      this.Id = "random_create";
      this.OperationResult = "DataPortal_Create/no parameters";
      this.MarkNew();
      completed.Invoke(this, null);
    }

    public void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<ItemWithAsynchRule>.CompletedHandler completed)
    {
      this.Id = "random_insert";
      this.OperationResult = "DataPortal_Insert";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    public void DataPortal_Update(Csla.DataPortalClient.LocalProxy<ItemWithAsynchRule>.CompletedHandler completed)
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
      completed.Invoke(this, null);
    }

    internal void DataPortal_Update()
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
    }

#else
    internal void DataPortal_Fetch(SingleCriteria<ItemWithAsynchRule, string> criteria)
    {
      this.Id = "fetch_" + criteria.Value;
      this.OperationResult = "DataPortal_Fetch/with parameters";
      this.MarkOld();
    }

    internal void DataPortal_Create()
    {
      this.Id = "random_create";
      this.OperationResult = "DataPortal_Create/no parameters";
      this.MarkNew();
    }

    internal void DataPortal_Insert()
    {
      this.Id = "random_insert";
      this.OperationResult = "DataPortal_Insert";
      this.MarkOld();
    }

    internal void DataPortal_Update()
    {
      this.Id = "random_update";
      this.OperationResult = "DataPortal_Update";
      this.MarkOld();
    }
#endif

  }
}
