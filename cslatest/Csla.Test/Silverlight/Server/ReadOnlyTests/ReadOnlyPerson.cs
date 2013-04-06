using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core.FieldManager;

namespace Csla.Testing.Business.ReadOnlyTest
{
  [Serializable]
  public partial class ReadOnlyPerson : ReadOnlyBase<ReadOnlyPerson>
  {
    public const string DataPortalUrl = "http://localhost:2752/WcfPortal.svc";
    
    //public ReadOnlyPerson() { }

    private static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(typeof(ReadOnlyPerson), new PropertyInfo<Guid>("Id"));

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyPerson), new PropertyInfo<string>("Name"));

    private static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
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
