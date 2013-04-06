using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace DataBinding.Business
{
  [Serializable]
  public partial class Address : BusinessBase<Address>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id"));
    public static readonly PropertyInfo<int> CustomerIdProperty = RegisterProperty(new PropertyInfo<int>("CustomerId"));
    public static readonly PropertyInfo<string> Street1Property = RegisterProperty(new PropertyInfo<string>("Street1"));
    public static readonly PropertyInfo<string> Street2Property = RegisterProperty(new PropertyInfo<string>("Street2"));
    public static readonly PropertyInfo<string> CityProperty = RegisterProperty(new PropertyInfo<string>("City"));
    public static readonly PropertyInfo<string> StateProperty = RegisterProperty(new PropertyInfo<string>("State"));
    public static readonly PropertyInfo<string> ZipProperty = RegisterProperty(new PropertyInfo<string>("Zip"));

    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public int CustomerId
    {
      get { return GetProperty(CustomerIdProperty); }
      set { SetProperty(CustomerIdProperty, value); }
    }

    public string Street1
    {
      get { return GetProperty(Street1Property); }
      set { SetProperty(Street1Property, value); }
    }

    public string Street2
    {
      get { return GetProperty(Street2Property); }
      set { SetProperty(Street2Property, value); }
    }

    public string City
    {
      get { return GetProperty(CityProperty); }
      set { SetProperty(CityProperty, value); }
    }

    public string State
    {
      get { return GetProperty(StateProperty); }
      set { SetProperty(StateProperty, value); }
    }

    public string Zip
    {
      get { return GetProperty(ZipProperty); }
      set { SetProperty(ZipProperty, value); }
    }
  }
}
