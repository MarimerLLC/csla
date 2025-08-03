//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a test business object using CSLA managed properties backed by</summary>
//-----------------------------------------------------------------------
using System;
using Csla;
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace cslalighttest.Serialization
{
  /// <summary>
  /// Implementation of a test business object using CSLA managed properties backed by
  /// fields, and custom serialization logic (requirement of backing managed properties
  /// with fields).
  /// </summary>
  [Serializable()]
  public sealed class Customer : BusinessBase<Customer>, IEquatable<Customer>
  {
    #region Constructors

    public Customer()
    {
      this.PrimaryContact = new CustomerContact();
      this.AccountsPayableContact = new CustomerContact();
    }

    #endregion

    #region System.Object Method Overrides

    public override bool Equals(object obj)
    {
      return Equals(obj as Customer);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;

      if (Name != null) hashCode ^= Name.GetHashCode();
      if (PrimaryContact != null) hashCode ^= PrimaryContact.GetHashCode();
      if (AccountsPayableContact != null) hashCode ^= AccountsPayableContact.GetHashCode();

      return hashCode;
    }

    #endregion

    #region IEquatable<Customer> Members

    public bool Equals(Customer other)
    {
      if (other != null)
      {
        return (StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name) &&
          CustomerContact.Equals(PrimaryContact, other.PrimaryContact) &&
          CustomerContact.Equals(AccountsPayableContact, other.AccountsPayableContact));
      }
      else
      {
        return false;
      }
    }

    #endregion

    #region Custom Serialization Methods

    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      info.AddChild("PrimaryContact", formatter.SerializeObject(PrimaryContact).ReferenceId);
      info.AddChild("AccountsPayableContact", formatter.SerializeObject(AccountsPayableContact).ReferenceId);
    }

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      this.PrimaryContact = (CustomerContact)formatter.GetObject(info.Children["PrimaryContact"].ReferenceId);
      this.AccountsPayableContact = (CustomerContact)formatter.GetObject(info.Children["AccountsPayableContact"].ReferenceId);
    }

    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("Name", Name);
    }

    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      this.Name = info.GetValue<string>("Name");
    }

    #endregion

    #region Properties

    private static PropertyInfo<string> Property_Name = RegisterProperty<string>(c => c.Name, RelationshipTypes.PrivateField);
    private string _name = Property_Name.DefaultValue;

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string Name
    {
      get { return GetProperty(Property_Name, _name); }
      set { SetProperty(Property_Name, ref _name, value); }
    }

    private static PropertyInfo<CustomerContact> Property_PrimaryContact = RegisterProperty<CustomerContact>(c => c.PrimaryContact, "Primary Contact", null, RelationshipTypes.PrivateField);
    private CustomerContact _primaryContact = Property_PrimaryContact.DefaultValue;

    /// <summary>
    /// Gets or sets the primary customer contact.
    /// </summary>
    public CustomerContact PrimaryContact
    {
      get { return GetProperty(Property_PrimaryContact, _primaryContact); }
      set { SetProperty(Property_PrimaryContact, ref _primaryContact, value); }
    }

    private static PropertyInfo<CustomerContact> Property_AccountsPayableContact = RegisterProperty<CustomerContact>(c => c.AccountsPayableContact, "A/P Contact", null, RelationshipTypes.PrivateField);
    private CustomerContact _accountsPayableContact = Property_AccountsPayableContact.DefaultValue;

    /// <summary>
    /// Gets or sets the accounts payable contact for the customer.
    /// </summary>
    public CustomerContact AccountsPayableContact
    {
      get { return GetProperty(Property_AccountsPayableContact, _accountsPayableContact); }
      set { SetProperty(Property_AccountsPayableContact, ref _accountsPayableContact, value); }
    }

    #endregion
  }
}