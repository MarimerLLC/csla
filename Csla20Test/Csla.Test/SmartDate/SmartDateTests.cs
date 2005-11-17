using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.SmartDate
{
    [TestClass()]
    public class SmartDateTests
    {
        #region Test Constructors
        [TestMethod()]
        public void TestSmartDateConstructors()
        {
            DateTime now = DateTime.Now;
            Csla.SmartDate d = new Csla.SmartDate(now);
            Assert.AreEqual(now, d.Date);

            d = new Csla.SmartDate(true);
            Assert.IsTrue(d.EmptyIsMin);
            d = new Csla.SmartDate(false);
            Assert.IsFalse(d.EmptyIsMin);

            d = new Csla.SmartDate("1/1/2005");
            Assert.AreEqual("1/1/2005", d.ToString());
            d = new Csla.SmartDate("Jan/1/2005");
            Assert.AreEqual("1/1/2005", d.ToString());
            d = new Csla.SmartDate("January-1-2005");
            Assert.AreEqual("1/1/2005", d.ToString());
            d = new Csla.SmartDate("1-1-2005");
            Assert.AreEqual("1/1/2005", d.ToString());
            d = new Csla.SmartDate("");
            Assert.AreEqual("", d.ToString());
            Assert.IsTrue(d.IsEmpty);

            d = new Csla.SmartDate("1/1/2005", true);
            Assert.AreEqual("1/1/2005", d.ToString());
            Assert.IsTrue(d.EmptyIsMin);
            d = new Csla.SmartDate("1/1/2005", false);
            Assert.AreEqual("1/1/2005", d.ToString());
            Assert.IsFalse(d.EmptyIsMin);
            d = new Csla.SmartDate("", true);
            Assert.AreEqual(DateTime.MinValue, d.Date);
            Assert.AreEqual("", d.ToString());
            d = new Csla.SmartDate("", false);
            Assert.AreEqual(DateTime.MaxValue, d.Date);
            Assert.AreEqual("", d.ToString());

            try
            {
                d = new Csla.SmartDate("Invalid Date", true);
            }
            catch (Exception ex) { Assert.IsTrue(ex is ArgumentException); }
            try
            {
                d = new Csla.SmartDate("Invalid Date", false);
            }
            catch (Exception ex) { Assert.IsTrue(ex is ArgumentException); }

            d = new Csla.SmartDate(now, true);
            Assert.AreEqual(now, d.Date);
            Assert.IsTrue(d.EmptyIsMin);
            d = new Csla.SmartDate(now, false);
            Assert.AreEqual(now, d.Date);
            Assert.IsFalse(d.EmptyIsMin);
        }
        #endregion

        #region Converters
        [TestMethod]
        public void TestConverters()
        {
            DateTime d = Csla.SmartDate.StringToDate("1/1/2005");
            Assert.AreEqual("1/1/2005", d.ToShortDateString());
            d = Csla.SmartDate.StringToDate("january-1-2005");
            Assert.AreEqual("1/1/2005", d.ToShortDateString());
            d = Csla.SmartDate.StringToDate(".");
            Assert.AreEqual(DateTime.Now.ToShortDateString(), d.ToShortDateString());
            d = Csla.SmartDate.StringToDate("-");
            Assert.AreEqual(DateTime.Now.AddDays(-1.0).ToShortDateString(), d.ToShortDateString());
            d = Csla.SmartDate.StringToDate("+");
            Assert.AreEqual(DateTime.Now.AddDays(1.0).ToShortDateString(), d.ToShortDateString());
            try
            {
                d = Csla.SmartDate.StringToDate("Invalid Date");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is System.ArgumentException);
            }

            d = Csla.SmartDate.StringToDate("");
            Assert.AreEqual(DateTime.MinValue, d);
            d = Csla.SmartDate.StringToDate(null);
            Assert.AreEqual(DateTime.MinValue, d);

            d = Csla.SmartDate.StringToDate("", true);
            Assert.AreEqual(DateTime.MinValue, d);
            d = Csla.SmartDate.StringToDate("", false);
            Assert.AreEqual(DateTime.MaxValue, d);
            try
            {
                d = Csla.SmartDate.StringToDate("Invalid Date", true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
            }
            try
            {
                d = Csla.SmartDate.StringToDate("Invalid Date", false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
            }
            d = Csla.SmartDate.StringToDate(null, true);
            Assert.AreEqual(DateTime.MinValue, d);
            d = Csla.SmartDate.StringToDate(null, false);
            Assert.AreEqual(DateTime.MaxValue, d);

            d = new DateTime(2005, 1, 2);
            string date = Csla.SmartDate.DateToString(d, "dd/MM/yyyy");
            Assert.AreEqual("02/01/2005", date);
            date = Csla.SmartDate.DateToString(d, "MM/dd/yy");
            Assert.AreEqual("01/02/05", date);
            date = Csla.SmartDate.DateToString(d, "");
            Assert.AreEqual("1/2/2005", date);

            #warning Bugs found here
            date = Csla.SmartDate.DateToString(DateTime.MinValue, "dd/MM/yyyy", true);
            Assert.AreEqual("", date);
            date = Csla.SmartDate.DateToString(DateTime.MinValue, "dd/MM/yyyy", false);
            Assert.AreEqual("", date);
            date = Csla.SmartDate.DateToString(DateTime.MaxValue, "dd/MM/yyyy", true);
            Assert.AreEqual("", date);
            date = Csla.SmartDate.DateToString(DateTime.MaxValue, "dd/MM/yyyy", false);
            Assert.AreEqual("", date);
        }
        #endregion

        #region Add
        [TestMethod()]
        public void Add()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(2005, 1, 1);
            d3 = new Csla.SmartDate(d2.Add(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, 30, d2.Date), d3.Date, "Dates should be equal");

            Assert.AreEqual(d3, d2 + new TimeSpan(30, 0, 0, 0, 0), "Dates should be equal");
        }
        #endregion

        #region Subtract
        [TestMethod()]
        public void Subtract()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(2005, 1, 1);
            d3 = new Csla.SmartDate(d2.Subtract(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, -30, d2.Date), d3.Date, "Dates should be equal");

            Assert.AreEqual(30, ((TimeSpan)(d2-d3)).Days, "Should be 30 days different");
            Assert.AreEqual(d3, d2 - new TimeSpan(30, 0, 0, 0, 0), "Should be equal");
        }
        #endregion

        #region Comparison
        [TestMethod()]
        public void Comparison()
        {
            Csla.SmartDate d2 = new Csla.SmartDate(true);
            Csla.SmartDate d3 = new Csla.SmartDate(false);

            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal");
            Assert.IsTrue(Csla.SmartDate.Equals(d2, d3), "Empty dates should be equal (shared)");
            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal (unary)");
            Assert.IsTrue(d2.Equals(""), "Should be equal to an empty string (d2)");
            Assert.IsTrue(d3.Equals(""), "Should be equal to an empty string (d3)");

            Assert.IsTrue(d2.Date.Equals(DateTime.MinValue), "Should be DateTime.MinValue");
            Assert.IsTrue(d3.Date.Equals(DateTime.MaxValue), "Should be DateTime.MaxValue");

            d2.Date = new DateTime(2005, 1, 1);
            d3 = new Csla.SmartDate(d2.Date, d2.EmptyIsMin);
            Assert.AreEqual(d2, d3, "Assigned dates should be equal");

            d3.Date = new DateTime(2005, 2, 2);
            Assert.AreEqual(1, d3.CompareTo(d2), "Should be greater than");
            Assert.AreEqual(-1, d2.CompareTo(d3), "Should be less than");
            Assert.IsFalse(d2.CompareTo(d3) == 0, "should not be equal");

            d3.Date = new DateTime(2005, 1, 1);
            Assert.IsFalse(1 == d2.CompareTo(d3), "should be equal");
            Assert.IsFalse(-1 == d2.CompareTo(d3), "should be equal");
            Assert.AreEqual(0, d2.CompareTo(d3), "should be equal");

            Assert.IsTrue(d3.Equals("1/1/2005"), "Should be equal to string date");
            Assert.IsTrue(d3.Equals(new DateTime(2005, 1, 1)), "should be equal to DateTime");
            
            Assert.IsTrue(d3.Equals(d2.Date.ToString()),"Should be equal to any date time string");
            Assert.IsTrue(d3.Equals(d2.Date.ToLongDateString()), "Should be equal to any date time string");
            Assert.IsTrue(d3.Equals(d2.Date.ToShortDateString()), "Should be equal to any date time string");
            Assert.IsFalse(d3.Equals(""), "Should not be equal to a blank string");

            //DateTime can be compared using all sorts of formats but the SmartDate cannot.
            //DateTime dt = DateTime.Now;
            //long ldt = dt.ToBinary();
            //Assert.IsTrue(dt.Equals(ldt), "Should be equal");
            //Should smart date also be converted into these various types?
        }
        #endregion

        #region Empty
        [TestMethod()]
        public void Empty()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d3 = new Csla.SmartDate(d2.Add(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(d2, d3, "Dates should be equal");
            Assert.AreEqual("", d2.Text, "Text should be empty");

            d3 = new Csla.SmartDate(d2.Subtract(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(d2, d3, "Dates should be equal");
            Assert.AreEqual("", d2.Text, "Text should be empty");

            d3 = new Csla.SmartDate();
            Assert.AreEqual(0, d2.CompareTo(d3), "d2 and d3 should be the same");
            Assert.IsTrue(d2.Equals(d3), "d2 and d3 should be the same");
            Assert.IsTrue(Csla.SmartDate.Equals(d2, d3), "d2 and d3 should be the same");

            d3.Date = DateTime.Now;
            Assert.AreEqual(-1, d2.CompareTo(d3), "d2 and d3 should not be the same");
            Assert.AreEqual(1, d3.CompareTo(d2), "d2 and d3 should not be the same");
            Assert.IsFalse(d2.Equals(d3), "d2 and d3 should not be the same");
            Assert.IsFalse(Csla.SmartDate.Equals(d2, d3), "d2 and d3 should not be the same");
            Assert.IsFalse(d3.Equals(d2), "d2 and d3 should not be the same");
            Assert.IsFalse(Csla.SmartDate.Equals(d3, d2), "d2 and d3 should not be the same");
        }
        #endregion

        #region Comparison Operators
        [TestMethod()]
        public void ComparisonOperators()
        {
            Csla.SmartDate d1 = new Csla.SmartDate();
            Csla.SmartDate d2 = new Csla.SmartDate();

            d1.Date = new DateTime(2005, 1, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 < d2, "d1 should be less than d2");
            d1.Date = new DateTime(2005, 2, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 < d2, "d1 should be equal to d2");
            d1.Date = new DateTime(2005, 3, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 < d2, "d1 should be greater than d2");

            d1.Date = new DateTime(2005, 3, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 > d2, "d1 should be greater than d2");
            d1.Date = new DateTime(2005, 2, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 > d2, "d1 should be equal to d2");
            d1.Date = new DateTime(2005, 1, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 > d2, "d1 should be less than d2");

            d1.Date = new DateTime(2005, 2, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 == d2, "d1 should be equal to d2");
            d1.Date = new DateTime(2005, 1, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 == d2, "d1 should not be equal to d2");

            //#warning Smart date does not overload the <= or >= operators!
            //Assert.Fail("Missing <= and >= operators");
            d1.Date = new DateTime(2005, 1, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 <= d2, "d1 should be less than or equal to d2");
            d1.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 <= d2, "d1 should be less than or equal to d2");
            d1.Date = new DateTime(2005, 3, 1);
            Assert.IsFalse(d1 <= d2, "d1 should be greater than to d2");

            d1.Date = new DateTime(2005, 3, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 >= d2, "d1 should be greater than or equal to d2");
            d1.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 >= d2, "d1 should be greater than or equal to d2");
            d1.Date = new DateTime(2005, 1, 1);
            Assert.IsFalse(d1 >= d2, "d1 should be less than to d2");

            d1.Date = new DateTime(2005, 1, 1);
            d2.Date = new DateTime(2005, 2, 1);
            Assert.IsTrue(d1 != d2, "d1 should not be equal to d2");
            d1.Date = new DateTime(2005, 2, 1);
            Assert.IsFalse(d1 != d2, "d1 should be equal to d2");
            d1.Date = new DateTime(2005, 3, 1);
            Assert.IsTrue(d1 != d2, "d1 should be greater than d2");
        }
        #endregion
    }
}
