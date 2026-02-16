using Csla;

namespace BusinessLibrary
{
  [CslaImplementProperties]
  public partial class PersonInfo : ReadOnlyBase<PersonInfo>
  {
    public partial int Id { get; private set; }

    public partial string Name { get; private set; }

    [Create, RunLocal]
    private void Create() { }

    [Fetch]
    private void Fetch(int id, [Inject]DataAccess.IPersonDal dal)
    {
      var data = dal.Get(id);
      Fetch(data);
    }

    [FetchChild]
    private void Fetch(DataAccess.PersonEntity data)
    {
      Id = data.Id;
      Name = data.Name;
    }
  }
}