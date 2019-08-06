//-----------------------------------------------------------------------
// <copyright file="LocationBusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.LogicalExecutionLocation
{
  [Serializable]
#pragma warning disable CS0436 // Type conflicts with imported type
  public class LocationBusinessBase: BusinessBase<LocationBusinessBase>
#pragma warning restore CS0436 // Type conflicts with imported type
  {

    protected static PropertyInfo<string> DataProperty = RegisterProperty<string>(new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<string> NestedDataProperty = RegisterProperty<string>(c => c.NestedData);
    public string NestedData
    {
      get { return GetProperty(NestedDataProperty); }
      set { SetProperty(NestedDataProperty, value); }
    }

    protected static PropertyInfo<string> RuleProperty = RegisterProperty<string>(new PropertyInfo<string>("Rule"));
    public string Rule
    {
      get { return GetProperty(RuleProperty); }
      set { SetProperty(RuleProperty, value); }
    }

#pragma warning disable CS0436 // Type conflicts with imported type
    public static LocationBusinessBase GetLocationBusinessBase()
#pragma warning restore CS0436 // Type conflicts with imported type
    {
#pragma warning disable CS0436 // Type conflicts with imported type
      return Csla.DataPortal.Fetch<LocationBusinessBase>();
#pragma warning restore CS0436 // Type conflicts with imported type
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new CheckRule { PrimaryProperty = DataProperty });
    }

    public class CheckRule : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
#pragma warning disable CS0436 // Type conflicts with imported type
        ((LocationBusinessBase)context.Target).Rule = Csla.ApplicationContext.LogicalExecutionLocation.ToString();
#pragma warning restore CS0436 // Type conflicts with imported type
      }
    }

    protected override void DataPortal_Update()
    {
      
    }

    protected void DataPortal_Fetch()
    {
      SetProperty(DataProperty, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
#pragma warning disable CS0436 // Type conflicts with imported type
      var nested = Csla.DataPortal.Fetch<LocationBusinessBase>(123);
#pragma warning restore CS0436 // Type conflicts with imported type
      NestedData = nested.Data;
    }

    protected void DataPortal_Fetch(int criteria)
    {
      SetProperty(DataProperty, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
    }
  }
}