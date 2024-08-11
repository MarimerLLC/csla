﻿//-----------------------------------------------------------------------
// <copyright file="PropertyGetSetTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Serialization.Mobile;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.PropertyGetSet
{
#if TESTING
  //[System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class PropertyGetSetTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o.Binding(bo => bo.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var serviceProvider = services.BuildServiceProvider();
      _testDIContext = new TestDIContext(serviceProvider);
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
      _changedName = "";
      _changedName = "";
    }

    [TestMethod]
    public void ForceStaticInit()
    {
      IDataPortal<EditableGetSetNFI> dataPortal = _testDIContext.CreateDataPortal<EditableGetSetNFI>();

      EditableGetSetNFI root = dataPortal.Fetch();
      root.Data = "a";
      root.Base = "b";
      root.TopBase = "c";
      Assert.AreEqual("a", root.Data);
      Assert.AreEqual("b", root.Base);
      Assert.AreEqual("c", root.TopBase);
    }

    [TestMethod]
    public void NullString()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.FieldBackedString = null;
      Assert.AreEqual(string.Empty, root.FieldBackedString, "FieldBackedString should be empty");
      root.F06 = null;
      Assert.AreEqual(string.Empty, root.F06, "F06 should be empty");
      root.ManagedStringField = null;
      Assert.AreEqual(string.Empty, root.ManagedStringField, "ManagedStringField should be empty");
      root.M07 = null;
      Assert.AreEqual(string.Empty, root.M07, "M07 should be empty");
    }

    [TestMethod]
    public void NonGenericLoadProperty()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.LoadM02(123);
      Assert.AreEqual(123, root.M02);

      root.LoadInternalAndPrivate("Test");
      Assert.AreEqual("Test", root.M08);

      IDataPortal<Command> commandDataPortal = _testDIContext.CreateDataPortal<Command>();
      var cmd = commandDataPortal.Create();
      cmd.Load("abc");
      Assert.AreEqual("abc", cmd.Name);

      IDataPortal<ReadOnly> roDataPortal = _testDIContext.CreateDataPortal<ReadOnly>();
      var ro = roDataPortal.Fetch();
      ro.Load("abc");
      Assert.AreEqual("abc", ro.Name);
    }

    [TestMethod]
    public void ExplicitFieldProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.PropertyChanging += root_PropertyChanging; 
      root.PropertyChanged += root_PropertyChanged;
      Assert.AreEqual("n/a", root.F03, "Default value should have been set");
      Assert.AreEqual("", root.FieldBackedString, "String should default to string.Empty");
      Assert.AreEqual(0, root.F02, "Numeric should default to 0");
      Assert.AreEqual(false, root.F05, "bool should default to false");

      root.FieldBackedString = "hi there";
      Assert.AreEqual("hi there", root.FieldBackedString, "String should have been set");

      Assert.AreEqual("FieldBackedString", _changingName, "FieldBackedString should have been changing");

      Assert.AreEqual("FieldBackedString", _changedName, "FieldBackedString should have changed");

      root.F02 = 123;
      Assert.AreEqual(123, root.F02, "Numeric should have been set");

      root.F05 = true;
      Assert.AreEqual(true, root.F05, "bool should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.PropertyChanging -= root_PropertyChanging;
      root.PropertyChanged -= root_PropertyChanged;
    }

    [TestMethod]
    public void SerializedExplicitFieldProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root = root.Clone();
      Assert.AreEqual("n/a", root.F03, "Default value should have been set");
      Assert.AreEqual("", root.FieldBackedString, "String should default to string.Empty");
      Assert.AreEqual(0, root.F02, "Numeric should default to 0");

      root.FieldBackedString = "hi there";
      root = root.Clone();
      Assert.AreEqual("hi there", root.FieldBackedString, "String should have been set");

      root.F02 = 123;
      Assert.AreEqual(123, root.F02, "Numeric should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void ManagedFieldProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.PropertyChanging += root_PropertyChanging;
      root.PropertyChanged += root_PropertyChanged;
      Assert.AreEqual("n/a", root.M03, "Default value should have been set");
      Assert.AreEqual("", root.ManagedStringField, "String should default to string.Empty");
      Assert.AreEqual(0, root.M02, "Numeric should default to 0");
      Assert.AreEqual(false, root.M05, "bool should default to false");

      root.ManagedStringField = "hi there";
      Assert.AreEqual("hi there", root.ManagedStringField, "String should have been set");
      Assert.AreEqual("ManagedStringField", _changingName, "ManagedStringField should have been changing");
      Assert.AreEqual("ManagedStringField", _changedName, "ManagedStringField should have changed");

      _changedName = string.Empty;
      _changingName = string.Empty;

      root.ManagedStringField = "hi there";
      Assert.AreEqual("hi there", root.ManagedStringField, "String should be the same");
      Assert.AreEqual(string.Empty, _changingName, "ManagedStringField should not have been changing");

      Assert.AreEqual(string.Empty, _changedName, "ManagedStringField should not have changed");

      root.M02 = 123;
      Assert.AreEqual(123, root.M02, "Numeric should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.PropertyChanging -= root_PropertyChanging;

      root.PropertyChanged -= root_PropertyChanged;
    }

    [TestMethod]
    public void ManagedFieldBaseProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);

      root.PropertyChanging += root_PropertyChanging;

      root.PropertyChanged += root_PropertyChanged;
      Assert.AreEqual("", root.Base, "String should default to string.Empty");
      Assert.AreEqual("", root.TopBase, "TopBase should default to string.Empty");

      root.Base = "hi there";
      Assert.AreEqual("hi there", root.Base, "String should have been set");

      Assert.AreEqual("Base", _changingName, "Base should have been changing");

      Assert.AreEqual("Base", _changedName, "Base should have changed");

      root.TopBase = "hi there";
      Assert.AreEqual("hi there", root.TopBase, "TopBase should have been set");

      Assert.AreEqual("TopBase", _changingName, "TopBase should have been changing");

      Assert.AreEqual("TopBase", _changedName, "TopBase should have changed");

      root.M05 = true;
      Assert.AreEqual(true, root.M05, "bool should have been set");


      root.PropertyChanging -= root_PropertyChanging;

      root.PropertyChanged -= root_PropertyChanged;
    }

    [TestMethod]
    public void SerializedManagedFieldProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root = root.Clone();
      Assert.AreEqual("n/a", root.M03, "Default value should have been set");
      Assert.AreEqual("", root.ManagedStringField, "String should default to string.Empty");
      Assert.AreEqual(0, root.M02, "Numeric should default to 0");

      root.ManagedStringField = "hi there";
      root = root.Clone();
      Assert.AreEqual("hi there", root.ManagedStringField, "String should have been set");

      root.M02 = 123;
      root = root.Clone();
      Assert.AreEqual(123, root.M02, "Numeric should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void MarkClean()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);

      root.ManagedStringField = "hi there";
      root.FieldBackedString = "hi there";
      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.MarkClean();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    public void SmartDateProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual("", root.F04, "Field should default to string.Empty");
      Assert.AreEqual("", root.M04, "Should default to string.Empty");

      root.F04 = new DateTime(1998, 12, 21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.F04, "Field SmartDate should have been set");

      root.M04 = new DateTime(1998,12,21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.M04, "SmartDate should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void SimpleChildProperties_LazyLoadedChild()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);

      root.PropertyChanging += root_PropertyChanging;

      root.PropertyChanged += root_PropertyChanged;
      
      EditableGetSet child = root.ManagedChild;
      Assert.IsNotNull(child, "Child should not be null");

      Assert.AreEqual("ManagedChild", _changingName, "ManagedChild should have been changing");

      Assert.AreEqual("ManagedChild", _changedName, "ManagedChild should have changed");

      _changingName = "";

      _changedName = "";

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(child.IsDirty, "Child should be dirty");


      child.PropertyChanging += root_PropertyChanging;

      child.PropertyChanged += root_PropertyChanged;
      child.FieldBackedString = "hi there";

      Assert.AreEqual("FieldBackedString", _changingName, "ManagedChild should NOT have been changing");
      Assert.AreEqual(false, ("ManagedChild" == _changedName), "ManagedChild should have changed");


      root.PropertyChanging -= root_PropertyChanging;
      child.PropertyChanging -= root_PropertyChanging;

      root.PropertyChanged -= root_PropertyChanged;
      child.PropertyChanged -= root_PropertyChanged;
    }

    [TestMethod]
    public void SerializedSimpleChildProperties()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      EditableGetSet child = root.ManagedChild;
      child.FieldBackedString = "hi there";

      root = root.Clone();

      root.PropertyChanging += root_PropertyChanging;

      root.PropertyChanged += root_PropertyChanged;

      child = root.ManagedChild;
      Assert.IsNotNull(child, "Child should not be null");
      Assert.AreEqual("hi there", child.FieldBackedString, "Child value should be intact");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(child.IsDirty, "Child should be dirty");


      _changingName = "";
      child.PropertyChanging += root_PropertyChanging;

      _changedName = "";
      child.PropertyChanged += root_PropertyChanged;
      child.FieldBackedString = "I've been cloned!";

      Assert.AreEqual("FieldBackedString", _changingName, "ManagedChild should NOT have been changing");
      Assert.AreEqual(false, ("ManagedChild" == _changedName), "ManagedChild should have changed");



      child.PropertyChanging -= root_PropertyChanging;
      root.PropertyChanging -= root_PropertyChanging;

      child.PropertyChanged -= root_PropertyChanged;
      root.PropertyChanged -= root_PropertyChanged;
    }

    [TestMethod]
    public void RootUndoCancel()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);

      Assert.IsFalse(root.IsDirty, "Root should not start dirty");

      Assert.AreEqual("", root.FieldBackedString, "Explicit String should default to string.Empty");
      Assert.AreEqual("", root.ManagedStringField, "Managed String should default to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after defaults load");

      root.BeginEdit();
      root.FieldBackedString = "fieldBackedString";
      root.ManagedStringField = "ManagedStringField";
      Assert.AreEqual("fieldBackedString", root.FieldBackedString, "String should be fieldBackedString");
      Assert.AreEqual("ManagedStringField", root.ManagedStringField, "String should be ManagedStringField");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.CancelEdit();
      Assert.AreEqual("", root.FieldBackedString, "Explicit String should revert to string.Empty");
      Assert.AreEqual("", root.ManagedStringField, "Managed String should revert to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void RootUndoApply()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);

      Assert.IsFalse(root.IsDirty, "Root should not start dirty");

      Assert.AreEqual("", root.FieldBackedString, "Explicit String should default to string.Empty");
      Assert.AreEqual("", root.ManagedStringField, "Managed String should default to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after defaults load");

      root.BeginEdit();
      root.FieldBackedString = "fieldBackedString";
      root.ManagedStringField = "ManagedStringField";
      Assert.AreEqual("fieldBackedString", root.FieldBackedString, "String should be fieldBackedString");
      Assert.AreEqual("ManagedStringField", root.ManagedStringField, "String should be ManagedStringField");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.ApplyEdit();
      Assert.AreEqual("fieldBackedString", root.FieldBackedString, "String should be fieldBackedString after apply");
      Assert.AreEqual("ManagedStringField", root.ManagedStringField, "String should be ManagedStringField after apply");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after ApplyEdit");
      Assert.IsTrue(root.IsValid, "Root should be valid (no validation rules exist)");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
    }

    [TestMethod]
    public void RootChildUndoCancel()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      EditableGetSet initialChild = root.ManagedChild;
      Assert.AreEqual(1, initialChild.EditLevel, "Child edit level after being created");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.CancelEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after CancelEdit");
      EditableGetSet secondChild = root.ManagedChild;
      Assert.AreEqual(0, secondChild.EditLevel, "Second child edit level after being created");
      Assert.IsFalse(ReferenceEquals(initialChild, secondChild), "Child objects should be different");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after second child created");
    }

    [TestMethod]
    public void SerializedEditLevel()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");
      Assert.AreEqual(1, root.ManagedChild.EditLevel, "Child edit level after BeginEdit");
      Assert.AreEqual(1, root.ManagedChildList.EditLevel, "List edit level after BeginEdit");
      root.ManagedChildList.Add(EditableGetSet.NewChildObject(childDataPortal));
      Assert.AreEqual(1, root.ManagedChildList[0].EditLevel, "List child edit level after BeginEdit");

      root = root.Clone();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after Clone");
      Assert.AreEqual(1, root.ManagedChild.EditLevel, "Child edit level after Clone");
      Assert.AreEqual(1, root.ManagedChildList.EditLevel, "List edit level after Clone");
      Assert.AreEqual(1, root.ManagedChildList[0].EditLevel, "List child edit level after Clone");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void RootChildUndoCancelIsDirty()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.BeginEdit();

      EditableGetSet initialChild = root.ManagedChild;

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(initialChild.IsDirty, "Child should be dirty");

      root.CancelEdit();

      // root.ManagedChild should be reset to null thus IsDirty should be false again.
      Assert.IsFalse(root.IsDirty, "Root should not be dirty");

      Assert.IsTrue(root.ManagedChild.IsDirty, "Child should be dirty after lazy loading");
      Assert.IsTrue(root.IsDirty, "Root should now be dirty since it lazy loaded ManagedChild");
    }

    [TestMethod]
    public void RootChildUndoApply()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      EditableGetSet initialChild = root.ManagedChild;
      Assert.AreEqual(1, initialChild.EditLevel, "Child edit level after being created");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.ApplyEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after ApplyEdit");
      EditableGetSet secondChild = root.ManagedChild;
      Assert.AreEqual(0, secondChild.EditLevel, "Second child edit level after ApplyEdit");
      Assert.IsTrue(ReferenceEquals(initialChild, secondChild), "Child objects should be the same");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after ApplyEdit");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
    }

    [TestMethod]
    public void RootChildListUndoCancel()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      ChildList list = root.ManagedChildList;
      Assert.AreEqual(1, list.EditLevel, "List edit level after being created");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(list.IsDirty, "List should not be dirty");

      list.Add(EditableGetSet.NewChildObject(childDataPortal));
      Assert.AreEqual(1, list.Count, "List count should be 1");

      root.CancelEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after CancelEdit");
      ChildList secondList = root.ManagedChildList;
      Assert.AreEqual(0, secondList.EditLevel, "Second list edit level after CancelEdit");
      Assert.IsFalse(ReferenceEquals(list, secondList), "List objects should not be the same");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after CancelEdit");
      Assert.IsFalse(secondList.IsDirty, "Second list should not be dirty");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void RootChildListUndoApply()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      var childList = root.ManagedChildList;
      Assert.AreEqual(1, childList.EditLevel, "List edit level after being created");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(childList.IsDirty, "List should not be dirty");

      childList.Add(EditableGetSet.NewChildObject(childDataPortal));
      Assert.AreEqual(1, childList.Count, "List count should be 1");

      root.ApplyEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after ApplyEdit");
      var secondChildList = root.ManagedChildList;
      Assert.AreEqual(0, secondChildList.EditLevel, "Second list edit level after ApplyEdit");
      Assert.IsTrue(ReferenceEquals(childList, secondChildList), "List objects should be the same");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after ApplyEdit");
      Assert.IsTrue(secondChildList.IsDirty, "Second list should be dirty");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
      Assert.IsFalse(root.ManagedChildList.IsDirty, "List should not be dirty after Save");
    }

    [TestMethod]
    public void LoadNullProperty()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.AreEqual(Guid.Empty, root.M06, "Guid should be null");
    }

