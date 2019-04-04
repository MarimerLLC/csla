//-----------------------------------------------------------------------
// <copyright file="SmartDateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Serialization;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using UnitDriven;
#if !WINDOWS_PHONE
using Microsoft.VisualBasic;
#endif
using Csla.Serialization.Mobile;
using System.Threading;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
using Microsoft.VisualBasic;
using Csla.Serialization.Mobile;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.SmartDate
{
  [TestClass()]
  public class SmartDateTests
  {
    System.Globalization.CultureInfo CurrentCulture { get; set; }
    System.Globalization.CultureInfo CurrentUICulture { get; set; }

    [TestInitialize]
    public void Setup()
    {

      // store current cultures
      CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
      CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

      // set to "en-US" for all tests
      System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
      System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
    }

    [TestCleanup]
    public void Cleanup()
    {
      // restore original cultures
      System.Threading.Thread.CurrentThread.CurrentCulture = CurrentCulture;
      System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentUICulture;
    }

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

      d = new Csla.SmartDate((DateTime?)null, true);
      Assert.AreEqual(DateTime.MinValue, d.Date);
      d = new Csla.SmartDate((DateTime?)null, false);
      Assert.AreEqual(DateTime.MaxValue, d.Date);
      d = new Csla.SmartDate((DateTime?)null, Csla.SmartDate.EmptyValue.MinDate);
      Assert.AreEqual(DateTime.MinValue, d.Date);
      d = new Csla.SmartDate((DateTime?)null, Csla.SmartDate.EmptyValue.MaxDate);
      Assert.AreEqual(DateTime.MaxValue, d.Date);
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
      Assert.AreEqual("02/01/2005", date, "dd/MM/yyyy test");
      date = Csla.SmartDate.DateToString(d, "MM/dd/yy");
      Assert.AreEqual("01/02/05", date, "MM/dd/yy test");
      date = Csla.SmartDate.DateToString(d, "");
      Assert.AreEqual("1/2/2005 12:00:00 AM", date);
      date = Csla.SmartDate.DateToString(d, "d");
      Assert.AreEqual("1/2/2005", date);
      date = new Csla.SmartDate(d).ToString();
      Assert.AreEqual("1/2/2005", date);

      date = Csla.SmartDate.DateToString(DateTime.MinValue, "dd/MM/yyyy", true);
      Assert.AreEqual("", date, "MinValue w/ emptyIsMin=true");
      date = Csla.SmartDate.DateToString(DateTime.MinValue, "dd/MM/yyyy", false);
      Assert.AreEqual(DateTime.MinValue.ToString("dd/MM/yyyy"), date, "MinValue w/ emptyIsMin=false");
      date = Csla.SmartDate.DateToString(DateTime.MaxValue, "dd/MM/yyyy", true);
      Assert.AreEqual(DateTime.MaxValue.ToString("dd/MM/yyyy"), date, "MaxValue w/ emptyIsMin=true");
      date = Csla.SmartDate.DateToString(DateTime.MaxValue, "dd/MM/yyyy", false);
      Assert.AreEqual("", date, "MaxValue w/ emptyIsMin=false");
    }
    #endregion

#if !WINDOWS_PHONE
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

      Assert.AreEqual(30, ((TimeSpan)(d2 - d3)).Days, "Should be 30 days different");
      Assert.AreEqual(d3, d2 - new TimeSpan(30, 0, 0, 0, 0), "Should be equal");
    }
    #endregion
