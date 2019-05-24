//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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

    private Customer() { }

    internal static Customer NewCustomer()
    {
      var returnValue = new Customer();
      returnValue.Name = "New";
      returnValue.MarkAsChild();
      return returnValue;
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c=>c.Id, "Customer Id", 0);
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

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c=>c.Name, "Customer Name", "");
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

    private static PropertyInfo<string> MethodProperty = RegisterProperty<string>(c=>c.Method, "Method", "");
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


    private static PropertyInfo<SmartDate> DateCreatedProperty = RegisterProperty<SmartDate>(c=>c.DateCreated, "Date Created On");
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


    private static PropertyInfo<bool> ThrowExceptionProperty = RegisterProperty<bool>(c=>c.ThrowException, "ThrowException", false);
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

    private static PropertyInfo<DateTimeOffset> DateTimeOffsetNullProperty = RegisterProperty<DateTimeOffset>(c=>c.DateTimeOffsetNull, "DateTimeOffsetNull");
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

    private static PropertyInfo<DateTimeOffset> DateTimeOffsetNotNullProperty = RegisterProperty<DateTimeOffset>(c=>c.DateTimeOffsetNotNull, "DateTimeOffsetNotNull", DateTimeOffset.Now);
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

    private static PropertyInfo<DateTimeOffset?> DateTimeOffsetNullableProperty = RegisterProperty<DateTimeOffset?>(c=>c.DateTimeOffsetNullable, "DateTimeOffsetNullable");
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

    private static PropertyInfo<CustomeType> TypeProperty = RegisterProperty(c=>c.Type, "Customer Type", CustomeType.Active);
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


    private static PropertyInfo<CustomerContactList> ContactsProperty = RegisterProperty<CustomerContactList>(c=>c.Contacts, "Contacts List");
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


    internal static Customer GetCustomer(int customerID)
    {
      Customer newCustomer = new Customer();
      newCustomer.DataPortal_Fetch(customerID);
      newCustomer.MarkAsChild();
      newCustomer.MarkOld();
      return newCustomer;
    }

    protected void DataPortal_Fetch(int criteria)
    {
      LoadProperty(IdProperty, criteria);
      LoadProperty(NameProperty, "Customer Name for Id: " + criteria.ToString());
      LoadProperty(DateCreatedProperty, new SmartDate(new DateTime(2000 + criteria, 1, 1)));
      LoadProperty(ContactsProperty, CustomerContactList.GetCustomerContactList(criteria));
      LoadProperty(DateTimeOffsetNotNullProperty, DateTimeOffset.Now);
      LoadProperty(TypeProperty, CustomeType.Inactive);

      if (criteria == customerIDThrowsException)
        throw new ApplicationException("Test for Silverlight DataSource Error!");
    }

    protected void DataPortal_Create(int criteria)
    {
      LoadProperty(IdProperty, criteria);
      LoadProperty(NameProperty, "New Customer for Id: " + criteria.ToString());
      LoadProperty(DateCreatedProperty, new SmartDate(DateTime.Today));
      LoadProperty(DateTimeOffsetNotNullProperty, DateTimeOffset.Now);
      LoadProperty(ContactsProperty, CustomerContactList.GetCustomerContactList(0));
    }

    protected override void DataPortal_DeleteSelf()
    {
      Method = "Deleted Customer " + GetProperty<string>(NameProperty);
    }

    protected void DataPortal_Delete(int criteria)
    {
      Method = "Deleted Customer ID " + criteria.ToString();
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
  }


  [Serializable]
  public class CustomerWO_DP_XYZ : BusinessBase<CustomerWO_DP_XYZ>
  {
  }
}