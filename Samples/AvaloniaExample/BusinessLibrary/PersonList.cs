using Csla;
using DataAccess;

namespace BusinessLibrary;

[Serializable]
public sealed class PersonList : ReadOnlyListBase<PersonList, PersonInfo>
{
  [Fetch]
  private void Fetch(IPersonDal dal, IChildDataPortal<PersonInfo> childPortal)
  {
    IsReadOnly = false;
    foreach (var item in dal.Get())
      Add(childPortal.FetchChild(item));
    IsReadOnly = true;
  }
}
