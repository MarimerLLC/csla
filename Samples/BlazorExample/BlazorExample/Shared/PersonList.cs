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
    private void Fetch([Inject]DataAccess.IPersonDal dal)
    {
      using (LoadListMode)
      {
        var data = dal.Get().Select(d => DataPortal.FetchChild<PersonInfo>(d));
        AddRange(data);
      }
    }
  }
}
