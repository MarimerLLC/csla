using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.CommandBase
{
  /// <summary>
  /// Summary description for CommandBaseTest
  /// </summary>
  [TestClass]
  public class CommandBaseTest : Csla.Server.ObjectFactory
  {
    public CommandBaseTest()
    {
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
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
      var cmd = new CommandObject();

      Assert.AreEqual(string.Empty, cmd.Name);
      Assert.AreEqual(0, cmd.Num);
    }

    [TestMethod]
    public void CommandBase_CanLoadProperties_WithObjectFactory()
    {
      var cmd = new CommandObject();

      LoadProperty(cmd, CommandObject.NameProperty, "Rocky");
      LoadProperty(cmd, CommandObject.NumProperty, 8);

      Assert.AreEqual("Rocky", cmd.Name);
      Assert.AreEqual(8, cmd.Num);
    }


    [TestMethod]
    public void CommandBase_CanLoadPropertiesUsingNonGenericPropertyInfo_WithObjectFactory()
    {
      var cmd = new CommandObject();

      IPropertyInfo nameProperty = (IPropertyInfo) CommandObject.NameProperty;
      IPropertyInfo numProperty = (IPropertyInfo)CommandObject.NumProperty;

      LoadProperty(cmd, nameProperty, "Rocky");
      LoadProperty(cmd, numProperty, 8);

      Assert.AreEqual("Rocky", cmd.Name);
      Assert.AreEqual(8, cmd.Num);
    }


    [TestMethod]
    public void CommandBase_CanReadProperties_WithObjectFactory()
    {
      var cmd = new CommandObject();

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
      var cmd = new CommandObject();

      IPropertyInfo nameProperty = (IPropertyInfo)CommandObject.NameProperty;
      IPropertyInfo numProperty = (IPropertyInfo)CommandObject.NumProperty;

      LoadProperty(cmd, nameProperty, "Rocky");
      LoadProperty(cmd, numProperty, 8);

      var name = ReadProperty(cmd, nameProperty);
      var num = ReadProperty(cmd, numProperty);

      Assert.AreEqual("Rocky", name);
      Assert.AreEqual(8, num);
    }
  }
}
