﻿using Csla.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.TestHelpers;

namespace Csla.Test.CommandBase
{
  /// <summary>
  /// Summary description for CommandBaseTest
  /// </summary>
  [TestClass]
  public class CommandBaseTest : Csla.Server.ObjectFactory
  {
    public CommandBaseTest(ApplicationContext applicationContext) : base(applicationContext)
    {
    }

    private TestDIContext _testDIContext = TestDIContextFactory.CreateDefaultContext();

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext { get; set; }

    [ClassInitialize()]
    public static void ClassInitialize(TestContext testContext) 
    { 

    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void CommandBase_AssertDefaultValues()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      var cmd = dataPortal.Create();

      Assert.AreEqual(string.Empty, cmd.Name);
      Assert.AreEqual(0, cmd.Num);
    }

    [TestMethod]
    public void CommandBase_CanLoadProperties_WithObjectFactory()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      var cmd = dataPortal.Create();

      LoadProperty(cmd, CommandObject.NameProperty, "Rocky");
      LoadProperty(cmd, CommandObject.NumProperty, 8);

      Assert.AreEqual("Rocky", cmd.Name);
      Assert.AreEqual(8, cmd.Num);
    }


    [TestMethod]
    public void CommandBase_CanLoadPropertiesUsingNonGenericPropertyInfo_WithObjectFactory()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      var cmd = dataPortal.Create();

      IPropertyInfo nameProperty = CommandObject.NameProperty;
      IPropertyInfo numProperty = CommandObject.NumProperty;

      LoadProperty(cmd, nameProperty, "Rocky");
      LoadProperty(cmd, numProperty, 8);

      Assert.AreEqual("Rocky", cmd.Name);
      Assert.AreEqual(8, cmd.Num);
    }


    [TestMethod]
    public void CommandBase_CanReadProperties_WithObjectFactory()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      var cmd = dataPortal.Create();

      LoadProperty(cmd, CommandObject.NameProperty, "Rocky");
      LoadProperty(cmd, CommandObject.NumProperty, 8);

      var name = ReadProperty(cmd, CommandObject.NameProperty);
      var num = ReadProperty(cmd, CommandObject.NumProperty);

      Assert.AreEqual("Rocky", name);
      Assert.AreEqual(8, num);
    }


    [TestMethod]
    public void CommandBase_CanReadPropertiesUsingNonGenericPropertyInfo_WithObjectFactory()
    {
      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      var cmd = dataPortal.Create();

      IPropertyInfo nameProperty = CommandObject.NameProperty;
      IPropertyInfo numProperty = CommandObject.NumProperty;

      LoadProperty(cmd, nameProperty, "Rocky");
      LoadProperty(cmd, numProperty, 8);

      var name = ReadProperty(cmd, nameProperty);
      var num = ReadProperty(cmd, numProperty);

      Assert.AreEqual("Rocky", name);
      Assert.AreEqual(8, num);
    }
  }
}
