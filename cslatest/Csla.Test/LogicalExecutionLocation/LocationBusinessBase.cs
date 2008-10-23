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
      ValidationRules.AddRule<LocationBusinessBase>(CheckRule, DataProperty);
    }

    private static bool CheckRule(LocationBusinessBase item, Csla.Validation.RuleArgs e)
    {
      item.Rule = Csla.ApplicationContext.LogicalExecutionLocation.ToString();
      return true;
    }

#if !SILVERLIGHT

    protected override void DataPortal_Update()
    {
      
    }

    protected void DataPortal_Fetch()
    {
      SetProperty(DataProperty, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
    }
#endif
  }
}
