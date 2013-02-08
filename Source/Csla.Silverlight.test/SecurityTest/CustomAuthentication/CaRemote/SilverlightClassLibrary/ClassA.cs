using System;
using System.Net;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace ClassLibrary
{
  [Serializable()]
  public class ClassA : BusinessBase<ClassA>
  {
    #region Factory Methods
    public static void Fetch(EventHandler<DataPortalResult<ClassA>> completed)
    {
      DataPortal<ClassA> dp = new DataPortal<ClassA>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }
    #endregion
    #region Data Access
#if !SILVERLIGHT
    private void DataPortal_Fetch()
    {
      A = "test";
      B = "test";
      GlobalContext = (string)Csla.ApplicationContext.GlobalContext["Test"];
      ClientContext = (string)Csla.ApplicationContext.ClientContext["TestClient"];
      Csla.ApplicationContext.GlobalContext["Test"] = "GlobalChangedByPortal";
      Csla.ApplicationContext.ClientContext["TestClient"] = "ClientChangedByPortal";
    }
    
    [RunLocal()]
    protected override void DataPortal_Create()
    {

    }
#endif
    #endregion

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

    private static PropertyInfo<string> ClientContextProperty = RegisterProperty(new PropertyInfo<string>("ClientContext"));
    public string ClientContext
    {
      get { return GetProperty<string>(ClientContextProperty); }
      set { SetProperty<string>(ClientContextProperty, value); }
    }

    private static PropertyInfo<string> GlobalContextProperty = RegisterProperty(new PropertyInfo<string>("GlobalContext"));
    public string GlobalContext
    {
      get { return GetProperty<string>(GlobalContextProperty); }
      set { SetProperty<string>(GlobalContextProperty, value); }
    }


    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(AProperty, "PropertyARole");
      AuthorizationRules.AllowRead(AProperty, "PropertyARole");
    }

    public static void AddObjectAuthorizationRules()
    {
      AuthorizationRules.AllowGet(typeof(ClassA), "ClassARole");
      AuthorizationRules.AllowCreate(typeof(ClassA), "ClassARole");
      AuthorizationRules.AllowEdit(typeof(ClassA), "ClassARole");
      AuthorizationRules.AllowDelete(typeof(ClassA), "ClassARole");
    }
  }
}
