//-----------------------------------------------------------------------
// <copyright file="CommandObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.Basic
{
  [Serializable]
  public class CommandObject : Csla.CommandBase<CommandObject>
  {

    private static object locker = new object();

    public CommandObject ExecuteServerCode()
    {
      return Csla.DataPortal.Execute(this);
    }

    [Obsolete]
    public void ExecuteServerCodeAsunch(EventHandler<DataPortalResult<CommandObject>> handler)
    {
      Csla.DataPortal.BeginExecute<CommandObject>(this, handler);
    }

    [Obsolete]
    public void ExecuteServerCodeAsunch(EventHandler<DataPortalResult<CommandObject>> handler, object userState)
    {
      Csla.DataPortal.BeginExecute<CommandObject>(this, handler, userState);
    }

    private string _property = "";
    public string AProperty
    {
      get
      {
        return _property;
      }
    }

    [Execute]
    protected override void DataPortal_Execute()
    {
      lock (locker)
      {
        //Csla.ApplicationContext.GlobalContext.Add("CommandObject", "DataPortal_Execute called");
        _property = "Executed";
      }
    }

    public CommandObject()
    {
    }
  }
}