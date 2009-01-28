using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Diagnostics;

namespace Csla.Test.Windows
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  [Serializable()]
  public class EditablePerson : BusinessBase<EditablePerson>
  {
#if SILVERLIGHT
    public EditablePerson()
#else
    private EditablePerson()
#endif
    {
      LoadProperty(FirstNameProperty, "John");
      LoadProperty(LastNameProperty, "Doe");
      LoadProperty(MiddleNameProperty, "A");
      LoadProperty(PlaceOfBirthProperty, "New York");
    }
    public static EditablePerson GetEditablePerson()
    {
      return new EditablePerson();
    }

    public static readonly PropertyInfo<string> FirstNameProperty = RegisterProperty(new PropertyInfo<string>("FirstName", "FirstName"));
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty(new PropertyInfo<string>("LastName", "LastName"));
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<string> MiddleNameProperty = RegisterProperty(new PropertyInfo<string>("MiddleName", "MiddleName"));
    public string MiddleName
    {
      get { return GetProperty(MiddleNameProperty); }
      set { SetProperty(MiddleNameProperty, value); }
    }

    public static readonly PropertyInfo<string> PlaceOfBirthProperty = RegisterProperty(new PropertyInfo<string>("PlaceOfBirth", "PlaceOfBirth"));
    public string PlaceOfBirth
    {
      get { return GetProperty(PlaceOfBirthProperty); }
      set { SetProperty(PlaceOfBirthProperty, value); }
    }

    public static readonly PropertyInfo<int> AuthLevelProperty = RegisterProperty(new PropertyInfo<int>("AuthLevel", "AuthLevel"));
    public int AuthLevel
    {
      get { return GetProperty(AuthLevelProperty); }
      set { SetProperty(AuthLevelProperty, value); }
    }

    public override bool CanReadProperty(string propertyName)
    {
      if (propertyName == FirstNameProperty.Name)
      {
        return ReadProperty(AuthLevelProperty) > 0;
      }

      return base.CanReadProperty(propertyName);
    }


    public override bool CanWriteProperty(string propertyName)
    {
      if (propertyName == FirstNameProperty.Name)
      {
        return ReadProperty(AuthLevelProperty) > 1;
      }

      return base.CanWriteProperty(propertyName);
    }

  }
}
