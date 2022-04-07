using System;
using Csla;
using System.Threading.Tasks;

namespace BusinessLibrary
{
  [Serializable]
  public class MyExpensiveCommand : CommandBase<MyExpensiveCommand>
  {
    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(nameof(Result));
    public bool Result
    {
      get { return ReadProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public static readonly PropertyInfo<string> ResultTextProperty = RegisterProperty<string>(nameof(ResultText));
    public string ResultText
    {
      get { return ReadProperty(ResultTextProperty); }
      private set { LoadProperty(ResultTextProperty, value); }
    }

    [Create]
    [RunLocal]
    private void Create()
    { }

    [Execute]
    private void Execute()
    {
      System.Threading.Thread.Sleep(5000);
      ResultText = "We're all good";
      Result = true;
    }
  }
}
