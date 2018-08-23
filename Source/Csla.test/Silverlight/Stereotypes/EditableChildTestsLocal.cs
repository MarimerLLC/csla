//-----------------------------------------------------------------------
// <copyright file="EditableChildTestsLocal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Testing.Business.EditableChildTests;
using System;
using UnitDriven;

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
    public void FetchAll_Returns_all_3_Elements_and_does_not_throw_any_Exceptions()
    {
      var context = GetContext();

      MockList.FetchAll((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(3, e.Object.Count);
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void FetchAll_Returns_Elements_with_correctly_assigned_Ids()
    {
      var context = GetContext();

      MockList.FetchAll((o, e) =>
      {
        var list = (MockList)e.Object;
        context.Assert.AreEqual(MockList.MockEditableChildId1, list[0].Id);
        context.Assert.AreEqual("Child_Fetch", list[0].DataPortalMethod);

        context.Assert.AreEqual(MockList.MockEditableChildId2, list[1].Id);
        context.Assert.AreEqual("Child_Fetch", list[1].DataPortalMethod);

        context.Assert.AreEqual(MockList.MockEditableChildId3, list[2].Id);
        context.Assert.AreEqual("Child_Fetch", list[2].DataPortalMethod);

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void FetchAll_Loads_GrandChildren_with_correct_Ids()
    {
      var context = GetContext();

      MockList.FetchAll((o, e) =>
      {
        var list = (MockList)e.Object;

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
          context.Assert.IsNotNull(child.GrandChildren);
          context.Assert.AreEqual(1, child.GrandChildren.Count);
          context.Assert.AreEqual(expectedGrandChildIds[index], child.GrandChildren[0].Id);
          context.Assert.AreEqual("Child_Fetch", child.GrandChildren[0].DataPortalMethod);
        }

        context.Assert.Success();
      });

      context.Complete();
    }

    #endregion

    #region ListWithChildren - FetchByName
    //valid names: c1, c2, c3
    [TestMethod]
    public void FetchByName_INVALID_Returns_Empty_List()
    {
      var context = GetContext();
      MockList.FetchByName("INVALID", (o, e) =>
      {
        var list = (MockList)e.Object;

        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(list);
        context.Assert.AreEqual(0, list.Count);

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void FetchByName_c2_Returns_one_Element_with_name_c2_and_correct_Id()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        var list = (MockList)e.Object;

        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(list);
        context.Assert.AreEqual(1, list.Count);
        context.Assert.AreEqual("c2", list[0].Name);

        context.Assert.Success();
      });

      context.Complete();
    }


    [TestMethod]
    public void FetchByName_c2_Returns_Element_with_correct_Id_Marked_NotNew_and_NotDirty()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        var list = (MockList)e.Object;

        context.Assert.AreEqual(MockList.MockEditableChildId2, list[0].Id);
        Assert.AreEqual("Child_Fetch", list[0].DataPortalMethod);
        context.Assert.IsFalse(list[0].IsNew);
        context.Assert.IsFalse(list[0].IsDirty);

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void FetchByName_c2_Returns_Element_with_one_GrandChild()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        var list = (MockList)e.Object;
        var child = list[0];
        context.Assert.AreEqual(1, child.GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId2, child.GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", child.GrandChildren[0].DataPortalMethod);
        context.Assert.IsFalse(child.GrandChildren[0].IsNew);
        context.Assert.IsFalse(child.GrandChildren[0].IsDirty);

        context.Assert.Success();
      });

      context.Complete();
    }

    #endregion

    #region Calling Save on non-Root element - Child/GrandChild Save() - InvalidOperationException

    [TestMethod]
    public void Save_on_EditableChild_Throws_InvalidOperationException()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        try
        {
          e.Object[0].Save();
          context.Assert.Fail();
        }
        catch (InvalidOperationException)
        {
          context.Assert.Success();
        }
      });

      context.Complete();
    }

    [TestMethod]
    public void Save_on_GrandChildList_Throws_InvalidOperationException()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        try
        {
          e.Object[0].GrandChildren.Save();
          context.Assert.Fail();
        }
        catch (InvalidOperationException)
        {
          context.Assert.Success();
        }
      });

      context.Complete();
    }

    [TestMethod]
    public void Save_on_GrandChildList_Item_Throws_InvalidOperationException()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        try
        {
          e.Object[0].GrandChildren[0].Save();
          context.Assert.Fail();
        }
        catch (InvalidOperationException)
        {
          context.Assert.Success();
        }
      });

      context.Complete();
    }

    #endregion

    #region Save()
    [TestMethod]
    public void After_Saved_FetchedList_should_contain_same_number_of_items()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        var fetchedList = (MockList)e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(fetchedList);
        context.Assert.AreEqual(1, fetchedList.Count);

        fetchedList[0].Name = "saving";
        fetchedList.Saved += (o2, e2) =>
        {
          context.Assert.IsNotNull(e2.NewObject);
          var savedList = (MockList)e2.NewObject;

          context.Assert.AreEqual(fetchedList.Count, savedList.Count);

          context.Assert.Success();
        };
        fetchedList.BeginSave();
      });

      context.Complete();

    }

    [TestMethod]
    public void After_Saved_FetchedList_Items_should_contain_same_values_and_Marked_NotDirty()
    {
      var context = GetContext();
      MockList.FetchByName("c2", (o, e) =>
      {
        var fetchedList = (MockList)e.Object;

        context.Assert.AreEqual("c2", fetchedList[0].Name);
        context.Assert.AreEqual("Child_Fetch", fetchedList[0].DataPortalMethod);

        fetchedList[0].Name = "saving";
        context.Assert.IsTrue(fetchedList[0].IsDirty);

        fetchedList.Saved += (o2, e2) =>
        {
          context.Assert.IsNotNull(e2.NewObject);
          var savedList = (MockList)e2.NewObject;

          context.Assert.AreEqual(fetchedList[0].Id, savedList[0].Id);
          context.Assert.AreEqual(fetchedList[0].Name, savedList[0].Name);

          context.Assert.AreEqual("Child_Update", savedList[0].DataPortalMethod);
          context.Assert.IsFalse(savedList[0].IsDirty);

          context.Assert.AreEqual(fetchedList[0].GrandChildren.Count, savedList[0].GrandChildren.Count);

          context.Assert.Success();
        };
        fetchedList.BeginSave();
      });

      context.Complete();
    }

    [TestMethod]
    public void After_Saved_GrandChild_should_be_NotDirty_and_should_have_same_value_as_Fetched_GrandChild()
    {
      var context = GetContext();

      MockList.FetchByName("c2", (o, e) =>
      {
        var fetchedList = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(1, e.Object.Count);
        context.Assert.AreEqual("c2", e.Object[0].Name);

        var fetchedGrandChild = fetchedList[0].GrandChildren[0];

        context.Assert.AreEqual("Child_Fetch", fetchedGrandChild.DataPortalMethod);

        fetchedGrandChild.Name = "saving";
        context.Assert.IsTrue(fetchedGrandChild.IsDirty);

        fetchedList.Saved += (o3, e3) =>
        {
          //context.Assert.IsNull(e3.Error);
          context.Assert.IsNotNull(e3.NewObject);

          var savedList = (MockList)e3.NewObject;
          var savedGrandChild = savedList[0].GrandChildren[0];

          context.Assert.AreEqual("Child_Update", savedGrandChild.DataPortalMethod);
          context.Assert.IsFalse(savedGrandChild.IsDirty);
          context.Assert.AreEqual(fetchedGrandChild.Name, savedGrandChild.Name);
          context.Assert.AreEqual(fetchedGrandChild.Id, savedGrandChild.Id);//Guids used - otherwisewe would not test for this
          context.Assert.Success();
        };

        fetchedList.BeginSave();

      });

      context.Complete();
    }

    #endregion

  }
}