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
using Csla;

namespace Csla.Test.ObjectFactory
{
  [Csla.Server.ObjectFactory("Csla.Test.ObjectFactory.CommandObjectFactory, Csla.Test")]
  [Serializable]
  public class CommandObject : CommandBase<CommandObject>
  {
    #region Authorization Methods

    public static bool CanExecuteCommand()
    {
      return true;
    }

    #endregion

    #region Factory Methods

    public static bool Execute()
    {
      if (!CanExecuteCommand())
        throw new Csla.Security.SecurityException("Not authorized to execute command");

      CommandObject cmd = new CommandObject();
      cmd.BeforeServer();
      cmd = Csla.DataPortal.Execute<CommandObject>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    #endregion

    #region Client-side Code

    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<CommandObject, bool>(p => p.Result);
    public bool Result
    {
      get { return ReadProperty<bool>(ResultProperty); }
      set { LoadProperty<bool>(ResultProperty, value); }
    }

    private void BeforeServer()
    {
    }

    private void AfterServer()
    {
    }

    #endregion

  }
}