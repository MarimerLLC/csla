using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.DataPortalChild
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public static Child NewChild()
    {
      return Csla.DataPortal.CreateChild<Child>();
    }

    public static Child GetChild()
    {
      return Csla.DataPortal.FetchChild<Child>();
    }

    private Child()
    {
      MarkAsChild();
    }

    private static PropertyInfo<string> DataProperty = new PropertyInfo<string>("Data");
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    public void DeleteChild()
    {
      base.MarkDeleted();
    }

    protected void Child_Create()
    {
      _status = "Created";
    }

    protected void Child_Fetch()
    {
      _status = "Fetched";
    }

    protected void Child_Insert()
    {
      _status = "Inserted";
    }

    protected void Child_Update()
    {
      _status = "Updated";
    }

    protected void Child_DeleteSelf()
    {
      _status = "Deleted";
    }
  }
}
