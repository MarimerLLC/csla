using System;
using Csla.Silverlight;
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

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyPerson), new PropertyInfo<string>("Name"));

    private static readonly PropertyInfo<DateTime> BirthdateProperty = RegisterProperty<DateTime>(
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
