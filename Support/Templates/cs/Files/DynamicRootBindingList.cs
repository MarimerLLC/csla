using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRootBindingList :
    DynamicBindingListBase<DynamicRoot>
  {
    #region Business Methods

    protected override object AddNewCore()
    {
      DynamicRoot item = DynamicRoot.NewDynamicRoot();
      Add(item);
      return item;
    }

    #endregion

    #region  Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(DynamicRootBindingList), "Role");
      //AuthorizationRules.AllowEdit(typeof(DynamicRootBindingList), "Role");
    }

    #endregion

    #region  Factory Methods

    public static DynamicRootBindingList NewDynamicRootBindingList()
    {
      return DataPortal.Create<DynamicRootBindingList>();
    }

    public static DynamicRootBindingList GetDynamicRootBindingList()
    {
      return DataPortal.Fetch<DynamicRootBindingList>();
    }

    public DynamicRootBindingList()
    {
      AllowNew = true;
    }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      // TODO: load values
      RaiseListChangedEvents = false;
      object listData = null;
      foreach (var item in (List<object>)listData)
        Add(DynamicRoot.GetDynamicRoot(item));
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}