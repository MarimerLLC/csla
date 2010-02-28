using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace cslalighttest.CslaDataProvider
{
  [Serializable]
  public class CustomerWithError : BusinessBase<CustomerWithError>
  {
    const int CustomerWithErrorIDThrowsException = 99;

#if SILVERLIGHT
    public CustomerWithError() { }
#else
    private CustomerWithError() { }
#endif

    internal static CustomerWithError NewCustomerWithError()
    {
      var returnValue = new CustomerWithError();
      returnValue.Name = "New";
      return returnValue;
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(new PropertyInfo<int>("Id", "CustomerWithError Id", 0));
    public int Id
    {
      get
      {
        return GetProperty<int>(IdProperty);
      }
      set
      {
        SetProperty<int>(IdProperty, value);
      }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name", "CustomerWithError Name", ""));
    public string Name
    {
      get
      {
        return GetProperty<string>(NameProperty);
      }
      set
      {
        SetProperty<string>(NameProperty, value);
      }
    }

    private static PropertyInfo<string> MethodProperty = RegisterProperty<string>(new PropertyInfo<string>("Method", "Method", ""));
    public string Method
    {
      get
      {
        return GetProperty<string>(MethodProperty);
      }
      set
      {
        SetProperty<string>(MethodProperty, value);
      }
    }


    private static PropertyInfo<bool> ThrowExceptionProperty = RegisterProperty<bool>(new PropertyInfo<bool>("ThrowException", "ThrowException", true));
    public bool ThrowException
    {
      get
      {
        return GetProperty<bool>(ThrowExceptionProperty);
      }
      set
      {
        LoadProperty<bool>(ThrowExceptionProperty, value);
      }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.IntegerMinValue, new Csla.Validation.CommonRules.IntegerMinValueRuleArgs(IdProperty, 1));
    }


    [Serializable()]
    public class FetchCriteria : CriteriaBase<FetchCriteria>
    {
      public FetchCriteria() { }

      public FetchCriteria(int CustomerWithErrorID)
        : base(typeof(CustomerWithError))
      {
        _CustomerWithErrorId = CustomerWithErrorID;
      }

      private int _CustomerWithErrorId;

      public int CustomerWithErrorID
      {
        get
        {
          return _CustomerWithErrorId;
        }
      }

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("_CustomerWithErrorId", _CustomerWithErrorId);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _CustomerWithErrorId = info.GetValue<int>("_CustomerWithErrorId");
      }
    }

    public static void GetCustomerWithError(int CustomerWithErrorID, EventHandler<DataPortalResult<CustomerWithError>> handler)
    {
      var dp = new DataPortal<CustomerWithError>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<CustomerWithError, int>(CustomerWithErrorID));
    }
#if SILVERLIGHT

    public static void GetCustomerWithError(EventHandler<DataPortalResult<CustomerWithError>> handler)
    {
      int CustomerWithErrorID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWithError>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<CustomerWithError, int>(CustomerWithErrorID));
    }

    public static void GetCustomerWithErrorWithException(EventHandler<DataPortalResult<CustomerWithError>> handler)
    {
      var dp = new DataPortal<CustomerWithError>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<CustomerWithError, int>(CustomerWithErrorIDThrowsException));
    }
    public static void CreateCustomerWithError(EventHandler<DataPortalResult<CustomerWithError>> handler)
    {
      int CustomerWithErrorID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWithError>();
      dp.CreateCompleted += handler;
      dp.BeginCreate(new SingleCriteria<CustomerWithError, int>(CustomerWithErrorID));
    }
#endif
#if !SILVERLIGHT

    internal static CustomerWithError GetCustomerWithError(int CustomerWithErrorID)
    {
      CustomerWithError newCustomerWithError = new CustomerWithError();
      newCustomerWithError.DataPortal_Fetch(new SingleCriteria<CustomerWithError, int>(CustomerWithErrorID));
      newCustomerWithError.MarkAsChild();
      newCustomerWithError.MarkOld();
      return newCustomerWithError;
    }

    protected void DataPortal_Fetch(SingleCriteria<CustomerWithError, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "CustomerWithError Name for Id: " + criteria.Value.ToString());

      if (criteria.Value == CustomerWithErrorIDThrowsException)
        throw new ApplicationException("Test for Silverlight DataSource Error!");
    }

    protected void DataPortal_Create(SingleCriteria<CustomerWithError, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "New CustomerWithError for Id: " + criteria.Value.ToString());
    }

    protected override void DataPortal_DeleteSelf()
    {
      Method = "Deleted CustomerWithError " + GetProperty<string>(NameProperty);
    }

    protected void DataPortal_Delete(SingleCriteria<CustomerWithError, int> criteria)
    {
      Method = "Deleted CustomerWithError ID " + criteria.Value.ToString();
    }

    protected override void DataPortal_Insert()
    {
      Method = "Inserted CustomerWithError " + GetProperty<string>(NameProperty);
    }
    protected override void DataPortal_Update()
    {
      Method = "Updating CustomerWithError " + GetProperty<string>(NameProperty);
    }
#endif

  }


  [Serializable]
  public class CustomerWithErrorWO_DP_XYZ : BusinessBase<CustomerWithErrorWO_DP_XYZ>
  {
    //#if SILVERLIGHT

    public static void GetCustomerWithError(EventHandler<DataPortalResult<CustomerWithErrorWO_DP_XYZ>> handler)
    {
      int CustomerWithErrorID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWithErrorWO_DP_XYZ>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<CustomerWithErrorWO_DP_XYZ, int>(CustomerWithErrorID));
    }

    public static void CreateCustomerWithError(EventHandler<DataPortalResult<CustomerWithErrorWO_DP_XYZ>> handler)
    {
      int CustomerWithErrorID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWithErrorWO_DP_XYZ>();
      dp.CreateCompleted += handler;
      dp.BeginCreate(new SingleCriteria<CustomerWithErrorWO_DP_XYZ, int>(CustomerWithErrorID));
    }
    //#endif

  }

}
