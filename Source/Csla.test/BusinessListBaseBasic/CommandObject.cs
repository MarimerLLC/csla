//-----------------------------------------------------------------------
// <copyright file="CommandObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessListBaseBasic
{
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
  {

    private static object locker = new object();

    public static PropertyInfo<string> APropertyProperty = RegisterProperty<string>(o => o.AProperty);

    public string AProperty
    {
      get { return ReadProperty(APropertyProperty); }
      set { LoadProperty(APropertyProperty, value); }
    }

    [RunLocal]
    [Create]
    private void Create()
    { }

    [Execute]
    protected void DataPortal_Execute()
    {
      lock (locker)
      {
        AProperty = "Executed";
      }
    }
  }
}