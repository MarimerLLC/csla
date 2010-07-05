//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
  public enum CustomeType
  {
    Active,
    Inactive
  }

  [Serializable]
  public class Customer : BusinessBase<Customer>
  {
    const int customerIDThrowsException = 99;

#if SILVERLIGHT
    public Customer() { }
#else
    private Customer() { }
#endif

    internal static Customer NewCustomer()
    {
      var returnValue = new Customer();
      returnValue.Name = "New";
      returnValue.MarkAsChild();
      return returnValue;
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(new PropertyInfo<int>("Id", "Customer Id", 0));
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

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name", "Customer Name", ""));
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


    private static PropertyInfo<SmartDate> DateCreatedProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("DateCreated", "Date Created On"));
    public string DateCreated
    {
      get
      {
        return GetProperty<SmartDate>(DateCreatedProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty<SmartDate>(DateCreatedProperty, test);
        }
      }
    }


    private static PropertyInfo<bool> ThrowExceptionProperty = RegisterProperty<bool>(new PropertyInfo<bool>("ThrowException", "ThrowException", false));
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

    private static PropertyInfo<DateTimeOffset> DateTimeOffsetNullProperty = RegisterProperty<DateTimeOffset>(new PropertyInfo<DateTimeOffset>("DateTimeOffsetNull", "DateTimeOffsetNull"));
    public DateTimeOffset DateTimeOffsetNull
    {
      get
      {
        return GetProperty(DateTimeOffsetNullProperty);
      }
      set
      {
        LoadProperty(DateTimeOffsetNullProperty, value);
      }
    }

    private static PropertyInfo<DateTimeOffset> DateTimeOffsetNotNullProperty = RegisterProperty<DateTimeOffset>(new PropertyInfo<DateTimeOffset>("DateTimeOffsetNotNull", "DateTimeOffsetNotNull", DateTimeOffset.Now));
    public DateTimeOffset DateTimeOffsetNotNull
    {
      get
      {
        return GetProperty(DateTimeOffsetNotNullProperty);
      }
      set
      {
        LoadProperty(DateTimeOffsetNotNullProperty, value);
      }
    }

    private static PropertyInfo<DateTimeOffset?> DateTimeOffsetNullableProperty = RegisterProperty<DateTimeOffset?>(new PropertyInfo<DateTimeOffset?>("DateTimeOffsetNullable", "DateTimeOffsetNullable"));
    public DateTimeOffset? DateTimeOffsetNullable
    {
      get
      {
        return GetProperty(DateTimeOffsetNullableProperty);
      }
      set
      {
        LoadProperty(DateTimeOffsetNullableProperty, value);
      }
    }

    private static PropertyInfo<CustomeType> TypeProperty = RegisterProperty(new PropertyInfo<CustomeType>("Type", "Customer Type", CustomeType.Active));
    public CustomeType Type
    {
      get
      {
        return GetProperty(TypeProperty);
      }
      set
      {
        SetProperty(TypeProperty, value);
      }
    }


    private static PropertyInfo<CustomerContactList> ContactsProperty = RegisterProperty<CustomerContactList>(new PropertyInfo<CustomerContactList>("Contacts", "Contacts List"));
    public CustomerContactList Contacts
    {
      get
      {
        return GetProperty<CustomerContactList>(ContactsProperty);
      }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(IdProperty, 1));
    }


    [Serializable()]
    public class FetchCriteria : CriteriaBase<FetchCriteria>
    {
      public FetchCriteria() { }

      public FetchCriteria(int customerID)
        : base()
      {
        _customerId = customerID;
      }

      private int _customerId;

      public int CustomerID
      {
        get
        {
          return _customerId;
        }
      }

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("_customerId", _customerId);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _customerId = info.GetValue<int>("_customerId");
      }
    }

    public static void GetCustomer(int customerID, EventHandler<DataPortalResult<Customer>> handler)
    {
      var dp = new DataPortal<Customer>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Customer, int>(customerID));
    }
#if SILVERLIGHT

    public static void GetCustomer(EventHandler<DataPortalResult<Customer>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      var dp = new DataPortal<Customer>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Customer, int>(customerID));
    }

    public static void GetCustomerWithException(EventHandler<DataPortalResult<Customer>> handler)
    {
      var dp = new DataPortal<Customer>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Customer, int>(customerIDThrowsException));
    }
    public static void CreateCustomer(EventHandler<DataPortalResult<Customer>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      var dp = new DataPortal<Customer>();
      dp.CreateCompleted += handler;
      dp.BeginCreate(new SingleCriteria<Customer, int>(customerID));
    }
#endif
#if !SILVERLIGHT

    internal static Customer GetCustomer(int customerID)
    {
      Customer newCustomer = new Customer();
      newCustomer.DataPortal_Fetch(new SingleCriteria<Customer, int>(customerID));
      newCustomer.MarkAsChild();
      newCustomer.MarkOld();
      return newCustomer;
    }

    protected void DataPortal_Fetch(SingleCriteria<Customer, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "Customer Name for Id: " + criteria.Value.ToString());
      LoadProperty(DateCreatedProperty, new SmartDate(new DateTime(2000 + criteria.Value, 1, 1)));
      LoadProperty(ContactsProperty, CustomerContactList.GetCustomerContactList(criteria.Value));
      LoadProperty(DateTimeOffsetNotNullProperty, DateTimeOffset.Now);
      LoadProperty(TypeProperty, CustomeType.Inactive);

      if (criteria.Value == customerIDThrowsException)
        throw new ApplicationException("Test for Silverlight DataSource Error!");
    }

    protected void DataPortal_Create(SingleCriteria<Customer, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "New Customer for Id: " + criteria.Value.ToString());
      LoadProperty(DateCreatedProperty, new SmartDate(DateTime.Today));
      LoadProperty(DateTimeOffsetNotNullProperty, DateTimeOffset.Now);
      LoadProperty(ContactsProperty, CustomerContactList.GetCustomerContactList(0));
    }

    protected override void DataPortal_DeleteSelf()
    {
      Method = "Deleted Customer " + GetProperty<string>(NameProperty);
    }

    protected void DataPortal_Delete(SingleCriteria<Customer, int> criteria)
    {
      Method = "Deleted Customer ID " + criteria.Value.ToString();
    }

    protected override void DataPortal_Insert()
    {
      Method = "Inserted Customer " + GetProperty<string>(NameProperty);
      DataPortal.UpdateChild(ReadProperty(ContactsProperty));
    }
    protected override void DataPortal_Update()
    {
      Method = "Updating Customer " + GetProperty<string>(NameProperty);
      DataPortal.UpdateChild(ReadProperty(ContactsProperty));
    }
#endif

  }


  [Serializable]
  public class CustomerWO_DP_XYZ : BusinessBase<CustomerWO_DP_XYZ>
  {
    //#if SILVERLIGHT

    public static void GetCustomer(EventHandler<DataPortalResult<CustomerWO_DP_XYZ>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWO_DP_XYZ>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<CustomerWO_DP_XYZ, int>(customerID));
    }

    public static void CreateCustomer(EventHandler<DataPortalResult<CustomerWO_DP_XYZ>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      var dp = new DataPortal<CustomerWO_DP_XYZ>();
      dp.CreateCompleted += handler;
      dp.BeginCreate(new SingleCriteria<CustomerWO_DP_XYZ, int>(customerID));
    }
    //#endif

  }

}