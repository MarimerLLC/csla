using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
  public class CommandObject : Csla.CommandBase<CommandObject>
  {

    private static object locker = new object();

    public CommandObject ExecuteServerCode()
    {
      return Csla.DataPortal.Execute(this);
    }

    public void ExecuteServerCodeAsunch(EventHandler<DataPortalResult<CommandObject>> handler)
    {
      Csla.DataPortal.BeginExecute<CommandObject>(this, handler);
    }

    public void ExecuteServerCodeAsunch(EventHandler<DataPortalResult<CommandObject>> handler, object userState)
    {
      Csla.DataPortal.BeginExecute<CommandObject>(this, handler, userState);
    }

    private string _property = "";
    public string AProperty
    {
      get
      {
        return _property;
      }
    }

    protected override void DataPortal_Execute()
    {
      lock (locker)
      {
        //Csla.ApplicationContext.GlobalContext.Add("CommandObject", "DataPortal_Execute called");
        _property = "Executed";
      }
    }

    public CommandObject()
    {
    }
  }
}
