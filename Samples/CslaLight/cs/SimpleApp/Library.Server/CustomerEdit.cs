using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Library
{
  [Serializable]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    #region Business Methods

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c =>c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c=>c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c=>c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
      set { SetProperty(StatusProperty, value); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringRequired, NameProperty);
    }

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(IdProperty, "None");
      //AuthorizationRules.AllowWrite(NameProperty, "None");
    }

    #endregion

    #region Factory Methods

#if SILVERLIGHT
    public static void BeginNewCustomer(
      DataPortal.ProxyModes proxyMode,
      EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      var dp = new DataPortal<CustomerEdit>(DataPortal.ProxyModes.LocalOnly);
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    public static void BeginGetCustomer(
      DataPortal.ProxyModes proxyMode,
      EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      var dp = new DataPortal<CustomerEdit>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    [Obsolete("For use by MobileFormatter")]
    public CustomerEdit()
    { /* required by MobileFormatter */ }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Create(Csla.DataPortalClient.LocalProxy<CustomerEdit>.CompletedHandler handler)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.DoWork += (s, e) =>
        { e.Result = e.Argument; };
      bw.RunWorkerCompleted += (s, e) =>
        {
          try
          {
            using (BypassPropertyChecks)
              Status = "Created " + ApplicationContext.ExecutionLocation.ToString();
            CreateComplete(handler);
          }
          catch (Exception ex)
          {
            handler(this, ex);
          }
        };
      bw.RunWorkerAsync();
    }

    private void CreateComplete(Csla.DataPortalClient.LocalProxy<CustomerEdit>.CompletedHandler handler)
    {
      base.DataPortal_Create(handler);
    }
#else
    public static CustomerEdit GetCustomer(int id)
    {
      return DataPortal.Fetch<CustomerEdit>(new SingleCriteria<CustomerEdit, int>(id));
    }

    private CustomerEdit()
    {	/* require use of factory methods */ }
#endif

    #endregion

    #region Data Access

#if !SILVERLIGHT
    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
        Status = "Created " + ApplicationContext.ExecutionLocation.ToString();
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<CustomerEdit, int> criteria)
    {
      using (BypassPropertyChecks)
      {
        Id = criteria.Value;
        Name = "Test " + criteria.Value;
        Status = "Retrieved " + ApplicationContext.ExecutionLocation.ToString();
      }
    }

    protected override void DataPortal_Insert()
    {
      using (BypassPropertyChecks)
      {
        Id = 987;
        Status = "Inserted " + ApplicationContext.ExecutionLocation.ToString();
      }
    }

    protected override void DataPortal_Update()
    {
      using (BypassPropertyChecks)
        Status = "Updated " + ApplicationContext.ExecutionLocation.ToString();
    }

    protected override void DataPortal_DeleteSelf()
    {
      using (BypassPropertyChecks)
        Status = "Deleted " + ApplicationContext.ExecutionLocation.ToString();
    }
#endif

    #endregion
  }
}