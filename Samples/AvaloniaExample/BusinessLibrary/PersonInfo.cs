using Csla;

namespace BusinessLibrary;

[Serializable]
public sealed class PersonInfo : ReadOnlyBase<PersonInfo>
{
  public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
  public int Id => GetProperty(IdProperty);

  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name => GetProperty(NameProperty);

  [FetchChild]
  private void Fetch(DataAccess.PersonEntity data)
  {
    LoadProperty(IdProperty, data.Id);
    LoadProperty(NameProperty, data.Name);
  }
}
