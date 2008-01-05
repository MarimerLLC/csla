using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.DataPortalChild
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    public static ChildList GetList()
    {
      return Csla.DataPortal.FetchChild<ChildList>();
    }

    private ChildList()
    {
      MarkAsChild();
    }

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    protected void Child_Fetch()
    {
      _status = "Fetched";
    }

    protected override void Child_Update()
    {
      base.Child_Update();
      _status = "Updated";
    }
  }
}
