using System;
using Csla;

namespace Csla.Test.CommandBase
{
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return ReadProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int> NumProperty = RegisterProperty<int>(c => c.Num);
    public int Num
    {
      get { return ReadProperty(NumProperty); }
      private set { LoadProperty(NumProperty, value); }
    }

    [RunLocal]
    [Create]
    private void Create()
    { }

  }
}
