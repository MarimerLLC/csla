using System;
using System.Linq;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class PersonList : ReadOnlyListBase<PersonList, PersonInfo>
  {
    [Fetch]
    private void Fetch([Inject]DataAccess.IPersonDal dal, [Inject]IChildDataPortal<PersonInfo> dp)
    {
      IsReadOnly = false;
      var data = dal.Get().Select(d => dp.FetchChild(d));
      AddRange(data);
      IsReadOnly = true;
    }
  }
}
