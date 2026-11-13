using Csla;
using DataAccess;

namespace BusinessLibrary;

[Serializable]
public sealed partial class PersonList : ReadOnlyListBase<PersonList, PersonInfo>
{
  [Fetch]
  private void Fetch(
    [Inject] IPersonDal dal,
    [Inject] IChildDataPortal<PersonInfo> childPortal)
  {
    IsReadOnly = false;

    foreach (var item in dal.Get())
      Add(childPortal.FetchChild(item));

    IsReadOnly = true;
  }
}