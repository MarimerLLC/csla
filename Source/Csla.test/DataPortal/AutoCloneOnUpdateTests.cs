//-----------------------------------------------------------------------
// <copyright file="AutoCloneOnUpdateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Configuration;
using Csla.TestHelpers;
using Csla.Testing.Business.BusyStatus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitDriven;

namespace Csla.Test.DataPortal;

[TestClass]
public class AutoCloneOnUpdateTests : TestBase
{
  private static TestDIContext _testDIContext;
  private static TestDIContext _noCloneOnUpdateDIContext;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context)
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext();
    _noCloneOnUpdateDIContext = TestDIContextFactory.CreateContext(opt => opt.
      DataPortal(dpo => dpo.ClientSideDataPortal(o => o.
        AutoCloneOnUpdate = false)));
  }

  [TestInitialize]
  public void Initialize()
  {
    TestResults.Reinitialise();
  }

  [TestMethod]
  public async Task SaveWithExceptionReturnsValidGraph()
  {
    var dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<TestItem>();
    var item = await dataPortal.CreateAsync();
    item.Name = "Test";
    try
    {
      item = await item.SaveAsync();
    }
    catch (Exception ex)
    {
      Assert.AreEqual("DataPortal.Update failed (Upsert failed)", ex.Message);
    }
    item.Name = "Test2";
  }
}

public class TestItem : BusinessBase<TestItem>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  [Create]
  [Fetch]
  [RunLocal]
  private void CreateFetch()
  {
    BusinessRules.CheckRules();
  }

  [Insert, Update]
  private void Upsert()
  {
    throw new Exception("Upsert failed");
  }
}
