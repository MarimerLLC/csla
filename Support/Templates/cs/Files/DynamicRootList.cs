using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRootList :
    DynamicListBase<DynamicRoot>
  {
    #region Business Methods

    protected override DynamicRoot AddNewCore()
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
      //AuthorizationRules.AllowGet(typeof(DynamicRootList), "Role");
      //AuthorizationRules.AllowEdit(typeof(DynamicRootList), "Role");
    }

    #endregion

    #region  Factory Methods

    public static DynamicRootList NewDynamicRootList()
    {
      return DataPortal.Create<DynamicRootList>();
    }

    public static DynamicRootList GetDynamicRootList()
    {
      return DataPortal.Fetch<DynamicRootList>();
    }

    public DynamicRootList()
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