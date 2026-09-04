//-----------------------------------------------------------------------
// <copyright file="ObjectAdapterTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for Csla.Data.ObjectAdapter</summary>
//-----------------------------------------------------------------------

using System.Data;
using Csla.Data;
using Csla.Test.DataBinding;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataAdapter
{
  [TestClass]
  public class ObjectAdapterTests
  {
    private const string ErrorReadingValueText = "Error reading value";

    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void Fill_BusinessObject_DoesNotWriteErrorForNullProperty()
    {
      var dataPortal = _testHost.GetDataPortal<ParentEntity>();
      var entity = ParentEntity.NewParentEntity(dataPortal);
      entity.Data = "some data";
      Assert.IsNull(entity.Parent, "test precondition: root object has no parent");

      var dt = new DataTable();
      new ObjectAdapter().Fill(dt, entity);

      Assert.AreEqual(1, dt.Rows.Count);
      var row = dt.Rows[0];
      foreach (DataColumn column in dt.Columns)
      {
        var value = row[column] as string;
        Assert.IsFalse(
          value != null && value.Contains(ErrorReadingValueText),
          $"Column '{column.ColumnName}' contains an error message instead of a value: '{value}'");
      }
      Assert.AreEqual("some data", row["Data"]);
      Assert.AreEqual(entity.ID.ToString(), row["ID"]);
    }

    [TestMethod]
    public void Fill_BusinessObject_SkipsNonBrowsableProperties()
    {
      var dataPortal = _testHost.GetDataPortal<ParentEntity>();
      var entity = ParentEntity.NewParentEntity(dataPortal);

      var dt = new DataTable();
      new ObjectAdapter().Fill(dt, entity);

      Assert.IsTrue(dt.Columns.Contains("Data"));
      Assert.IsTrue(dt.Columns.Contains("ID"));
      Assert.IsFalse(dt.Columns.Contains("Parent"));
      Assert.IsFalse(dt.Columns.Contains("IsDirty"));
      Assert.IsFalse(dt.Columns.Contains("IsNew"));
      Assert.IsFalse(dt.Columns.Contains("BrokenRulesCollection"));
    }

    [TestMethod]
    public void Fill_PlainObject_NullPropertyBecomesDBNull()
    {
      var source = new PlainObject { Name = null, Age = 42, NullableNumber = null };

      var dt = new DataTable();
      new ObjectAdapter().Fill(dt, source);

      Assert.AreEqual(1, dt.Rows.Count);
      var row = dt.Rows[0];
      Assert.AreEqual(DBNull.Value, row["Name"]);
      Assert.AreEqual(DBNull.Value, row["NullableNumber"]);
      Assert.AreEqual("42", row["Age"]);
      Assert.AreEqual(DBNull.Value, row["NullField"]);
    }

    [TestMethod]
    public void Fill_BusinessList_ProducesOneRowPerItem()
    {
      var dataPortal = _testHost.GetDataPortal<ChildEntityList>();
      var list = dataPortal.Fetch(new object());

      var dt = new DataTable();
      new ObjectAdapter().Fill(dt, list);

      Assert.AreEqual(list.Count, dt.Rows.Count);
      Assert.IsFalse(dt.Columns.Contains("Parent"));
      for (var i = 0; i < list.Count; i++)
      {
        Assert.AreEqual(list[i].FirstName, dt.Rows[i]["FirstName"]);
        foreach (DataColumn column in dt.Columns)
        {
          var value = dt.Rows[i][column] as string;
          Assert.IsFalse(
            value != null && value.Contains(ErrorReadingValueText),
            $"Row {i}, column '{column.ColumnName}' contains an error message instead of a value: '{value}'");
        }
      }
    }

    private class PlainObject
    {
      public string Name { get; set; }
      public int Age { get; set; }
      public int? NullableNumber { get; set; }
      public string NullField = null;
    }
  }
}
