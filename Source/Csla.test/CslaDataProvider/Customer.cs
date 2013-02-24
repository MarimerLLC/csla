﻿//-----------------------------------------------------------------------
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

namespace Csla.Test.CslaDataProvider
{
  [Serializable]
  public class Customer : BusinessBase<Customer>
  {
    const int customerIDThrowsException = 99;

    private Customer() { }

    internal static Customer NewCustomer()
    {
      var returnValue = new Customer();
      returnValue.Name = "New";
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


    private static PropertyInfo<Csla.SmartDate> DateCreatedProperty = RegisterProperty<Csla.SmartDate>(new PropertyInfo<Csla.SmartDate>("DateCreated", "Date Created On"));
    public string DateCreated
    {
      get
      {
        return GetProperty<Csla.SmartDate>(DateCreatedProperty).Text;
      }
      set
      {
        Csla.SmartDate test = new Csla.SmartDate();
        if (Csla.SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty<Csla.SmartDate>(DateCreatedProperty, test);
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

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(IdProperty, 1));
    }


    [Serializable()]
    public class FetchCriteria : CriteriaBase<FetchCriteria>
    {
      public FetchCriteria() { }

      public FetchCriteria(int customerID)
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

    internal static Customer GetCustomer(int customerID)
    {
      Customer newCustomer = new Customer();
      newCustomer.DataPortal_Fetch(new SingleCriteria<Customer, int>(customerID));
      newCustomer.MarkOld();
      return newCustomer;
    }

    protected void DataPortal_Fetch(SingleCriteria<Customer, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "Customer Name for Id: " + criteria.Value.ToString());
      LoadProperty(DateCreatedProperty, new Csla.SmartDate(new DateTime(2000 + criteria.Value, 1, 1)));

      if (criteria.Value == customerIDThrowsException)
        throw new ApplicationException("Test Error!");
    }

    protected void DataPortal_Create(SingleCriteria<Customer, int> criteria)
    {
      LoadProperty(IdProperty, criteria.Value);
      LoadProperty(NameProperty, "New Customer for Id: " + criteria.Value.ToString());
      LoadProperty(DateCreatedProperty, new Csla.SmartDate(DateTime.Today));
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
    }
    protected override void DataPortal_Update()
    {
      Method = "Updating Customer " + GetProperty<string>(NameProperty);
    }

  }




}