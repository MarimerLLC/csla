//-----------------------------------------------------------------------
// <copyright file="CustomerContact.cs" company="Marimer LLC">
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
  public sealed class CustomerContact : BusinessBase<CustomerContact>, IEquatable<CustomerContact>
  {
    #region Constructors

    public CustomerContact()
    {
      // Nothing
    }

    #endregion

    #region System.Object Method Overrides

    public override bool Equals(object obj)
    {
      return Equals(obj as CustomerContact);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;

      if (FirstName != null) hashCode ^= FirstName.GetHashCode();
      if (LastName != null) hashCode ^= LastName.GetHashCode();

      return hashCode;
    }

    #endregion

    #region IEquatable<CustomerContact> Members

    public bool Equals(CustomerContact other)
    {
      if (other != null)
      {
        return (StringComparer.OrdinalIgnoreCase.Equals(FirstName, other.FirstName) &&
          StringComparer.OrdinalIgnoreCase.Equals(LastName, other.LastName));
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
      base.OnGetChildren(info, formatter);
    }

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      base.OnSetChildren(info, formatter);
    }

    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("FirstName", FirstName);
      info.AddValue("LastName", LastName);

      base.OnGetState(info, mode);
    }

    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      this.FirstName = info.GetValue<string>("FirstName");
      this.LastName = info.GetValue<string>("LastName");

      base.OnSetState(info, mode);
    }

    #endregion

    #region Properties

    private static PropertyInfo<string> Property_FirstName = RegisterProperty<string>(c => c.FirstName, "First Name", "", RelationshipTypes.PrivateField);
    private string _firstName = Property_FirstName.DefaultValue;

    /// <summary>
    /// Gets or sets the first name of the contact.
    /// </summary>
    public string FirstName
    {
      get { return GetProperty(Property_FirstName, _firstName); }
      set { SetProperty(Property_FirstName, ref _firstName, value); }
    }

    private static PropertyInfo<string> Property_LastName = RegisterProperty<string>(c => c.LastName, "Last Name", "", RelationshipTypes.PrivateField);
    private string _lastName = Property_LastName.DefaultValue;

    /// <summary>
    /// Gets or sets the last name of the contact.
    /// </summary>
    public string LastName
    {
      get { return GetProperty(Property_LastName, _lastName); }
      set { SetProperty(Property_LastName, ref _lastName, value); }
    }

    #endregion
  }
}