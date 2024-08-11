﻿using Csla.Configuration;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.BasicModern
{
  [TestClass]
  public class BasicModernTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o.Binding(bo => bo.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var serviceProvider = services.BuildServiceProvider();
      _testDIContext = new TestDIContext(serviceProvider);
    }

    [TestMethod]
    public void CreateGraph()
    {
      var graph = CreateRoot();
      Assert.IsTrue(graph.IsNew, "IsNew");
      Assert.IsFalse(graph.IsValid, "IsValid");
      Assert.AreEqual(0, graph.Children.Count, "Children count");
    }

    [TestMethod]
    public void MakeOldMetastateEvents()
    {
      var graph = CreateRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.MakeOld();

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty");
      Assert.IsFalse(changed.Contains("IsValid"), "IsValid");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable");
      Assert.IsTrue(changed.Contains("IsNew"), "IsNew");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted");
    }

    [TestMethod]
    public void MarkDeletedMetastateEvents()
    {
      var graph = CreateRoot();
      graph.Name = "abc";
      graph = graph.Save();
      var changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.Delete();

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty");
      Assert.IsFalse(changed.Contains("IsValid"), "IsValid");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew");
      Assert.IsTrue(changed.Contains("IsDeleted"), "IsDeleted");
    }

    [TestMethod]
    public void RootChangedMetastateEventsId()
    {
      var graph = CreateRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
        {
          changed.Add(e.PropertyName);
        };

      graph.Id = 123;

      Assert.IsTrue(changed.Contains("Id"), "Id");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted");
    }

    [TestMethod]
    public void RootChangedMetastateEventsName()
    {
      var graph = CreateRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.Name = "abc";

      Assert.IsTrue(changed.Contains("Name"), "Name");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted");

      graph = graph.Save();
      changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };

      Assert.IsFalse(graph.IsDirty, "IsDirty should be false");

      graph.Name = "def";

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

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
    public void RootChangedMetastateEventsChild()
    {
      var graph = CreateRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };
      graph.Name = "abc";
      changed.Clear();
      var child = graph.Children.AddNew();
      child.Id = 123;
      child.Name = "xyz";

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsFalse(changed.Contains("Children"), "Children after add");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after add");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty after add");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after add");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid after add");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after add");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after add");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after add");
    }

    [TestMethod]
    public void RootChangedMetastateEventsChildAfterSave()
    {
      var graph = CreateRoot();
      var changed = new List<string>();
      graph.Name = "abc";
      var child = graph.Children.AddNew();
      child.Id = 123;
      child.Name = "xyz";

      graph = graph.Save();

      changed.Clear();
      graph.PropertyChanged += (_, e) =>
      {
        changed.Add(e.PropertyName);
      };
      graph.ChildChanged += (_, e) =>
      {
        if (e.ChildObject is not null)
          changed.Add($"{e.ChildObject.GetType()}-{e.PropertyChangedArgs.PropertyName}");
      };

      graph.Children[0].Name = "mnop";

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsFalse(changed.Contains("Children"), "Children after edit");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after edit");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty after edit");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after edit");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid after edit");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after edit");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after edit");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after edit");
    }

    private Root CreateRoot()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      return dataPortal.Create();
    }
  }
}
