using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable]
  public class Customer : BusinessBase<Customer>
  {
#if SILVERLIGHT
    public Customer() { }
#else
    private Customer() { }
#endif

    internal static Customer NewCustomer()
    {
      Customer returnValue = new Customer();
      returnValue.Name = "New";
      return returnValue;
    }

    internal static Customer NewChildCustomer()
    {
      Customer returnValue = new Customer();
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

    private static PropertyInfo<CustomerContactList> ContactsProperty = RegisterProperty<CustomerContactList>(typeof(Customer),new PropertyInfo<CustomerContactList>("Contacts", "Contacts List"));
    public CustomerContactList Contacts
    {
      get
      {
        return GetProperty<CustomerContactList>(ContactsProperty);
      }
    }

    protected override void AddAuthorizationRules()
    {
      //AuthorizationRules.AllowCreate(typeof(Customer), new string[1] { "admin"});
      //AuthorizationRules.AllowDelete(typeof(Customer), new string[1] { "admin" });
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.IntegerMinValue, new Csla.Validation.CommonRules.IntegerMinValueRuleArgs(IdProperty, 1));
    }


    [Serializable()]
    public class FetchCriteria : CriteriaBase
    {
      public FetchCriteria() { }

      public FetchCriteria(int customerID)
        : base(typeof(Customer))
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
      DataPortal<Customer> dp = new DataPortal<Customer>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Customer, int>(customerID));
    }
#if SILVERLIGHT
   
    public static void GetCustomer(EventHandler<DataPortalResult<Customer>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      DataPortal<Customer> dp = new DataPortal<Customer>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Customer, int>(customerID));
    }

    public static void CreateCustomer(EventHandler<DataPortalResult<Customer>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      DataPortal<Customer> dp = new DataPortal<Customer>();
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
    }

    protected void DataPortal_Create(SingleCriteria<Customer, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "New Customer for Id: " + criteria.Value.ToString());
      LoadProperty(DateCreatedProperty, new SmartDate(DateTime.Today));
      LoadProperty(ContactsProperty, CustomerContactList.GetCustomerContactList(0));
    }

    protected internal void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext["CustomerDelete"] = "Deleted Customer " + GetProperty<string>(NameProperty);
    }

    protected internal void DataPortal_Insert()
    {
      Csla.ApplicationContext.GlobalContext["CustomerInsert"] = "Inserted Customer " + GetProperty<string>(NameProperty);
      MarkClean();
    }
    protected internal void DataPortal_Update()
    {
      Csla.ApplicationContext.GlobalContext["CustomerUpdate"] = "Updating Customer " + GetProperty<string>(NameProperty);
      MarkClean();
    }
#endif

  }
}
