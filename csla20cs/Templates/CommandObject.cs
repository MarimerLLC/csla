using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Templates
{
  [Serializable()]
  class CommandObject : CommandBase
  {
    #region Client-side Code

    bool _result;

    public bool Result
    {
      get { return _result; }
    }

    private void BeforeServer()
    {
      // implement code to run on client
      // before server is called
    }

    private void AfterServer()
    {
      // implement code to run on client
      // after server is called
    }

    #endregion

    #region Factory Methods

    public static bool TheCommand()
    {
      CommandObject cmd = new CommandObject();
      cmd.BeforeServer();
      cmd = DataPortal.Execute<CommandObject>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    private CommandObject()
    { /* require use of factory methods */ }

    #endregion

    #region Server-side Code

    protected override void DataPortal_Execute()
    {
      // implement code to run on server
      // here - and set result value(s)
      _result = true;
    }

    #endregion
  }
}
