//-----------------------------------------------------------------------
// <copyright file="PropertyGetSetTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using UnitDriven;
using Csla.Serialization.Mobile;
using Csla.Core;
using Csla.Serialization;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.PropertyGetSet
{
#if TESTING
  //[System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class PropertyGetSetTests
  {
    private ApplicationContext.PropertyChangedModes _mode;

    [TestInitialize]
    public void Initialize()
    {
      _mode = Csla.ApplicationContext.PropertyChangedMode;
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;
    }

    [TestCleanup]
    public void Cleanup()
    {
      Csla.ApplicationContext.PropertyChangedMode = _mode;
    }

    [TestMethod]
    public void ForceStaticInit()
    {
      EditableGetSetNFI root = new EditableGetSetNFI();
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
      EditableGetSet root = new EditableGetSet();
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
      var root = new EditableGetSet();
      root.LoadM02(123);
      Assert.AreEqual(123, root.M02);

      root.LoadInternalAndPrivate("Test");
      Assert.AreEqual("Test", root.M08);

      var cmd = new Command();
      cmd.Load("abc");
      Assert.AreEqual("abc", cmd.Name);

      var ro = new ReadOnly();
      ro.Load("abc");
      Assert.AreEqual("abc", ro.Name);
    }

    [TestMethod]
    public void ExplicitFieldProperties()
    {
      EditableGetSet root = new EditableGetSet();
      root.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging); 
      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
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

      root.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void SerializedExplicitFieldProperties()
    {
      EditableGetSet root = new EditableGetSet();
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
      EditableGetSet root = new EditableGetSet();
      root.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
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

      root.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void ManagedFieldBaseProperties()
    {
      EditableGetSet root = new EditableGetSet();

      root.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
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


      root.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void SerializedManagedFieldProperties()
    {
      EditableGetSet root = new EditableGetSet();
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
      EditableGetSet root = new EditableGetSet();

      root.ManagedStringField = "hi there";
      root.FieldBackedString = "hi there";
      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.MarkClean();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    public void SmartDateProperties()
    {
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual("", root.F04, "Field should default to string.Empty");
      Assert.AreEqual("", root.M04, "Should default to string.Empty");

      root.F04 = new DateTime(1998, 12, 21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.F04, "Field SmartDate should have been set");

      root.M04 = new DateTime(1998,12,21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.M04, "SmartDate should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void SimpleChildProperties()
    {
      EditableGetSet root = new EditableGetSet();

      root.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      
      EditableGetSet child = root.ManagedChild;
      Assert.IsNotNull(child, "Child should not be null");

      Assert.AreEqual("ManagedChild", _changingName, "ManagedChild should have been changing");

      Assert.AreEqual("ManagedChild", _changedName, "ManagedChild should have changed");

      _changingName = "";

      _changedName = "";

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(child.IsDirty, "Child should be dirty");


      child.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);

      child.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      child.FieldBackedString = "hi there";

      Assert.AreEqual("FieldBackedString", _changingName, "ManagedChild should NOT have been changing");
      Assert.AreEqual(false, ("ManagedChild" == _changedName), "ManagedChild should have changed");


      root.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);
      child.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      child.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void SerializedSimpleChildProperties()
    {
      EditableGetSet root = new EditableGetSet();
      EditableGetSet child = root.ManagedChild;
      child.FieldBackedString = "hi there";

      root = root.Clone();

      root.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);

      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);

      child = root.ManagedChild;
      Assert.IsNotNull(child, "Child should not be null");
      Assert.AreEqual("hi there", child.FieldBackedString, "Child value should be intact");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(child.IsDirty, "Child should be dirty");


      _changingName = "";
      child.PropertyChanging += new PropertyChangingEventHandler(root_PropertyChanging);

      _changedName = "";
      child.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      child.FieldBackedString = "I've been cloned!";

      Assert.AreEqual("FieldBackedString", _changingName, "ManagedChild should NOT have been changing");
      Assert.AreEqual(false, ("ManagedChild" == _changedName), "ManagedChild should have changed");



      child.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanging -= new PropertyChangingEventHandler(root_PropertyChanging);

      child.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void RootUndoCancel()
    {
      EditableGetSet root = new EditableGetSet();

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
      EditableGetSet root = new EditableGetSet();

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
      EditableGetSet root = new EditableGetSet();
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
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");
      Assert.AreEqual(1, root.ManagedChild.EditLevel, "Child edit level after BeginEdit");
      Assert.AreEqual(1, root.ManagedChildList.EditLevel, "List edit level after BeginEdit");
      root.ManagedChildList.Add(new EditableGetSet(true));
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
      EditableGetSet root = new EditableGetSet();
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
      EditableGetSet root = new EditableGetSet();
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
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      ChildList list = root.ManagedChildList;
      Assert.AreEqual(1, list.EditLevel, "List edit level after being created");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(list.IsDirty, "List should not be dirty");

      list.Add(new EditableGetSet(true));
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
      var root = new EditableGetSet();
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      var childList = root.ManagedChildList;
      Assert.AreEqual(1, childList.EditLevel, "List edit level after being created");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(childList.IsDirty, "List should not be dirty");

      childList.Add(new EditableGetSet(true));
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
      var root = new EditableGetSet();
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
      var first = new EditableGetSet();
      try
      {
        var root = new BadGetSet();
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
      try
      {
        var root = new BadGetSetTwo();
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
      EditableGetSet root = new EditableGetSet();
      Assert.IsFalse(root.ManagedStringFieldDirty, "ManagedStringField should not be dirty");
      root.ManagedStringField = "hi there";
      Assert.IsTrue(root.ManagedStringFieldDirty, "ManagedStringField should be dirty");
    }

    [TestMethod]
    public void If_ManagedStringField_Property_Changes_ChildChanged_Event_Should_Not_Fire()
    {
      var root = new EditableGetSet();
      root.ChildChanged += (o, e) => { throw new InvalidOperationException(); };
      root.ManagedStringField = "test";
    }

    [TestMethod]
    public void If_FieldBackedString_Property_Changes_On_ManagedChild_Then_ChildChanged_Should_Fire_On_Root_ButNot_On_ManagedChild()
    {
      int changed = 0;
      var root = new EditableGetSet();
      root.ChildChanged += (o, e) => { changed++; };
      root.ManagedChild.ChildChanged += (o, e) => { throw new InvalidOperationException();};
      root.ManagedChild.FieldBackedString = "changed";

      Assert.AreEqual(1, changed);
    }

    [TestMethod]
    public void If_FieldBackedString_Property_Changes_On_Item_In_ManagedChildList_Then_ChildChanged_Fires_On_Root_And_On_ManagedChildList()
    {
      int rootChanged = 0;
      int listChanged = 0;

      var root = new EditableGetSet();
      root.ChildChanged += (o, e) => { rootChanged++; };

      var list = root.ManagedChildList;
      list.ChildChanged += (o, e) => { listChanged++; };

      list.Add(new EditableGetSet(true));
      list[0].FieldBackedString = "child change";

      Assert.AreEqual(4, rootChanged);//this event fires 4 times: lazy load of the child list, Item[], Count and property change on item in the list
      Assert.AreEqual(1, listChanged);
    }

    [TestMethod]
    public void If_FieldBackedString_Changes_On_GrandChild_Then_ChildChanged_Fires_On_GrandChild_Child_and_Root()
    {
      int rootChanged = 0;
      int childListChanged = 0;
      int grandChildListChanged = 0;
      int childChanged = 0;
      int grandChildPropertyChanged = 0;

      var root = new EditableGetSet();
      root.PropertyChanged += (o, e) => { throw new InvalidOperationException(); };
      root.ChildChanged += (o, e) => { rootChanged++; };
      root.ManagedChildList.ChildChanged += (o, e) => { childListChanged++; };

      var child = new EditableGetSet(true);
      child.PropertyChanged += (o, e) => { throw new InvalidOperationException(); };
      child.ChildChanged += (o, e) => { childChanged++; };
      child.ManagedChildList.ChildChanged += (o, e) => { grandChildListChanged++; };

      var grandChild = new EditableGetSet(true);
      grandChild.ChildChanged += (o, e) => { throw new InvalidOperationException(); }; // ChildChange only fires when child of self changes
      grandChild.PropertyChanged += (o, e) => { grandChildPropertyChanged++; };

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
      var root = new EditableGetSet();
      var child = new EditableGetSet(true);
      var grandChild = new EditableGetSet(true);
      root.ManagedChildList.Add(child);
      child.ManagedChildList.Add(grandChild);

      root.BeginEdit();
      root.CancelEdit();

      int changed = 0;
      root.ChildChanged += (o, e) => { changed++;};
      child.FieldBackedString = "changed";

      Assert.AreEqual(1, changed);
    }

    [TestMethod]
    public void If_FieldBackedString_Is_Changed_On_GrandChild_List_Item_After_Root_Is_Deserialized_Then_Root_ChildChanged_Event_Fires()
    {
      var root = new EditableGetSet();
      var child = new EditableGetSet(true);
      var grandChild = new EditableGetSet(true);
      root.ManagedChildList.Add(child);
      child.ManagedChildList.Add(grandChild);

      byte[] buffer = MobileFormatter.Serialize(root);
      root = (EditableGetSet)MobileFormatter.Deserialize(buffer);

      int changed = 0;
      root.ChildChanged += (o, e) => { changed++; };
      root.ManagedChildList[0].ManagedChildList[0].FieldBackedString = "changed";
      Assert.AreEqual(1, changed, "after MobileFormatter");

      changed = 0;
      root = root.Clone();
      root.ChildChanged += (o, e) => { changed++; };
      root.ManagedChildList[0].ManagedChildList[0].FieldBackedString = "changed again";
      Assert.AreEqual(1, changed, "after clone");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void LazyLoadChild_GetBeforeSet()
    {
      var root = new EditableGetSet();
      var child = root.LazyChild;
    }

    [TestMethod]
    public void LazyLoadChild_GetAfterSet()
    {
      var root = new EditableGetSet();
      root.LazyChild = new ChildList();
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
      LoadProperty((Csla.Core.IPropertyInfo)NameProperty, name);
    }
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
      LoadProperty((Csla.Core.IPropertyInfo)NameProperty, name);
      LoadProperty((Csla.Core.IPropertyInfo)_originalNameProperty, name);
      LoadProperty((Csla.Core.IPropertyInfo)_originalNamePrivateProperty, name);
    }
  }
}