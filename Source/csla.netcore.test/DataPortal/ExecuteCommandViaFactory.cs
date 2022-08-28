//-----------------------------------------------------------------------
// <copyright file="ExecuteCommandViaFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Server;

namespace Csla.Test.DataPortal
{
  [Serializable]
  [ObjectFactory(typeof(ExecuteCommandFactory))]
  public class ExecuteCommandViaFactory : CommandBase<ExecuteCommandViaFactory>
  {
    public static readonly PropertyInfo<string> ValueProperty = RegisterProperty<string>(nameof(Value));
    public string Value
    {
      get => ReadProperty(ValueProperty);
      set => LoadProperty(ValueProperty, value);
    }
  }
}
