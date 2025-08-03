//-----------------------------------------------------------------------
// <copyright file="CommandObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.Basic
{
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
  {

    private static object locker = new object();

    public string AProperty { get; private set; } = "";

    [Create]
    protected void DataPortal_Create()
    {
    }

    [Execute]
    protected void DataPortal_Execute()
    {
      lock (locker)
      {
        TestResults.Add("CommandObject", "DataPortal_Execute called");
        AProperty = "Executed";
      }
    }
  }
}