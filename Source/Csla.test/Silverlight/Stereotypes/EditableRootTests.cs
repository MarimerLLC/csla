//-----------------------------------------------------------------------
// <copyright file="EditableRootTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using Csla.Testing.Business.EditableRootTests;
using System.Diagnostics;
using Csla;
using UnitDriven;
using Csla.DataPortalClient;
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

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableRootTests : TestBase
  {
    [TestMethod]
    public void CanConstructTest()
    {
      var context = GetContext();
      var root = new MockEditableRoot();
      context.Assert.Success();
    }

    [TestMethod]
    public async Task When_CreateNew_Returns_EditableRoot_Then_returned_object_is_Marked_New_Dirty_and_NotDeleted()
    {
      var context = GetContext();
      var actual = await Csla.DataPortal.CreateAsync<MockEditableRoot>();
      context.Assert.IsNotNull(actual);
      context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
      context.Assert.IsTrue(actual.IsNew);
      context.Assert.IsTrue(actual.IsDirty);
      context.Assert.IsFalse(actual.IsDeleted);
      context.Assert.AreEqual("create", actual.DataPortalMethod);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    
    public async Task When_New_EditableRoot_is_Saved_Then_returned_object_isMarked_NotNew_NotDirty()
    {
      var context = GetContext();
      var root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, true)
      {
        Name = "justin"
      };
      var actual = await root.SaveAsync();
      context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
      context.Assert.IsFalse(actual.IsNew);
      context.Assert.IsFalse(actual.IsDirty);
      context.Assert.AreEqual("insert", actual.DataPortalMethod);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    
    public async Task When_EditableRoot_is_Saved_Then_we_receive_an_object_back_that_is_Marked_as_NotNew_NotDirty()
    {
      var context = GetContext();
      var root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Name = "justin";

      //State prior to saved
      context.Assert.IsFalse(root.IsNew);
      context.Assert.IsTrue(root.IsDirty);

      var actual = await root.SaveAsync();
      context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
      context.Assert.IsFalse(actual.IsNew);
      context.Assert.IsFalse(actual.IsDirty);
      context.Assert.AreEqual("update", actual.DataPortalMethod);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    
    public async Task If_EditableRoot_IsDeleted_Then_Saved_Returns_New_Dirty_Instance_of_Root_That_is_no_longer_marked_Deleted()
    {
      var context = GetContext();
      var root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Delete();
      //state prior to save()
      context.Assert.IsFalse(root.IsNew);
      context.Assert.IsTrue(root.IsDirty);
      context.Assert.IsTrue(root.IsDeleted);

      var actual = await root.SaveAsync();
      context.Assert.IsTrue(actual.IsNew);
      context.Assert.IsTrue(actual.IsDirty);
      context.Assert.IsFalse(actual.IsDeleted);
      context.Assert.AreEqual("delete", actual.DataPortalMethod);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public async Task When_Fetching_EditableRoot_the_object_returned_ShouldNotBe_Marked_New_Deleted_or_Dirty()
    {
      var actual = await Csla.DataPortal.FetchAsync<MockEditableRoot>(MockEditableRoot.MockEditableRootId);
      Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
      Assert.AreEqual("fetch", actual.DataPortalMethod);
      Assert.IsFalse(actual.IsNew);
      Assert.IsFalse(actual.IsDeleted);
      Assert.IsFalse(actual.IsDirty);
    }
  }
}