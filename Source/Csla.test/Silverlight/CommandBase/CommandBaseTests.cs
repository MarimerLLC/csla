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
using System.Threading.Tasks;

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
    private const string Parameter = "test parameter";
    private const string ExpectedExecutionResult = TestCommandBase.ExecutionSignal + Parameter;


    [TestMethod]

    public async Task Asynch_Remote_call_wo_userState_passed_Results_parameters_passed_to_server_and_noException()
    {
      var command = new TestCommandBase(Parameter);
      var result = await DataPortal.ExecuteAsync<TestCommandBase>(command);
      Assert.AreEqual(ExpectedExecutionResult, result.ExecutionResult, ExecutionResultInvalidMessage);
    }
  }
}