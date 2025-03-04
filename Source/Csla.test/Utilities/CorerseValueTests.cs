//-----------------------------------------------------------------------
// <copyright file="CorerseValueTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Utilities
{
  [TestClass]
  public class CoerseValueTests
  {
    [TestMethod]
    public void TestCoerseValue()
    {
      UtilitiesTestHelper helper = new UtilitiesTestHelper();

      helper.IntProperty = 0;
      helper.StringProperty = "1";
      helper.IntProperty = (int)Csla.Utilities.CoerceValue(typeof(int), typeof(string), null, helper.StringProperty);
      Assert.AreEqual(1, helper.IntProperty, "Should have converted to int");

      helper.IntProperty = 2;
      helper.StringProperty = "";
      helper.StringProperty = (string)Csla.Utilities.CoerceValue(typeof(string), typeof(int), null, helper.IntProperty);
      Assert.AreEqual("2", helper.StringProperty, "Should have converted to string");


      helper.StringProperty = "1";
      helper.NullableStringProperty = null;
      object convertedValue = Csla.Utilities.CoerceValue(typeof(string), typeof(string), null, helper.NullableStringProperty);
      Assert.IsNull(helper.NullableStringProperty);
      Assert.IsNull(convertedValue);

      string actual = (string)Csla.Utilities.CoerceValue(typeof(string), typeof(UtilitiesTestHelper), null, helper);
      Assert.AreEqual(UtilitiesTestHelper.ToStringValue, actual, "Should have issued ToString()");
    }

    [TestMethod]
    public void SmartDateCoerseValue()
    {
      var date = DateTime.Now;
      var smart = Csla.Utilities.CoerceValue<Csla.SmartDate>(typeof(DateTime), new Csla.SmartDate(), date);
      Assert.AreEqual(date, smart.Date, "Dates should be equal");
    }
  }
}