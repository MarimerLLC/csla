//-----------------------------------------------------------------------
// <copyright file="CustomerContact.cs" company="Marimer LLC">
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
  public class CustomerContact : BusinessBase<CustomerContact>
  {
    private CustomerContact() { }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c=>c.Id, "Contact Id", 0);
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

    private static PropertyInfo<int> CustomerIdProperty = RegisterProperty<int>(c=>c.CustomerId, "Customer Id", 0);
    public int CustomerId
    {
      get
      {
        return GetProperty<int>(CustomerIdProperty);
      }
      set
      {
        SetProperty<int>(CustomerIdProperty, value);
      }
    }

    private static PropertyInfo<string>FirstNameProperty = RegisterProperty<string>(c=>c.FirstName, "Contact's First Name", "");
    public string FirstName
    {
      get
      {
        return GetProperty<string>(FirstNameProperty);
      }
      set
      {
        SetProperty<string>(FirstNameProperty, value);
      }
    }

    private static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c=>c.LastName, "Contact's Last Name", "");
    public string LastName
    {
      get
      {
        return GetProperty<string>(LastNameProperty);
      }
      set
      {
        SetProperty<string>(LastNameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> BirthdayProperty = RegisterProperty<SmartDate>(c=>c.Birthday, "Contact's Birthday");
    public string Birthday
    {
      get
      {
        return GetProperty<SmartDate>(BirthdayProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty<SmartDate>(BirthdayProperty, test);
        }
      }
    }

    private static PropertyInfo<string> ParentNameProperty = RegisterProperty<string>(c=>c.ParentName, "Parent Name", "");
    public string ParentName
    {
      get
      {
        return GetProperty<string>(ParentNameProperty);
      }
      set
      {
        SetProperty<string>(ParentNameProperty, value);
      }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(IdProperty, 1));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(FirstNameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(LastNameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(FirstNameProperty, 30));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(LastNameProperty, 50));
    }

    internal static CustomerContact GetCustomerContact(int customerId, int id, string firstName, string lastName, DateTime birthday) 
    {
      return DataPortal.FetchChild<CustomerContact>(customerId, id, firstName, lastName, birthday);
    }

    protected void Child_DeleteSelf()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["CustomerContactDelete"] = "Deleted Customer Contact" + GetProperty<string>(FirstNameProperty) + ", " + GetProperty<string>(LastNameProperty);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected void Child_Insert()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["CustomerContactInsert"] = "Inserted Customer Contact" + GetProperty<string>(FirstNameProperty) + ", " + GetProperty<string>(LastNameProperty);
#pragma warning restore CS0618 // Type or member is obsolete
      CustomerContactList parent = this.Parent as CustomerContactList;
      Customer grandParent = parent.MyParent;
      LoadProperty(ParentNameProperty, grandParent.Name);
    }
    protected void Child_Update()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["CustomerContactUpdate"] = "Updated  Customer Contact" + GetProperty<string>(FirstNameProperty) + ", " + GetProperty<string>(LastNameProperty);
#pragma warning restore CS0618 // Type or member is obsolete
      CustomerContactList parent = this.Parent as CustomerContactList;
      Customer grandParent = parent.MyParent;
      LoadProperty(ParentNameProperty,grandParent.Name);
    }

    private void Child_Fetch(int customerId, int id, string firstName, string lastName, DateTime birthday)
    {
      LoadProperty(CustomerIdProperty, customerId);
      LoadProperty(IdProperty, id);
      LoadProperty(FirstNameProperty, firstName);
      LoadProperty(LastNameProperty, lastName);
      LoadProperty(BirthdayProperty, new SmartDate(birthday));
    }
  }
}