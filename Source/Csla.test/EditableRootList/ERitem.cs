using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.EditableRootList
{
  [Serializable]
  public class ERitem : BusinessBase<ERitem>
  {
    string _data = string.Empty;
    public string Data
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _data;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_data.Equals(value))
        {
          _data = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _data;
    }

    private ERitem()
    { /* require use of factory methods */ }

    private ERitem(string data)
    {
      _data = data;
      MarkOld();
    }

    public static ERitem NewItem()
    {
      return new ERitem();
    }

    public static ERitem GetItem(string data)
    {
      return new ERitem(data);
    }

    protected override void DataPortal_Insert()
    {
      ApplicationContext.GlobalContext["DP"] = "Insert";
    }

    protected override void DataPortal_Update()
    {
      ApplicationContext.GlobalContext["DP"] = "Update";
    }

    protected override void DataPortal_DeleteSelf()
    {
      ApplicationContext.GlobalContext["DP"] = "DeleteSelf";
    }

    protected override void DataPortal_Delete(object criteria)
    {
      ApplicationContext.GlobalContext["DP"] = "Delete";
    }
  }
}
