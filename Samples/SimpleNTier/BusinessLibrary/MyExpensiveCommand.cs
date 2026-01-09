using System;
using Csla;
using System.Threading.Tasks;

namespace BusinessLibrary
{
  [CslaImplementProperties]
  public partial class MyExpensiveCommand : CommandBase<MyExpensiveCommand>
  {
    public partial bool Result { get; private set; }

    public partial string ResultText { get; private set; }

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
