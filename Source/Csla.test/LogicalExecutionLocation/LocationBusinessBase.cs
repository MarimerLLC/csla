//-----------------------------------------------------------------------
// <copyright file="LocationBusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
  public class LocationBusinessBase: BusinessBase<LocationBusinessBase>
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
#if !SILVERLIGHT

    public static LocationBusinessBase GetLocationBusinessBase()
    {
      return Csla.DataPortal.Fetch<LocationBusinessBase>();
    }
#else
    public static void GetLocationBusinessBase(EventHandler<DataPortalResult<LocationBusinessBase>> handler)
    {
      Csla.DataPortal.BeginFetch<LocationBusinessBase>(handler);
    }
#endif
    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new CheckRule { PrimaryProperty = DataProperty });
    }

    public class CheckRule : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        ((LocationBusinessBase)context.Target).Rule = Csla.ApplicationContext.LogicalExecutionLocation.ToString();
      }
    }

#if !SILVERLIGHT

    protected override void DataPortal_Update()
    {
      
    }

    protected void DataPortal_Fetch()
    {
      SetProperty(DataProperty, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
      var nested = Csla.DataPortal.Fetch<LocationBusinessBase>(new SingleCriteria<LocationBusinessBase, int>(123));
      NestedData = nested.Data;
    }

    protected void DataPortal_Fetch(object criteria)
    {
      SetProperty(DataProperty, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
    }
#endif
  }
}