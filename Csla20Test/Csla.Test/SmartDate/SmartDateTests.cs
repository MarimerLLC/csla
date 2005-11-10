using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.VisualBasic;

namespace Csla.Test.SmartDate
{
    [TestFixture()]
    public class SmartDateTests
    {
        [Test()]
        public void TestSmartDate()
        {
            DateTime now = DateTime.Now;
            Csla.SmartDate d = new Csla.SmartDate(now, true);
            Assert.AreEqual(now, d.Date);
        }

        [Test()]
        public void Add()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(1, 1, 2005);
            d3 = new Csla.SmartDate(d2.Add(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, 30, d2.Date), d3.Date, "Dates should be equal");
        }

        [Test()]
        public void Subtract()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d2.Date = new DateTime(1, 1, 2005);
            d3 = new Csla.SmartDate(d2.Subtract(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(DateAndTime.DateAdd(DateInterval.Day, -30, d2.Date), d3.Date, "Dates should be equal");
        }

        [Test()]
        public void Comparison()
        {
            Csla.SmartDate d2 = new Csla.SmartDate(true);
            Csla.SmartDate d3 = new Csla.SmartDate(false);

            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal");
            Assert.IsTrue(Csla.SmartDate.Equals(d2, d3), "Empty dates should be equal (shared)");
            Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal (unary)");

            d2.Date = new DateTime(1, 1, 2005);
            d3 = new Csla.SmartDate(d2.Date, d2.EmptyIsMin);
            Assert.AreEqual(d2, d3, "Assigned dates should be equal");

            d3.Date = new DateTime(2, 2, 2005);
            Assert.AreEqual(1, d3.CompareTo(d2), "Should be greater than");
            Assert.AreEqual(-1, d2.CompareTo(d3), "Should be less than");
            Assert.IsFalse(d2.CompareTo(d3) == 0, "should not be equal");

            Assert.IsTrue(d3.Equals("2/2/2005"), "Should be equal to string date");
            Assert.IsTrue(d3.Equals(new DateTime(2, 2, 2005)), "should be equal to DateTime");
        }

        [Test()]
        public void Empty()
        {
            Csla.SmartDate d2 = new Csla.SmartDate();
            Csla.SmartDate d3;

            d3 = new Csla.SmartDate(d2.Add(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(d2, d3, "Dates should be equal");

            d3 = new Csla.SmartDate(d2.Subtract(new TimeSpan(30, 0, 0, 0)));
            Assert.AreEqual(d2, d3, "Dates should be equal");
        }

        [Test()]
        public void ComparisonOperators()
        {
            Csla.SmartDate d1 = new Csla.SmartDate();
            Csla.SmartDate d2 = new Csla.SmartDate();

            d1.Date = new DateTime(1, 1, 2005);
            d2.Date = new DateTime(2, 1, 2005);
            Assert.IsTrue(d1 < d2, "d1 should be less than d2");

            d1.Date = new DateTime(3, 1, 2005);
            d2.Date = new DateTime(2, 1, 2005);
            Assert.IsTrue(d1 > d2, "d1 should be greater than d2");

            d1.Date = new DateTime(2, 1, 2005);
            d2.Date = new DateTime(2, 1, 2005);
            Assert.IsTrue(d1 == d2, "d1 should be equal to d2");
        }
    }
}
