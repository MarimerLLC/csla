//-----------------------------------------------------------------------
// <copyright file="BusinessDocumentBaseMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>
//   Tests for metastate PropertyChanged events on BusinessDocumentBase.
//   Mirrors BasicModernTests patterns. Requires Xaml PropertyChangedMode
//   and is skipped on CI server due to timing sensitivity.
// </summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BusinessDocumentBase
{
  [TestClass]
  public class BusinessDocumentBaseMetastateTests
  {
    private static TestDIContext _testDIContext = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o.Binding(bo => bo.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var serviceProvider = services.BuildServiceProvider();
      _testDIContext = new TestDIContext(serviceProvider);
    }

    private MetastateDocument NewDocument()
    {
      var portal = _testDIContext.CreateDataPortal<MetastateDocument>();
      return portal.Create();
    }

    #region MakeOld

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void MakeOldMetastateEvents()
    {
      var doc = NewDocument();
      var changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      doc.MakeOld();

      Assert.IsTrue(changed.Contains("IsNew"), "IsNew should fire");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty should fire");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty should fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");

      Assert.IsFalse(changed.Contains("IsValid"), "IsValid should not fire");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid should not fire");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted should not fire");
    }

    #endregion

    #region MarkDeleted

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void MarkDeletedMetastateEvents()
    {
      var doc = NewDocument();
      doc.Name = "abc";
      doc = doc.Save();

      var changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      doc.Delete();

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty should fire");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty should fire");
      Assert.IsFalse(changed.Contains("IsValid"), "IsValid should not fire");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid should not fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew should not fire");
      Assert.IsTrue(changed.Contains("IsDeleted"), "IsDeleted should fire");
    }

    #endregion

    #region Property Changed Metastate

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void RootChangedMetastateEventsId()
    {
      // New doc is invalid (Name required) — setting Id (no rule) still triggers metastate events
      var doc = NewDocument();
      var changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      doc.Id = 123;

      Assert.IsTrue(changed.Contains("Id"), "Id should fire");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty should not fire (already dirty as new)");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty should not fire");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid should fire");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid should fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew should not fire");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted should not fire");
    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void RootChangedMetastateEventsName()
    {
      var doc = NewDocument();
      var changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      doc.Name = "abc";

      Assert.IsTrue(changed.Contains("Name"), "Name should fire");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty should not fire (new, already dirty)");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty should not fire");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid should fire");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid should fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew should not fire");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted should not fire");

      doc = doc.Save();
      changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      Assert.IsFalse(doc.IsDirty);

      doc.Name = "def";

      Assert.IsTrue(doc.IsDirty);

      Assert.IsTrue(changed.Contains("Name"), "Name after save");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after save");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty after save");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after save");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid after save");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after save");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after save");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after save");
    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void RootChangedMetastateEventsChild()
    {
      var childPortal = _testDIContext.CreateChildDataPortal<MetastateLineItem>();

      var doc = NewDocument();
      doc.Name = "abc";
      var changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      // Adding a child (fetched, non-dirty) propagates ChildChanged → doc fires metastate events
      doc.Add(childPortal.FetchChild());
      Assert.IsTrue(doc.IsDirty, "IsDirty should be true (doc is new+dirty)");

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty should fire after child add");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty should not fire");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid should fire");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid should not fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew should not fire");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted should not fire");

      doc = doc.Save();
      changed = new List<string>();
      doc.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

      Assert.IsFalse(doc.IsDirty);

      doc[0].Name = "modified";

      Assert.IsTrue(doc.IsDirty);

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty should fire after child modify");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty should not fire");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid should fire");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid should not fire");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable should fire");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew should not fire");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted should not fire");
    }

    #endregion
  }
}
