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
        [TestMethod()]
        public void TestSmartDate()
        {
            DateTime now = DateTime.Now;
            Csla.SmartDate d = new Csla.SmartDate(now, true);
            Assert.AreEqual(now, d.Date);
        }

        [TestMethod()]
        public void Add()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(2005, 1, 1);
            d3 = new Csla.SmartDate(d2.Add(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, 30, d2.Date), d3.Date, "Dates should be equal");
        }

        [TestMethod()]
        public void Subtract()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(2005, 1, 1);
            d3 = new Csla.SmartDate(d2.Subtract(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, -30, d2.Date), d3.Date, "Dates should be equal");
        }
        #region Comparison
        [TestMethod()]
        public void Comparison()
        {
            Csla.SmartDate d2 = new Csla.SmartDate(true);
            Csla.SmartDate d3 = new Csla.SmartDate(false);

            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal");
            Assert.IsTrue(Csla.SmartDate.Equals(d2, d3), "Empty dates should be equal (shared)");
            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal (unary)");

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
            //Assert.IsTrue(d3.Equals(d2.Date.ToShortTimeString()), "Should be equal to any date time string");
            Assert.IsTrue(d3.Equals(d2.Date.ToShortDateString()), "Should be equal to any date time string");
            //Assert.IsTrue(d3.Equals(d2.Date.ToLongTimeString()), "Should be equal to any date time string");
            Assert.IsTrue(d3.Equals(d2.Date.ToLongDateString()), "Should be equal to any date time string");

            //DateTime can be compared using all sorts of formats but the SmartDate cannot.
            //DateTime dt = DateTime.Now;
            //long ldt = dt.ToBinary();
            //Assert.IsTrue(dt.Equals(ldt), "Should be equal");
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

            #warning Smart date does not overload the <= or >= operators!
            Assert.Fail("Missing <= and >= operators");
            //d1.Date = new DateTime(2005, 1, 1);
            //d2.Date = new DateTime(2005, 2, 1);
            //Assert.IsTrue(d1 <= d2, "d1 should be less than or equal to d2");
            //d1.Date = new DateTime(2005, 2, 1);
            //Assert.IsTrue(d1 <= d2, "d1 should be less than or equal to d2");
            //d1.Date = new DateTime(2005, 3, 1);
            //Assert.IsFalse(d1 <= d2, "d1 should be greater than to d2");

            //d1.Date = new DateTime(2005, 3, 1);
            //d2.Date = new DateTime(2005, 2, 1);
            //Assert.IsTrue(d1 >= d2, "d1 should be greater than or equal to d2");
            //d1.Date = new DateTime(2005, 2, 1);
            //Assert.IsTrue(d1 >= d2, "d1 should be greater than or equal to d2");
            //d1.Date = new DateTime(2005, 1, 1);
            //Assert.IsFalse(d1 >= d2, "d1 should be less than to d2");

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
