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

#if SILVERLIGHT
    public TestCommandBase()
    { }
#else
    protected TestCommandBase() 
    { }
#endif

    public static void ExecuteCommand(string parameter, EventHandler<DataPortalResult<TestCommandBase>> handler)
    {
      var dp = new DataPortal<TestCommandBase>();
      dp.ExecuteCompleted += handler;
      dp.BeginExecute(new TestCommandBase(parameter));
    }

    public static void ExecuteCommand(string parameter, EventHandler<DataPortalResult<TestCommandBase>> handler, object userState)
    {
      var dp = new DataPortal<TestCommandBase>();
      dp.ExecuteCompleted += handler;
      dp.BeginExecute(new TestCommandBase(parameter), userState);
    }

    public static void ExecuteCommandStaticPortal(string parameter, EventHandler<DataPortalResult<TestCommandBase>> handler)
    {
      var command = new TestCommandBase(parameter);
      Csla.DataPortal.BeginExecute<TestCommandBase>(command, handler);
    }

    public static void ExecuteCommandStaticPortal(string parameter, EventHandler<DataPortalResult<TestCommandBase>> handler, object userState)
    {
      var command = new TestCommandBase(parameter);
      Csla.DataPortal.BeginExecute<TestCommandBase>(command, handler, userState);
    }

#if !SILVERLIGHT
    protected override void DataPortal_Execute()
    {
      ExecutionResult = ExecutionSignal + Parameter;
    }
#else
    public void DataPortal_Execute(LocalProxy<TestCommandBase>.CompletedHandler handler)
    {
      ExecutionResult = ExecutionSignal + Parameter;
      handler(this, null);
    }
#endif
  }
}
