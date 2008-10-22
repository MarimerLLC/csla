using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ : CommandBase
  {
    #region Authorization Methods

    public static bool CanExecuteCommand()
    {
      // TODO: customize to check user role
      //return Csla.ApplicationContext.User.IsInRole("Role");
      return true;
    }

    #endregion

    #region Factory Methods

    public static bool Execute()
    {
      if (!CanExecuteCommand())
        throw new System.Security.SecurityException("Not authorized to execute command");

      $safeitemname$ cmd = new $safeitemname$();
      cmd.BeforeServer();
      cmd = DataPortal.Execute<$safeitemname$>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    private $safeitemname$()
    { /* require use of factory methods */ }

    #endregion

    #region Client-side Code

    // TODO: add your own fields and properties
    bool _result;

    public bool Result
    {
      get { return _result; }
    }

    private void BeforeServer()
    {
      // TODO: implement code to run on client
      // before server is called
    }

    private void AfterServer()
    {
      // TODO: implement code to run on client
      // after server is called
    }

    #endregion

    #region Server-side Code

    protected override void DataPortal_Execute()
    {
      // TODO: implement code to run on server
      // and set result value(s)
      _result = true;
    }

    #endregion
  }
}