#endif

    #region Comparison
    [TestMethod()]
    public void Comparison()
    {
      Csla.SmartDate d2 = new Csla.SmartDate(true);
      Csla.SmartDate d3 = new Csla.SmartDate(false);
      Csla.SmartDate d4 = new Csla.SmartDate(Csla.SmartDate.EmptyValue.MinDate);
      Csla.SmartDate d5 = new Csla.SmartDate(Csla.SmartDate.EmptyValue.MaxDate);

      Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal");
      Assert.IsTrue(Csla.SmartDate.Equals(d2, d3), "Empty dates should be equal (shared)");
      Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal (unary)");
      Assert.IsTrue(d2.Equals(""), "Should be equal to an empty string (d2)");
      Assert.IsTrue(d3.Equals(""), "Should be equal to an empty string (d3)");

      Assert.IsTrue(d2.Date.Equals(DateTime.MinValue), "Should be DateTime.MinValue");
      Assert.IsTrue(d3.Date.Equals(DateTime.MaxValue), "Should be DateTime.MaxValue");

      Assert.IsTrue(d4.Date.Equals(DateTime.MinValue), "Should be DateTime.MinValue (d4)");
      Assert.IsTrue(d5.Date.Equals(DateTime.MaxValue), "Should be DateTime.MaxValue (d5)");

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

      Assert.IsTrue(d3.Equals(d2.Date.ToString()), "Should be equal to any date time string");
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

    [TestMethod]
    public void MaxDateMaxValue()
    {
      // test for maxDateValue

      Csla.SmartDate target = new Csla.SmartDate(Csla.SmartDate.EmptyValue.MaxDate);

      DateTime expected = DateTime.MaxValue;

      DateTime actual = target.Date;

      Assert.AreEqual(expected, actual);
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

    [TestMethod]
    public void TryParseTest()
    {
      Csla.SmartDate sd = new Csla.SmartDate();
      if (Csla.SmartDate.TryParse("blah", ref sd))
        Assert.AreEqual(true, false, "TryParse should have failed");
      if (Csla.SmartDate.TryParse("t", ref sd))
        Assert.AreEqual(DateTime.Now.Date, sd.Date.Date, "Date should have been now");
      else
        Assert.AreEqual(true, false, "TryParse should have succeeded");
    }

    #endregion

    #region Serialization
    [TestMethod()]
    public void SerializationTest()
    {
      Csla.SmartDate d2;

      d2 = new Csla.SmartDate();
      Csla.SmartDate clone = (Csla.SmartDate)MobileFormatter.Deserialize(MobileFormatter.Serialize(d2));
      Assert.AreEqual(d2, clone, "Dates should have ben the same");

      d2 = new Csla.SmartDate(DateTime.Now, false);
      clone = (Csla.SmartDate)MobileFormatter.Deserialize(MobileFormatter.Serialize(d2));
      Assert.AreEqual(d2, clone, "Dates should have ben the same");

      d2 = new Csla.SmartDate(DateTime.Now.AddDays(10), false);
      d2.FormatString = "YYYY/DD/MM";
      clone = (Csla.SmartDate)MobileFormatter.Deserialize(MobileFormatter.Serialize(d2));
      Assert.AreEqual(d2, clone, "Dates should have ben the same");

      cslalighttest.Serialization.PersonWIthSmartDateField person;
      person = cslalighttest.Serialization.PersonWIthSmartDateField.GetPersonWIthSmartDateField("Sergey", 2000);
      Assert.AreEqual(person.Birthdate, person.Clone().Birthdate, "Dates should have ben the same");

      Csla.SmartDate expected = person.Birthdate;
      person.BeginEdit();
      person.Birthdate = new Csla.SmartDate(expected.Date.AddDays(10)); // to guarantee it's a different value
      person.CancelEdit();
      Csla.SmartDate actual = person.Birthdate;
      Assert.AreEqual(expected, actual);

    }
    #endregion

    [TestMethod]
    public void DefaultFormat()
    {
      var obj = new SDtest();
      Assert.AreEqual("", obj.TextDate, "Should be empty");

      var now = DateTime.Now;
      obj.TextDate = string.Format("{0:g}", now);
      Assert.AreEqual(string.Format("{0:g}", now), obj.TextDate, "Should be today");
    }


    [TestMethod]
    public void CustomParserReturnsDateTime()
    {
      Csla.SmartDate.CustomParser = (s) =>
                                      {
                                        if (s == "test") return DateTime.Now;
                                        return null;
                                      };

      // uses custom parser
      var date = new Csla.SmartDate("test");
      Assert.AreEqual(DateTime.Now.Date, date.Date.Date);

      // uses buildin parser
      var date2 = new Csla.SmartDate("t");
      Assert.AreEqual(DateTime.Now.Date, date.Date.Date);
    }
  }

  [Serializable]
  public class SDtest : BusinessBase<SDtest>
  {
    public static PropertyInfo<Csla.SmartDate> TextDateProperty = 
      RegisterProperty<Csla.SmartDate>(c => c.TextDate, null, new Csla.SmartDate { FormatString = "g" });
    public string TextDate
    {
      get { return GetPropertyConvert<Csla.SmartDate, string>(TextDateProperty); }
      set { SetPropertyConvert<Csla.SmartDate, string>(TextDateProperty, value); }
    }

    public static PropertyInfo<Csla.SmartDate> MyDateProperty = RegisterProperty<Csla.SmartDate>(c => c.MyDate);
    public Csla.SmartDate MyDate
    {
      get { return GetProperty(MyDateProperty); }
      set { SetProperty(MyDateProperty, value); }
    }
  }
}