//-----------------------------------------------------------------------
// <copyright file="ExecuteCommand.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.DataPortal
{
  [Serializable]
  public class ExecuteCommand : CommandBase<ExecuteCommand>
  {
    public static readonly PropertyInfo<string> ValueProperty = RegisterProperty<string>(nameof(Value));
    public string Value
    {
      get => ReadProperty(ValueProperty);
      set => LoadProperty(ValueProperty, value);
    }

    [Create]
    private void Create() { }

    /// <summary>
    /// Round-trip execute implementation
    /// </summary>
    [Execute]
    private void Execute()
    {
      Value += ".";
    }

    /// <summary>
    /// Execute with parameters (like a Fetch operation)
    /// </summary>
    /// <param name="text"></param>
    [Execute]
    private void Execute(string text)
    {
      Value = text;
    }
  }
}
