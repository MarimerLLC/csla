//-----------------------------------------------------------------------
// <copyright file="Address.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;
using Csla;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class Address : AddressBase
  {
    public Address()
    {
      MarkAsChild();
    }
    public static readonly PropertyInfo<string> ZipCodeProperty = RegisterProperty(
      typeof(Address),
      new PropertyInfo<string>("ZipCode"));

    public string ZipCode
    {
      get { return GetProperty<string>(ZipCodeProperty); }
      set { SetProperty<string>(ZipCodeProperty, value); }
    }
  }
}