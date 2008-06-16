using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core.FieldManager;

namespace cslalighttest.Serialization
{
  [Serializable]
  public partial class Person : BusinessBase<Person>
  {
    static Person() { }

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(Person),
      new PropertyInfo<string>("Name"));

    private static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
      typeof(Person),
      new PropertyInfo<DateTime>("Birthdate"));

    private static readonly PropertyInfo<AddressList> AddressesProperty = RegisterProperty<AddressList>(
      typeof(Person),
      new PropertyInfo<AddressList>("Addresses"));

    private static readonly PropertyInfo<Address> PrimaryAddressProperty = RegisterProperty<Address>(
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

    public Address PrimaryAddress
    {
      get { return GetProperty<Address>(PrimaryAddressProperty); }
      set { SetProperty<Address>(PrimaryAddressProperty, value); }
    }

    public override bool Equals(object theOtherPerson)
    {
      Person myOtherPerson = theOtherPerson as Person;
      if (myOtherPerson == null)
        return false;
      if (myOtherPerson.Name != this.Name)
        return false;
      if (myOtherPerson.Age != this.Age)
        return false;
      if (!this.Addresses.Equals(myOtherPerson.Addresses))
        return false;
      if ((myOtherPerson.PrimaryAddress == null) != (this.PrimaryAddress == null))
        return false;  // not same state of nullity
      if (this.PrimaryAddress != null && !this.PrimaryAddress.Equals(myOtherPerson.PrimaryAddress))
        return false;
      return this.Addresses.Equals(myOtherPerson.Addresses);
    }

    public override int GetHashCode()
    {
      return (this.Name + this.Age.ToString()).GetHashCode();
    }
  }
}
