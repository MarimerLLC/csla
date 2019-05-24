//-----------------------------------------------------------------------
// <copyright file="TestCommandBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.CommandBase
{
  [Serializable]
  public class TestCommandBase : Csla.CommandBase<TestCommandBase>
  {
    public const string ExecutionSignal = "Executed command with parameter: ";

    public static readonly PropertyInfo<string> ParameterProperty = RegisterProperty(
      typeof(TestCommandBase),
      new PropertyInfo<string>("Parameter"));

    public static readonly PropertyInfo<string> ExecutionResultProperty = RegisterProperty(
      typeof(TestCommandBase),
      new PropertyInfo<string>("ExecutionResult"));

    public string Parameter
    {
      get { return ReadProperty(ParameterProperty); }
      protected set { LoadProperty(ParameterProperty, value); }
    }

    public string ExecutionResult
    {
      get { return ReadProperty(ExecutionResultProperty); }
      protected set { LoadProperty(ExecutionResultProperty, value); }
    }

    public TestCommandBase(string parameter) 
    { 
      Parameter = parameter; 
    }

    protected TestCommandBase() 
    { }

    protected override void DataPortal_Execute()
    {
      ExecutionResult = ExecutionSignal + Parameter;
    }
  }
}