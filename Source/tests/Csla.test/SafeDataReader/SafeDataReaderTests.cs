//-----------------------------------------------------------------------
// <copyright file="SafeDataReaderTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Data;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.SafeDataReader
{
  [TestClass]
  public class SafeDataReaderTests
  {
    [TestInitialize]
    public void Initialize()
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
    }

    private static string CONNECTION_STRING => WellKnownValues.DataPortalTestDatabase;


    public void ClearDataBase()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();
      using var cm = cn.CreateCommand();
      cm.CommandText = "DELETE FROM Table2";
      cm.ExecuteNonQuery();
    }

    [TestMethod]
    public void CloseSafeDataReader()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();

      using var cm = cn.CreateCommand();
      cm.CommandText = "SELECT FirstName FROM Table2";

      using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
      {
        Assert.AreEqual(false, dr.IsClosed);
        dr.Close();
        Assert.AreEqual(true, dr.IsClosed);
      }
    }

    [TestMethod]
    public void TestFieldCount()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();

      using var cm = cn.CreateCommand();
      cm.CommandText = "SELECT FirstName, LastName FROM Table2";

      using (var dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
      {
        Assert.IsTrue(dr.FieldCount > 0);
        Assert.AreEqual(false, dr.NextResult());
        dr.Close();
      }
    }

    [TestMethod]
    public void GetSchemaTable()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();
      using var cm = cn.CreateCommand();
      DataTable dtSchema = null;
      cm.CommandText = "SELECT * FROM MultiDataTypes";

      using (var dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
      {
        dtSchema = dr.GetSchemaTable();
        dr.Close();
      }

      // Verify column names via the ColumnName field
      Assert.AreEqual("BIGINTFIELD", dtSchema.Rows[0]["ColumnName"]);
    }

    [TestMethod]
    public void IsDBNull()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();
      using var cm = cn.CreateCommand();
      cm.CommandText = "SELECT TEXT, BIGINTFIELD, IMAGEFIELD FROM MultiDataTypes";

      using var dr = new Csla.Data.SafeDataReader(cm.ExecuteReader());
      dr.Read();
      Assert.AreEqual(true, dr.IsDBNull(2));
      Assert.AreEqual(false, dr.IsDBNull(1));
      dr.Close();
    }

    [TestMethod]
    public void GetDataTypes()
    {
      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();
      using var cm = cn.CreateCommand();
      cm.CommandText =
          "SELECT BITFIELD, CHARFIELD, DATETIMEFIELD, UNIQUEIDENTIFIERFIELD, SMALLINTFIELD, INTFIELD, BIGINTFIELD, TEXT FROM MultiDataTypes";
      bool bitfield;
      char charfield;
      Csla.SmartDate datetimefield;
      Guid uniqueidentifierfield;
      Int16 smallintfield;
      Int32 intfield;
      Int64 bigintfield;
      String text;

      using (var dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
      {
        dr.Read();
        bitfield = dr.GetBoolean("BITFIELD");
        charfield = dr.GetChar("CHARFIELD");
        datetimefield = dr.GetSmartDate("DATETIMEFIELD");
        uniqueidentifierfield = dr.GetGuid("UNIQUEIDENTIFIERFIELD");
        smallintfield = dr.GetInt16("SMALLINTFIELD");
        intfield = dr.GetInt32("INTFIELD");
        bigintfield = dr.GetInt64("BIGINTFIELD");
        text = dr.GetString("TEXT");
        dr.Close();
      }

      Assert.AreEqual(false, bitfield);
      Assert.AreEqual('z', charfield);
      Assert.AreEqual("12/13/2005", datetimefield.ToString());
      Assert.AreEqual("c0f92820-61b5-11da-8cd6-0800200c9a66", uniqueidentifierfield.ToString());
      Assert.AreEqual(32767, smallintfield);
      Assert.AreEqual(2147483647, intfield);
      Assert.AreEqual(92233720368547111, bigintfield);
      Assert.AreEqual("a bunch of text...a bunch of text...a bunch of text...a bunch of text...", text);
    }

    [TestMethod]
    [ExpectedException(typeof(SQLiteException))]
    public void ThrowSqlException()
    {
      var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();

      using var cm = cn.CreateCommand();
      cm.CommandText = "SELECT FirstName FROM NonExistantTable";

      Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader());
    }

    [TestMethod]
    public void TestSafeDataReader()
    {
      List<string> list = new List<string>();

      using var cn = new SQLiteConnection(CONNECTION_STRING);
      cn.Open();

      using (var cm = cn.CreateCommand())
      {
        cm.CommandText = "SELECT Name, Date, Age FROM Table1";

        using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
        {
          while (dr.Read()) //returns two results
          {
            string output = dr.GetString("Name") + ", age " + dr.GetInt32("Age") + ", added on " + dr.GetSmartDate("Date");
            Assert.AreEqual(false, dr.IsClosed);

            list.Add(output);
            Console.WriteLine(output);
          }
          dr.Close();
          Assert.AreEqual(true, dr.IsClosed);
        }
      }

      Assert.AreEqual("Bill, age 56, added on 12/23/2004", list[0]);
      Assert.AreEqual("Jim, age 33, added on 1/14/2003", list[1]);
    }
  }
}
