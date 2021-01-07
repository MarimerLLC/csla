//-----------------------------------------------------------------------
// <copyright file="CommandObjectMissingFactoryMethod.cs" company="Marimer LLC">
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
  [Csla.Server.ObjectFactory("Csla.Test.ObjectFactory.CommandObjectFactory, Csla.Test", null, null, null, null, "ExecuteMissingMethod")]
  [Serializable]
  public class CommandObjectMissingFactoryMethod : CommandBase<CommandObjectMissingFactoryMethod>
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

      CommandObjectMissingFactoryMethod cmd = new CommandObjectMissingFactoryMethod();
      cmd.BeforeServer();
      cmd = Csla.DataPortal.Execute<CommandObjectMissingFactoryMethod>(cmd);
      cmd.AfterServer();
      return cmd.Result;
    }

    #endregion

    #region Client-side Code

    public static readonly PropertyInfo<bool> ResultProperty = RegisterProperty<CommandObjectMissingFactoryMethod, bool>(p => p.Result);
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