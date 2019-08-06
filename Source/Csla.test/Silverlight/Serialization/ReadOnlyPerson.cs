//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPerson.cs" company="Marimer LLC">
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
  public class ReadOnlyPerson : ReadOnlyBase<ReadOnlyPerson>
  {
    static ReadOnlyPerson() { }

    public static ReadOnlyPerson GetReadOnlyPerson(string personName, int year)
    {
      ReadOnlyPerson person = new ReadOnlyPerson();
      person.LoadProperty(NameProperty, personName);
      person.LoadProperty(BirthdateProperty, new DateTime(year, 1, 1));
      return person;
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyPerson), new PropertyInfo<string>("Name"));

    public static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
      typeof(ReadOnlyPerson),
      new PropertyInfo<DateTime>("Birthdate"));

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
    }

    public DateTime Birthdate
    {
      get { return GetProperty<DateTime>(BirthdateProperty); }
    }
  }
}