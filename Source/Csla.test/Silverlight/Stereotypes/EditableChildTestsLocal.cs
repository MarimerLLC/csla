//-----------------------------------------------------------------------
// <copyright file="EditableChildTestsLocal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Testing.Business.EditableChildTests;
using System;
using UnitDriven;
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
  public class EditableChildTestsLocal : TestBase
  {
    #region List With Children - FetchAll

    [TestMethod]
    public async Task FetchAll_Returns_all_3_Elements_and_does_not_throw_any_Exceptions()
    {
      var result = await Csla.DataPortal.FetchAsync<MockList>();
      Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task FetchAll_Returns_Elements_with_correctly_assigned_Ids()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>();
      Assert.AreEqual(MockList.MockEditableChildId1, list[0].Id);
      Assert.AreEqual("Child_Fetch", list[0].DataPortalMethod);

      Assert.AreEqual(MockList.MockEditableChildId2, list[1].Id);
      Assert.AreEqual("Child_Fetch", list[1].DataPortalMethod);

      Assert.AreEqual(MockList.MockEditableChildId3, list[2].Id);
      Assert.AreEqual("Child_Fetch", list[2].DataPortalMethod);
    }

    [TestMethod]
    public async Task FetchAll_Loads_GrandChildren_with_correct_Ids()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>();
      var expectedGrandChildIds =
          new[]
          {
            GrandChildList.GrandChildId1,
            GrandChildList.GrandChildId2,
            GrandChildList.GrandChildId3
          };

      for (int index = 0; index < list.Count; index++)
      {
        var child = list[index];
        Assert.IsNotNull(child.GrandChildren);
        Assert.AreEqual(1, child.GrandChildren.Count);
        Assert.AreEqual(expectedGrandChildIds[index], child.GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", child.GrandChildren[0].DataPortalMethod);
      }
    }

    #endregion

    #region ListWithChildren - FetchByName
    //valid names: c1, c2, c3
    [TestMethod]
    public async Task FetchByName_INVALID_Returns_Empty_List()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("INVALID");
      Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public async Task FetchByName_c2_Returns_one_Element_with_name_c2_and_correct_Id()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      Assert.AreEqual(1, list.Count);
    }


    [TestMethod]
    public async Task FetchByName_c2_Returns_Element_with_correct_Id_Marked_NotNew_and_NotDirty()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      Assert.AreEqual(MockList.MockEditableChildId2, list[0].Id);
      Assert.AreEqual("Child_Fetch", list[0].DataPortalMethod);
      Assert.IsFalse(list[0].IsNew);
      Assert.IsFalse(list[0].IsDirty);
    }

    [TestMethod]
    public async Task FetchByName_c2_Returns_Element_with_one_GrandChild()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      var child = list[0];
      Assert.AreEqual(1, child.GrandChildren.Count);
      Assert.AreEqual(GrandChildList.GrandChildId2, child.GrandChildren[0].Id);
      Assert.AreEqual("Child_Fetch", child.GrandChildren[0].DataPortalMethod);
      Assert.IsFalse(child.GrandChildren[0].IsNew);
      Assert.IsFalse(child.GrandChildren[0].IsDirty);
    }

    #endregion

    #region Calling Save on non-Root element - Child/GrandChild Save() - InvalidOperationException

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Save_on_EditableChild_Throws_InvalidOperationException()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      list[0].Save();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Save_on_GrandChildList_Throws_InvalidOperationException()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      list[0].GrandChildren.Save();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Save_on_GrandChildList_Item_Throws_InvalidOperationException()
    {
      var list = await Csla.DataPortal.FetchAsync<MockList>("c2");
      list[0].GrandChildren[0].Save();
    }

    #endregion

    #region Save()
    [TestMethod]
    public async Task After_Saved_FetchedList_should_contain_same_number_of_items()
    {
      var fetchedList = await Csla.DataPortal.FetchAsync<MockList>("c2");
      Assert.AreEqual(1, fetchedList.Count);

      fetchedList[0].Name = "saving";
      var savedList = await fetchedList.SaveAsync();
      Assert.AreEqual(fetchedList.Count, savedList.Count);
    }

    [TestMethod]
    public async Task After_Saved_FetchedList_Items_should_contain_same_values_and_Marked_NotDirty()
    {
      var fetchedList = await Csla.DataPortal.FetchAsync<MockList>("c2");
      Assert.AreEqual("c2", fetchedList[0].Name);
      Assert.AreEqual("Child_Fetch", fetchedList[0].DataPortalMethod);

      fetchedList[0].Name = "saving";
      Assert.IsTrue(fetchedList[0].IsDirty);
      var savedList = await fetchedList.SaveAsync();
      Assert.AreEqual(fetchedList[0].Id, savedList[0].Id);
      Assert.AreEqual(fetchedList[0].Name, savedList[0].Name);

      Assert.AreEqual("Child_Update", savedList[0].DataPortalMethod);
      Assert.IsFalse(savedList[0].IsDirty);

      Assert.AreEqual(fetchedList[0].GrandChildren.Count, savedList[0].GrandChildren.Count);
    }

    [TestMethod]
    public async Task After_Saved_GrandChild_should_be_NotDirty_and_should_have_same_value_as_Fetched_GrandChild()
    {
      var fetchedList = await Csla.DataPortal.FetchAsync<MockList>("c2");
      var fetchedGrandChild = fetchedList[0].GrandChildren[0];

      Assert.AreEqual("Child_Fetch", fetchedGrandChild.DataPortalMethod);

      fetchedGrandChild.Name = "saving";
      Assert.IsTrue(fetchedGrandChild.IsDirty);

      var savedList = await fetchedList.SaveAsync();
      var savedGrandChild = savedList[0].GrandChildren[0];

      Assert.AreEqual("Child_Update", savedGrandChild.DataPortalMethod);
      Assert.IsFalse(savedGrandChild.IsDirty);
      Assert.AreEqual(fetchedGrandChild.Name, savedGrandChild.Name);
      Assert.AreEqual(fetchedGrandChild.Id, savedGrandChild.Id);//Guids used - otherwisewe would not test for this
    }

    #endregion

  }
}