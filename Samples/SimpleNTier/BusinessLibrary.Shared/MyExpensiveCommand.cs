using System;
using Csla;
using System.Threading.Tasks;

namespace BusinessLibrary
{
  [Serializable]
  public class MyExpensiveCommand : CommandBase<MyExpensiveCommand>
  {
    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(c => c.Result);
    public bool Result
    {
      get { return ReadProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public static readonly PropertyInfo<string> ResultTextProperty = RegisterProperty<string>(c => c.ResultText);
    public string ResultText
    {
      get { return ReadProperty(ResultTextProperty); }
      private set { LoadProperty(ResultTextProperty, value); }
    }

    public static async Task<MyExpensiveCommand> DoCommandAsync()
    {
      var cmd = new MyExpensiveCommand();
      cmd = await DataPortal.ExecuteAsync<MyExpensiveCommand>(cmd);
      return cmd;
    }

#if !MOBILE
    protected override void DataPortal_Execute()
    {
      System.Threading.Thread.Sleep(5000);
      ResultText = "We're all good";
      Result = true;
    }
#endif
  }
}
