//-----------------------------------------------------------------------
// <copyright file="CommandObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.ObjectFactory
{
  public class CommandObjectFactory : Csla.Server.ObjectFactory
  {
    public object Execute(CommandObject command)
    {
      command.Result = true;
      return command;
    }
  }
}