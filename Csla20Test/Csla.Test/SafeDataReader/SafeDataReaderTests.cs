using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.DataBinding;
using System.Data;
using System.Data.SqlClient;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.SafeDataReader
{
    [TestClass()]
    public class SafeDataReaderTests
    {
        //pull from ConfigurationManager
        private const string CONNECTION_STRING =
            "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|DataPortalTestDatabase.mdf;Integrated Security=True;User Instance=True";

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
