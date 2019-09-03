using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class PersonInfo : ReadOnlyBase<PersonInfo>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    [FetchChild]
    private void Fetch(DataAccess.PersonEntity data)
    {
      Id = data.Id;
      Name = data.Name;
    }
  }
}