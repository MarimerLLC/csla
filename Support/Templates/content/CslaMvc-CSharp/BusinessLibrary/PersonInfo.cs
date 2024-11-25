using System;
using Csla;

namespace CslaMvc.BusinessLibrary
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

    [Fetch]
    private void Fetch(int id, [Inject]CslaMvc.DataAccess.IPersonDal dal)
    {
      var data = dal.Get(id);
      Fetch(data);
    }

    [FetchChild]
    private void Fetch(CslaMvc.DataAccess.PersonEntity data)
    {
      Id = data.Id;
      Name = data.Name;
    }
  }
}