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

    private string _property = "";
    public string AProperty
    {
      get
      {
        return _property;
      }
    }

    [Create]
    protected void DataPortal_Create()
    {
    }

    [Execute]
	protected void DataPortal_Execute()
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