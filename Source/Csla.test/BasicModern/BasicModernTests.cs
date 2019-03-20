using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.BasicModern
{
  [TestClass]
  public class BasicModernTests
  {
    [TestMethod]
    public void EditLevelsWorkWithMobileFormatter()
    {
      var oldSetting = Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"];
      try
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", "MobileFormatter");

        var root = Root.NewRoot();

        var originalRootId = root.Id;

        root.BeginEdit();
        root.Id = originalRootId + 1;
        root.CancelEdit();

        Assert.AreEqual(originalRootId, root.Id);

        root.BeginEdit();
        root.Id = originalRootId + 1;
        root.ApplyEdit();

        Assert.AreEqual(originalRootId + 1, root.Id);
      }
      finally
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", oldSetting);
      }
    }

    [TestMethod]
    public void CloneWorkswithMobileFormatter()
    {
      var oldSetting = Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"];
      try
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", "MobileFormatter");

        var original = Root.NewRoot();

        original.Name = "Test Root";

        var child = original.Children.AddNew();

        child.Name = "TestChild";

        var copy = original.Clone();

        Assert.IsFalse(ReferenceEquals(original, copy));
        Assert.AreEqual(original.Id, copy.Id);
        Assert.AreEqual(original.IsDirty, copy.IsDirty);
        Assert.AreEqual(original.IsSelfDirty, copy.IsSelfDirty);
        
        for(var i = 0; i < original.Children.Count; i++)
        {
          Assert.IsFalse(ReferenceEquals(original.Children[i], copy.Children[i]));
          Assert.AreEqual(original.Children[i].Name, copy.Children[i].Name);
          Assert.AreEqual(original.Children[i].IsDirty, copy.Children[i].IsDirty);
        }
      }
      finally
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", oldSetting);
      }
    }

    [TestMethod]
    public void CreateGraph()
    {
      var graph = Root.NewRoot();
      Assert.IsTrue(graph.IsNew, "IsNew");
      Assert.IsFalse(graph.IsValid, "IsValid");
      Assert.AreEqual(0, graph.Children.Count, "Children count");
    }

    [TestMethod]
    public void MakeOldMetastateEvents()
    {
      var graph = Root.NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
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
      var graph = Root.NewRoot();
      graph.Name = "abc";
      graph = graph.Save();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
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
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = Root.NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
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
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = Root.NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
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
      graph.PropertyChanged += (o, e) =>
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
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = Root.NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };
      graph.Name = "abc";
      changed.Clear();
      graph.Children.Add(Csla.DataPortal.FetchChild<Child>(123, "xyz"));

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsFalse(changed.Contains("Children"), "Children after add");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after add");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty after add");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after add");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid after add");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after add");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after add");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after add");

      graph = graph.Save();
      changed.Clear();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      Assert.IsFalse(graph.IsDirty, "IsDirty should be false");

      graph.Children[0].Name = "mnop";

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
  }
}
