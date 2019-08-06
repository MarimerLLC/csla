//-----------------------------------------------------------------------
// <copyright file="SafeDataReaderTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Csla.Test.DataBinding;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using System.Configuration;
#endif

#if DEBUG

namespace Csla.Test.SafeDataReader
{
    [TestClass()]
    public class SafeDataReaderTests
    {
      [TestInitialize]
      public void Initialize()
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
      }

    private static string CONNECTION_STRING = WellKnownValues.DataPortalTestDatabase;


        public void ClearDataBase()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = new SqlCommand("DELETE FROM Table2", cn);

            try
            {
                cn.Open();
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                cn.Close();
            }
        }

        [TestMethod()]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void CloseSafeDataReader()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            cn.Open();

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandText = "SELECT FirstName FROM Table2";

                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    Assert.AreEqual(false, dr.IsClosed);
                    dr.Close();
                    Assert.AreEqual(true, dr.IsClosed);
                }
            }
        }

        [TestMethod()]
        public void TestFieldCount()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            cn.Open();

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandText = "SELECT FirstName, LastName FROM Table2";

                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    Assert.IsTrue(dr.FieldCount > 0);
                    Assert.AreEqual(false, dr.NextResult());
                    dr.Close();
                }
                cn.Close();
            }
        }

        [TestMethod()]
        
        public void GetSchemaTable()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = cn.CreateCommand();
            DataTable dtSchema = null;
            cm.CommandText = "SELECT * FROM MultiDataTypes";
            cn.Open();

            using (cm)
            {
                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    dtSchema = dr.GetSchemaTable();
                    dr.Close();
                }
            }
            cn.Close();

            Assert.AreEqual("BIGINTFIELD", dtSchema.Rows[0][0]);
            Assert.AreEqual(typeof(System.Int64), dtSchema.Rows[0][12]);
            Assert.AreEqual(typeof(System.Byte[]), dtSchema.Rows[1][12]);
        }

        [TestMethod()]
        public void IsDBNull()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = cn.CreateCommand();
            cm.CommandText = "SELECT TEXT, BIGINTFIELD, IMAGEFIELD FROM MultiDataTypes";

            cn.Open();
            using (cm)
            {
                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    dr.Read();
                    Assert.AreEqual(true, dr.IsDBNull(2));
                    Assert.AreEqual(false, dr.IsDBNull(1));
                    dr.Close();
                }
            }
            cn.Close();
        }

    [TestMethod()]
        public void GetDataTypes()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            SqlCommand cm = cn.CreateCommand();
            cm.CommandText = 
                "SELECT BITFIELD, CHARFIELD, DATETIMEFIELD, UNIQUEIDENTIFIERFIELD, SMALLINTFIELD, INTFIELD, BIGINTFIELD, TEXT FROM MultiDataTypes";
            bool bitfield;
            char charfield;
            Csla.SmartDate datetimefield;
            Guid uniqueidentifierfield;
            System.Int16 smallintfield;
            System.Int32 intfield;
            System.Int64 bigintfield;
            System.String text;

            cn.Open();
            using (cm)
            {
                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    dr.Read();
                    bitfield = dr.GetBoolean("BITFIELD");
                    //this causes an error in vb version (char array initialized to nothing in vb version
                    //and it's initialized with new Char[1] in c# version)
                    charfield = dr.GetChar("CHARFIELD");
                    datetimefield = dr.GetSmartDate("DATETIMEFIELD");
                    uniqueidentifierfield = dr.GetGuid("UNIQUEIDENTIFIERFIELD");
                    smallintfield = dr.GetInt16("SMALLINTFIELD");
                    intfield = dr.GetInt32("INTFIELD");
                    bigintfield = dr.GetInt64("BIGINTFIELD");
                    text = dr.GetString("TEXT");
                    dr.Close();
                }
            }
            cn.Close();

            Assert.AreEqual(false, bitfield);
            Assert.AreEqual('z', charfield);
            Assert.AreEqual("12/13/2005", datetimefield.ToString());
            Assert.AreEqual("c0f92820-61b5-11da-8cd6-0800200c9a66", uniqueidentifierfield.ToString());
            Assert.AreEqual(32767, smallintfield);
            Assert.AreEqual(2147483647, intfield);
            Assert.AreEqual(92233720368547111, bigintfield);
            Assert.AreEqual("a bunch of text...a bunch of text...a bunch of text...a bunch of text...", text);
        }





        [TestMethod()]
        [ExpectedException(typeof(SqlException))]
        public void ThrowSqlException()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            cn.Open();

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandText = "SELECT FirstName FROM NonExistantTable";

                Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader());
            }
        }

        [TestMethod()]
        public void TestSafeDataReader()
        {
            List<string> list = new List<string>();

            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            cn.Open();

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandText = "SELECT Name, Date, Age FROM Table1";

                using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
                {
                    while (dr.Read()) //returns two results
                    {
                        string output = dr.GetString("Name") + ", age " + dr.GetInt32("Age") + ", added on " + dr.GetSmartDate("Date");
                        Assert.AreEqual("varchar", dr.GetDataTypeName("Name"));
                        Assert.AreEqual(false, dr.IsClosed);

                        list.Add(output);
                        Console.WriteLine(output);
                    }
                    dr.Close();
                    Assert.AreEqual(true, dr.IsClosed);
                }
                cn.Close();
            }

            Assert.AreEqual("Bill, age 56, added on 12/23/2004", list[0]);
            Assert.AreEqual("Jim, age 33, added on 1/14/2003", list[1]);
        }
    }
}
#endif