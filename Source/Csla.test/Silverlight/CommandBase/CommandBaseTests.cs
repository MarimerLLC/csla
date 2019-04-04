//-----------------------------------------------------------------------
// <copyright file="CommandBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using UnitDriven;
using Csla.Testing.Business.CommandBase;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif



namespace cslalighttest.CommandBase
{
  [TestClass]
  public class CommandBaseTests : TestBase
  {
    private const string ExecutionResultInvalidMessage = "Execution result is not valid";
    private const string ExpectedUserState = "state";
    private const string Parameter = "test parameter";
    private const string ExpectedExecutionResult = TestCommandBase.ExecutionSignal + Parameter;


    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    
    public void Asynch_Remote_call_wo_userState_passed_Results_parameters_passed_to_server_and_noException()
    {
      var context = GetContext();

      TestCommandBase.ExecuteCommand(Parameter, (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.IsNull(e.UserState);
        context.Assert.AreEqual(ExpectedExecutionResult, e.Object.ExecutionResult, ExecutionResultInvalidMessage);

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void Asynch_Remote_call_with_userState_passed_Results_parameters_passed_to_server_with_userState_and_noException()
    {
      var context = GetContext();

      TestCommandBase.ExecuteCommand(Parameter, (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(ExpectedUserState, e.UserState);
        context.Assert.AreEqual(ExpectedExecutionResult, e.Object.ExecutionResult, ExecutionResultInvalidMessage);

        context.Assert.Success();
      }, ExpectedUserState);

      context.Complete();
    }

    [TestMethod]
    public void Synch_StaticPortal_call_Results_parameters_passed_to_server_and_noException()
    {
      TestCommandBase command = DataPortal.Execute<TestCommandBase>(new TestCommandBase(Parameter));
      Assert.AreEqual(ExpectedExecutionResult, command.ExecutionResult, "Execution result is not valid");
    }

    [TestMethod]
    public void Asynch_StaticPortal_call_wo_userState_passed_Results_parameters_passed_to_server_and_noException()
    {
      var context = GetContext();

      TestCommandBase.ExecuteCommandStaticPortal(Parameter, (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(ExpectedExecutionResult, e.Object.ExecutionResult, ExecutionResultInvalidMessage);

        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void Asynch_StaticPortal_call_with_userState_passed_Results_parameters_passed_to_server_with_userState_and_noException()
    {
      var context = GetContext();

      TestCommandBase.ExecuteCommandStaticPortal(Parameter, (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(ExpectedExecutionResult, e.Object.ExecutionResult, ExecutionResultInvalidMessage);
        context.Assert.AreEqual(ExpectedUserState, e.UserState);

        context.Assert.Success();
      }, ExpectedUserState);
      context.Complete();
    }

  }



}