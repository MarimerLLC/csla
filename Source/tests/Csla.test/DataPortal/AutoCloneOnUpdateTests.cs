//-----------------------------------------------------------------------
// <copyright file="AutoCloneOnUpdateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Configuration;
using Csla.Testing;
using Csla.TestHelpers;
using Csla.Testing.Business.BusyStatus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal;

[TestClass]
public class AutoCloneOnUpdateTests
{
  private static CslaTestHost _testHost;
  private static CslaTestHost _noCloneOnUpdateHost;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context)
  {
    _testHost = CslaTestHost.Create();
    _noCloneOnUpdateHost = CslaTestHost.Create(t => t.ConfigureCsla(opt => opt.
      DataPortal(dpo => dpo.AddClientSideDataPortal(o => o.
        AutoCloneOnUpdate = false))));
  }

  [ClassCleanup]
  public static void ClassCleanup()
  {
    _testHost?.Dispose();
    _noCloneOnUpdateHost?.Dispose();
  }

  [TestInitialize]
  public void Initialize()
  {
    TestResults.Reinitialise();
  }

  [TestMethod]
  public async Task SaveWithExceptionReturnsValidGraph()
  {
    var dataPortal = _noCloneOnUpdateHost.GetDataPortal<TestItem>();
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
