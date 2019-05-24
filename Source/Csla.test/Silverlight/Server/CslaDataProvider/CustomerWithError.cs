//-----------------------------------------------------------------------
// <copyright file="CustomerWithError.cs" company="Marimer LLC">
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
  [Serializable]
  public class CustomerWithError : BusinessBase<CustomerWithError>
  {
    const int CustomerWithErrorIDThrowsException = 99;

    private CustomerWithError() { }

    internal static CustomerWithError NewCustomerWithError()
    {
      var returnValue = new CustomerWithError();
      returnValue.Name = "New";
      return returnValue;
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c=>c.Id, "CustomerWithError Id", 0);
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

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c=>c.Name, "CustomerWithError Name", "");
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


    private static PropertyInfo<bool> ThrowExceptionProperty = RegisterProperty<bool>(c => c.ThrowException, "ThrowException", true);
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

      public FetchCriteria(int CustomerWithErrorID)
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

    internal static CustomerWithError GetCustomerWithError(int CustomerWithErrorID)
    {
      CustomerWithError newCustomerWithError = new CustomerWithError();
      newCustomerWithError.DataPortal_Fetch(CustomerWithErrorID);
      newCustomerWithError.MarkAsChild();
      newCustomerWithError.MarkOld();
      return newCustomerWithError;
    }

    protected void DataPortal_Fetch(int criteria)
    {
      LoadProperty(IdProperty, criteria);
      LoadProperty(NameProperty, "CustomerWithError Name for Id: " + criteria.ToString());

      if (criteria == CustomerWithErrorIDThrowsException)
        throw new ApplicationException("Test for Silverlight DataSource Error!");
    }

    protected void DataPortal_Create(int criteria)
    {
      LoadProperty(IdProperty, criteria);
      LoadProperty(NameProperty, "New CustomerWithError for Id: " + criteria.ToString());
    }

    protected override void DataPortal_DeleteSelf()
    {
      Method = "Deleted CustomerWithError " + GetProperty<string>(NameProperty);
    }

    protected void DataPortal_Delete(int criteria)
    {
      Method = "Deleted CustomerWithError ID " + criteria.ToString();
    }

    protected override void DataPortal_Insert()
    {
      Method = "Inserted CustomerWithError " + GetProperty<string>(NameProperty);
    }
    protected override void DataPortal_Update()
    {
      Method = "Updating CustomerWithError " + GetProperty<string>(NameProperty);
    }
  }


  [Serializable]
  public class CustomerWithErrorWO_DP_XYZ : BusinessBase<CustomerWithErrorWO_DP_XYZ>
  {
  }

}