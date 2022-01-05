//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPerson.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Testing.Business.ReadOnlyTest
{
  [Serializable]
  public partial class ReadOnlyPerson : ReadOnlyBase<ReadOnlyPerson>
  {
    public const string DataPortalUrl = "http://localhost:4832/WcfPortal.svc";
    
    //public ReadOnlyPerson() { }

    public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(typeof(ReadOnlyPerson), new PropertyInfo<Guid>("Id"));

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyPerson), new PropertyInfo<string>("Name"));

    public static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
      typeof(ReadOnlyPerson),
      new PropertyInfo<DateTime>("Birthdate"));

    public Guid Id
    {
      get { return GetProperty<Guid>(IdProperty); }
    }


    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
    }

    public DateTime Birthdate
    {
      get { return GetProperty<DateTime>(BirthdateProperty); }
    }

    public static ReadOnlyPerson GetReadOnlyPersonForList(string personName, int year)
    {
      ReadOnlyPerson person = new ReadOnlyPerson();
      person.LoadProperty(NameProperty, personName);
      person.LoadProperty(BirthdateProperty, new DateTime(year, 1, 1));
      return person;
    }
  }
}