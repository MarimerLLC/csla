using Csla;

namespace BusinessLibrary
{
  [CslaImplementProperties]
  public partial class PersonInfo : ReadOnlyBase<PersonInfo>
  {
    public partial int Id { get; private set; }

    public partial string? Name { get; private set; }

    [FetchChild]
    private void Fetch(DataAccess.PersonEntity data)
    {
      LoadProperty(IdProperty, data.Id);
      LoadProperty(NameProperty, data.Name);
    }
  }
}