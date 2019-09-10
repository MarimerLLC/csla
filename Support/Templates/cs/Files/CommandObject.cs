using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
  {
    public static bool CanExecuteCommand()
    {
      // TODO: customize to check user role
      //return Csla.ApplicationContext.User.IsInRole("Role");
      return true;
    }

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

    #region Client-side Code

    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(nameof(Result));
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

    [Execute]
    private void ExecuteCommand()
    {
      // TODO: implement code to run on server
      // and set result value(s)
      Result = true;
    }

    #endregion
  }
}
