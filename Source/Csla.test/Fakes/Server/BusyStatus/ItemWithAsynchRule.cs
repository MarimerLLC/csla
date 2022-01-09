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
using System.Threading.Tasks;

namespace Csla.Testing.Business.BusyStatus
{
  [Serializable]
  public class ItemWithAsynchRule : BusinessBase<ItemWithAsynchRule>
  {
    public ItemWithAsynchRule() { }

    private static PropertyInfo<string> IdProperty = RegisterProperty<string>(c => c.Id);

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

    private static PropertyInfo<string> OperationResultProperty = RegisterProperty<string>(c => c.OperationResult, "Operation Result", string.Empty);

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

    private static PropertyInfo<string> RuleFieldProperty = RegisterProperty<string>(c => c.RuleField, "Rule Field", string.Empty);

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

    internal static ItemWithAsynchRule GetOneItemForList(IChildDataPortal<ItemWithAsynchRule> childDataPortal, string id)
    {
      return childDataPortal.FetchChild(id);
    }

    public bool IsRunningRules
    {
      get { return BusinessRules.RunningAsyncRules && BusinessRules.RunningRules; }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new TwoSecondsLongRule(RuleFieldProperty));
    }

    public class TwoSecondsLongRule : Rules.BusinessRuleAsync
    {
      public TwoSecondsLongRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
        base.RunMode = RunModes.DenyOnServerSidePortal | RunModes.DenyCheckRules;
      }

      protected override async Task ExecuteAsync(IRuleContext context)
      {
        // TODO: I'm not sure how I would replicate this exact check in Csla 6.
        // For now, I've had to stop it running at all in the server-side portal - but this is not the same as before!
        // What the rule used to do was run everywhere but finish immediately if on the server, now it doesn't
        // run at all on the server :-(
        //if (Csla.ApplicationContext.LogicalExecutionLocation == Csla.ApplicationContext.LogicalExecutionLocations.Client)
        //{
        await Task.Delay(2000);

        var value = context.InputPropertyValues[PrimaryProperty];
        if (value == null || value.ToString().ToUpper() == "ERROR")
          context.AddErrorResult("error detected");
        //}
        //else
        //{
        //  var value = context.InputPropertyValues[PrimaryProperty];
        //  if (value == null || value.ToString().ToUpper() == "ERROR")
        //    context.AddErrorResult("error detected");
        //  context.Complete();
        //}
      }
    }

    [FetchChild]
    private void FetchChild(string id)
    {
      using (BypassPropertyChecks)
      {
        LoadProperty(IdProperty, id);
      }
      BusinessRules.CheckRules();
    }

    [Fetch]
    internal void DataPortal_Fetch(string criteria)
    {
      this.Id = "fetch_" + criteria;
      this.OperationResult = "DataPortal_Fetch/with parameters";
    }

    [Create]
    protected void DataPortal_Create()
    {
      this.Id = "random_create";
      this.OperationResult = "DataPortal_Create/no parameters";
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      this.Id = "random_insert";
      this.OperationResult = "DataPortal_Insert";
      this.MarkOld();
    }

    [Update]
    protected void DataPortal_Update()
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