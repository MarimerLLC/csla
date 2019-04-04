//-----------------------------------------------------------------------
// <copyright file="PersonWIthSmartDateField.cs" company="Marimer LLC">
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
  public class PersonWIthSmartDateField : BusinessBase<PersonWIthSmartDateField>
  {
    static PersonWIthSmartDateField() { }

    public static PersonWIthSmartDateField GetPersonWIthSmartDateField(string personName, int year)
    {
      PersonWIthSmartDateField person = new PersonWIthSmartDateField();
      person.LoadProperty(NameProperty, personName);
      person.LoadProperty(BirthdateProperty, new SmartDate(new DateTime(year, 1, 1)));
      return person;
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(PersonWIthSmartDateField), new PropertyInfo<string>("Name"));

    public static readonly PropertyInfo<SmartDate> BirthdateProperty = RegisterProperty<SmartDate>(
      typeof(PersonWIthSmartDateField),
      new PropertyInfo<SmartDate>("Birthdate"));

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
    }

    public SmartDate Birthdate
    {
      get { return GetProperty<SmartDate>(BirthdateProperty); }
      set { SetProperty(BirthdateProperty, value); }
    }
  }
}