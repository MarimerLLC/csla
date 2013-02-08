using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
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

      CommandObject cmd = new CommandObject();
      cmd.BeforeServer();
      cmd = DataPortal.Execute<CommandObject>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    private CommandObject()
    { /* require use of factory methods */ }

    #endregion

    #region Client-side Code

    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(p => p.Result);
    public bool Result
    {
      get { return ReadProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
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
