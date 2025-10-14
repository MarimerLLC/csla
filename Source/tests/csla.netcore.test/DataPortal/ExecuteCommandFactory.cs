//-----------------------------------------------------------------------
// <copyright file="ExecuteCommandFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla.Server;

namespace Csla.Test.DataPortal
{
  public class ExecuteCommandFactory : ObjectFactory
  {
    public ExecuteCommandFactory(ApplicationContext applicationContext) : base(applicationContext)
    { }

    [Create]
    public ExecuteCommandViaFactory Create() 
    { 
      var cmd = ApplicationContext.CreateInstanceDI<ExecuteCommandViaFactory>();
      MarkNew(cmd);
      return cmd;
    }

    /// <summary>
    /// Round-trip execute implementation
    /// </summary>
    [Execute]
    public ExecuteCommandViaFactory Execute(ExecuteCommandViaFactory cmd)
    {
      cmd.Value += ".";
      return cmd;
    }

    ///// <summary>
    ///// Execute with parameters (like a Fetch operation)
    ///// </summary>
    ///// <param name="text"></param>
    //[Execute]
    //public ExecuteCommandViaFactory Execute(string text)
    //{
    //  var cmd = ApplicationContext.CreateInstanceDI<ExecuteCommandViaFactory>();
    //  cmd.Value = text;
    //  return cmd;
    //}
  }
}
