using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace BlazorExample.Shared
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