#if !WINDOWS_PHONE
// BUG: This method throws an exception during Type Initialization which causes Visual Studio
// to crash if the debugger is attached to the emulator at the time this is run.
// https://connect.microsoft.com/VisualStudio/feedback/details/606930/consistent-visual-studio-crash-on-typeinitializationexception-in-wp7-emulator
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PropertyNotRegistered()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IDataPortal<BadGetSet> badDataPortal = _testDIContext.CreateDataPortal<BadGetSet>();

      var first = EditableGetSet.GetObject(dataPortal);
      try
      {
        var root = BadGetSet.GetObject(badDataPortal);
        var tmp = root.Id;
      }
      catch (TypeInitializationException ex)
      {
        throw ex.InnerException;
      }
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PropertyRegisteredTwice()
    {
      IDataPortal<BadGetSetTwo> dataPortal = _testDIContext.CreateDataPortal<BadGetSetTwo>();

      try
      {
        var root = BadGetSetTwo.GetObject(dataPortal);
        var tmp = root.Id;
      }
      catch (TypeInitializationException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        else
          throw;
      }
    }
#endif

    #region Event Bubbling

    private string _changingName;
    void root_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
      _changingName = e.PropertyName;
    }

    private string _changedName;
    void root_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      _changedName = e.PropertyName;
    }

    [TestMethod]
    public void FieldDirty()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      Assert.IsFalse(root.ManagedStringFieldDirty, "ManagedStringField should not be dirty");
      root.ManagedStringField = "hi there";
      Assert.IsTrue(root.ManagedStringFieldDirty, "ManagedStringField should be dirty");
    }

    [TestMethod]
    public void If_ManagedStringField_Property_Changes_ChildChanged_Event_Should_Not_Fire()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.ChildChanged += (_, _) => { throw new InvalidOperationException(); };
      root.ManagedStringField = "test";
    }

    [TestMethod]
    public void If_FieldBackedString_Property_Changes_On_ManagedChild_Then_ChildChanged_Should_Fire_On_Root_ButNot_On_ManagedChild()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      int changed = 0;

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.ChildChanged += (_, _) => { changed++; };
      root.ManagedChild.ChildChanged += (_, _) => { throw new InvalidOperationException();};
      root.ManagedChild.FieldBackedString = "changed";

      Assert.AreEqual(1, changed);
    }

    [TestMethod]
    public void If_FieldBackedString_Property_Changes_On_Item_In_ManagedChildList_Then_ChildChanged_Fires_On_Root_And_On_ManagedChildList()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();
      
      int rootChanged = 0;
      int listChanged = 0;

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.ChildChanged += (_, _) => { rootChanged++; };

      var list = root.ManagedChildList;
      list.ChildChanged += (_, _) => { listChanged++; };

      list.Add(EditableGetSet.NewChildObject(childDataPortal));
      list[0].FieldBackedString = "child change";

      Assert.AreEqual(4, rootChanged);//this event fires 4 times: lazy load of the child list, Item[], Count and property change on item in the list
      Assert.AreEqual(1, listChanged);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void If_FieldBackedString_Changes_On_GrandChild_Then_ChildChanged_Fires_On_GrandChild_Child_and_Root()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      int rootChanged = 0;
      int childListChanged = 0;
      int grandChildListChanged = 0;
      int childChanged = 0;
      int grandChildPropertyChanged = 0;

      EditableGetSet root = EditableGetSet.GetObject(dataPortal);
      root.PropertyChanged += (_, _) => { throw new InvalidOperationException(); };
      root.ChildChanged += (_, _) => { rootChanged++; };
      root.ManagedChildList.ChildChanged += (_, _) => { childListChanged++; };

      var child = EditableGetSet.NewChildObject(childDataPortal);
      child.PropertyChanged += (_, _) => { throw new InvalidOperationException(); };
      child.ChildChanged += (_, _) => { childChanged++; };
      child.ManagedChildList.ChildChanged += (_, _) => { grandChildListChanged++; };

      var grandChild = EditableGetSet.NewChildObject(childDataPortal);
      grandChild.ChildChanged += (_, _) => { throw new InvalidOperationException(); }; // ChildChange only fires when child of self changes
      grandChild.PropertyChanged += (_, _) => { grandChildPropertyChanged++; };

      root.ManagedChildList.Add(child);
      child.ManagedChildList.Add(grandChild);
      root.ManagedChildList[0].ManagedChildList[0].FieldBackedString = "child change"; // or c2.FieldBackedString = "child change";

      Assert.AreEqual(7, rootChanged);              //Child, and GrandChild lists lazy loaded + Property changed on GrandChildList Item
      Assert.AreEqual(4, childChanged);             //GrandChild lists lazy loaded + Property changed on GrandChildList Item
      Assert.AreEqual(4, childListChanged);         //GrandChild lists lazy loaded + Property changed on GrandChildList Item
      Assert.AreEqual(1, grandChildListChanged);    //Property changed on GrandChildList Item
      Assert.AreEqual(1, grandChildPropertyChanged);//Property changed on GrandChildList Item
    }

    [TestMethod]
    public void If_FieldBackedString_Property_Is_Changed_On_Child_After_CancelEdit_Then_ChildChanged_Fires_On_Root()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      var root = EditableGetSet.GetObject(dataPortal);
      var child = EditableGetSet.NewChildObject(childDataPortal);
      var grandChild = EditableGetSet.NewChildObject(childDataPortal);
      root.ManagedChildList.Add(child);
      child.ManagedChildList.Add(grandChild);

      root.BeginEdit();
      root.CancelEdit();

      int changed = 0;
      root.ChildChanged += (_, _) => { changed++;};
      child.FieldBackedString = "changed";

      Assert.AreEqual(1, changed);
    }

    [TestMethod]
    public void If_FieldBackedString_Is_Changed_On_GrandChild_List_Item_After_Root_Is_Deserialized_Then_Root_ChildChanged_Event_Fires()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IChildDataPortal<EditableGetSet> childDataPortal = _testDIContext.CreateChildDataPortal<EditableGetSet>();

      var root = EditableGetSet.GetObject(dataPortal);
      var child = EditableGetSet.NewChildObject(childDataPortal);
      var grandChild = EditableGetSet.NewChildObject(childDataPortal);
      root.ManagedChildList.Add(child);
      child.ManagedChildList.Add(grandChild);

      var applicationContext = _testDIContext.CreateTestApplicationContext();
      MemoryStream stream = new MemoryStream();
      MobileFormatter formatter = new MobileFormatter(applicationContext);
      formatter.Serialize(stream, root);
      stream.Seek(0, SeekOrigin.Begin);
      root = (EditableGetSet)formatter.Deserialize(stream);

      int changed = 0;
      root.ChildChanged += (_, _) => { changed++; };
      root.ManagedChildList[0].ManagedChildList[0].FieldBackedString = "changed";
      Assert.AreEqual(1, changed, "after MobileFormatter");

      changed = 0;
      root = root.Clone();
      root.ChildChanged += (_, _) => { changed++; };
      root.ManagedChildList[0].ManagedChildList[0].FieldBackedString = "changed again";
      Assert.AreEqual(1, changed, "after clone");
    }


    [TestMethod]
    public void LazyLoadChild_GetAfterSet()
    {
      IDataPortal<EditableGetSet> dataPortal = _testDIContext.CreateDataPortal<EditableGetSet>();
      IDataPortal<ChildList> childDataPortal = _testDIContext.CreateDataPortal<ChildList>();

      var root = EditableGetSet.GetObject(dataPortal);
      root.LazyChild = ChildList.NewObject(childDataPortal);
      var child = root.LazyChild;
      Assert.IsNotNull(child);
    }

    #endregion
  }

  [Serializable]
  public class Command : CommandBase<Command>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return ReadProperty(NameProperty); }
    }

    public void Load(string name)
    {
      LoadProperty((Core.IPropertyInfo)NameProperty, name);
    }

    [RunLocal]
    [Create]
    private void Create()
    { }

  }

  [Serializable]
  public class ReadOnly : ReadOnlyBase<ReadOnly>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return ReadProperty(NameProperty); }
    }

    public static readonly PropertyInfo<string> _originalNameProperty = RegisterProperty<string>(c => c.OriginalName);
    internal string OriginalName
    {
      get { return ReadProperty(_originalNameProperty); }
    }

    public static readonly PropertyInfo<string> _originalNamePrivateProperty = RegisterProperty<string>(c => c.OriginalNamePrivate);
    private string OriginalNamePrivate
    {
      get { return ReadProperty(_originalNamePrivateProperty); }
    }

    public void Load(string name)
    {
      LoadProperty((Core.IPropertyInfo)NameProperty, name);
      LoadProperty((Core.IPropertyInfo)_originalNameProperty, name);
      LoadProperty((Core.IPropertyInfo)_originalNamePrivateProperty, name);
    }

    [Fetch]
    private void Fetch()
    { }
  }
}