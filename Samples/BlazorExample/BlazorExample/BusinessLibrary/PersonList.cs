using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class PersonList : ReadOnlyListBase<PersonList, PersonInfo>
  {
    [Fetch]
    private void Fetch([Inject]DataAccess.IPersonDal dal, [Inject]IChildDataPortal<PersonInfo> personInfoPortal)
    {
      using (LoadListMode)
      {
        var data = dal.Get().Select(d => personInfoPortal.FetchChild(d));
        AddRange(data);
      }
    }
  }
}
