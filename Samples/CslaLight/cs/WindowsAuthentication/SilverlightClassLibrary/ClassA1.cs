using System;
using System.Net;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace ClassLibrary
{
  [Serializable()]
  public class ClassA1 : BusinessBase<ClassA1>
  {
    private static PropertyInfo<string> AProperty = RegisterProperty(new PropertyInfo<string>("A"));
    public string A
    {
      get { return GetProperty<string>(AProperty); }
      set { SetProperty<string>(AProperty, value); }
    }
    private static PropertyInfo<string> BProperty = RegisterProperty(new PropertyInfo<string>("B"));
    public string B
    {
      get { return GetProperty<string>(BProperty); }
      set { SetProperty<string>(BProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.ReadProperty, AProperty, new System.Collections.Generic.List<string>(new string[] { "Users" })));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, AProperty, new System.Collections.Generic.List<string>(new string[] { "Users" })));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, new System.Collections.Generic.List<string>(new string[] { "Users" })));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, new System.Collections.Generic.List<string>(new string[] { "Users" })));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, new System.Collections.Generic.List<string>(new string[] { "Users" })));
    }

    public void MarkMeOld()
    {
      MarkOld();
    }

    public void MarkMeDirty()
    {
      MarkDirty();
    }

#if SILVERLIGHT
    public static void GetA(string id, EventHandler<DataPortalResult<ClassA1>> handler)
    {
      DataPortal.BeginFetch<ClassA1>(new SingleCriteria<ClassA1, string>(id), handler);
    }

    public static void DeleteA(string id, EventHandler<DataPortalResult<ClassA1>> handler)
    {
      DataPortal.BeginDelete<ClassA1>(new SingleCriteria<ClassA1, string>(id), handler);
    }

    public static void CreateA(string id, EventHandler<DataPortalResult<ClassA1>> handler)
    {
      DataPortal.BeginCreate<ClassA1>(new SingleCriteria<ClassA1, string>(id), handler);
    }

#else

    private void DataPortal_Create(SingleCriteria<ClassA1, string> criteria)
    {
      LoadProperty(AProperty, Csla.ApplicationContext.User.ToString());
    }

    private void DataPortal_Fetch(SingleCriteria<ClassA1, string> criteria)
    {
      LoadProperty(AProperty, Csla.ApplicationContext.User.ToString());
    }

    private void DataPortal_Update()
    {
      LoadProperty(AProperty, Csla.ApplicationContext.User.ToString());
    }

    private void DataPortal_Delete(SingleCriteria<ClassA1, string> criteria)
    {
      LoadProperty(AProperty, Csla.ApplicationContext.User.ToString());
    }
#endif
  }
}
