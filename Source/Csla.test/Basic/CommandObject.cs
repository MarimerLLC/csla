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

    public CommandObject()
    {
    }
  }
}