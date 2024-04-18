﻿//-----------------------------------------------------------------------
// <copyright file="CommandObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ObjectFactory
{
  public class CommandObjectFactory : Csla.Server.ObjectFactory
  {
    public CommandObjectFactory(ApplicationContext applicationContext) : base(applicationContext)
    {
    }

    [RunLocal]
    public object Create()
    {
      return ApplicationContext.CreateInstanceDI<CommandObject>();
    }

    public object Execute(CommandObject command)
    {
      command.Result = true;
      return command;
    }
  }
}