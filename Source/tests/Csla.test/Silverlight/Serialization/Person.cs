//-----------------------------------------------------------------------
// <copyright file="Person.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace cslalighttest.Serialization
{
  [Serializable]
  public partial class Person : BusinessBase<Person>
  {
    static Person() { }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(Person),
      new PropertyInfo<string>("Name"));

    public static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
      typeof(Person),
      new PropertyInfo<DateTime>("Birthdate"));

    public static readonly PropertyInfo<AddressList> AddressesProperty = RegisterProperty<AddressList>(
      typeof(Person),
      new PropertyInfo<AddressList>("Addresses"));

    public static readonly PropertyInfo<Address> PrimaryAddressProperty = RegisterProperty<Address>(
      typeof(Person),
      new PropertyInfo<Address>("PrimaryAddress"));

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public DateTime Birthdate
    {
      get { return GetProperty<DateTime>(BirthdateProperty); }
      set { SetProperty<DateTime>(BirthdateProperty, value); }
    }

    public int Age
    {
      get { return (DateTime.Now - Birthdate).Days / 365; }
      set { Birthdate = DateTime.Now - new TimeSpan(value * 365, 0, 0, 0); }
    }

    [NonSerialized]
    private string mUnserialized = "";
    public string Unserialized
    {
      get { return mUnserialized; }
      set { mUnserialized = value; }
    }

    public AddressList Addresses 
    {
      get { return GetProperty<AddressList>(AddressesProperty); }
      set { SetProperty<AddressList>(AddressesProperty, value); }
    }

    public DateTime GetBDate()
    {
      return GetProperty<DateTime>(BirthdateProperty);
    }

    private static PropertyInfo<DateTimeOffset> DtoDateProperty = RegisterProperty<DateTimeOffset>(c => c.DtoDate, "DateTimeOffset date");
    public DateTimeOffset DtoDate
    {
      get { return GetProperty(DtoDateProperty); }
      set { SetProperty(DtoDateProperty, value); }
    }

    public Address PrimaryAddress
    {
      get { return GetProperty<Address>(PrimaryAddressProperty); }
      set { SetProperty<Address>(PrimaryAddressProperty, value); }
    }
  }
}