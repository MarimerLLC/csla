using System;
using Csla;

namespace DataPortalPerf
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    [Fetch]
    private void Fetch()
    { }
  }
}