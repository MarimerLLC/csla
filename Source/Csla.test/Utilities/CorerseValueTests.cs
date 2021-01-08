//-----------------------------------------------------------------------
// <copyright file="CorerseValueTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
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

namespace Csla.Test.Utilities
{
  [TestClass]
  public class CoerseValueTests : TestBase
  {
    [TestMethod]
    public void TestCoerseValue()
    {
      UnitTestContext context = GetContext();
      UtilitiesTestHelper helper = new UtilitiesTestHelper();

      helper.IntProperty = 0;
      helper.StringProperty = "1";
      helper.IntProperty = (int)Csla.Utilities.CoerceValue(typeof(int), typeof(string), null, helper.StringProperty);
      context.Assert.AreEqual(1, helper.IntProperty, "Should have converted to int");

      helper.IntProperty = 2;
      helper.StringProperty = "";
      helper.StringProperty = (string)Csla.Utilities.CoerceValue(typeof(string), typeof(int), null, helper.IntProperty);
      context.Assert.AreEqual("2", helper.StringProperty, "Should have converted to string");


      helper.StringProperty = "1";
      helper.NullableStringProperty = null;
      object convertedValue = Csla.Utilities.CoerceValue(typeof(string), typeof(string), null, helper.NullableStringProperty);
      context.Assert.IsNull(helper.NullableStringProperty);
      context.Assert.IsNull(convertedValue);

      context.Assert.AreEqual(UtilitiesTestHelper.ToStringValue, (string)Csla.Utilities.CoerceValue(typeof(string), typeof(UtilitiesTestHelper), null, helper), "Should have issued ToString()");
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void SmartDateCoerseValue()
    {
      UnitTestContext context = GetContext();

      var date = DateTime.Now;
      var smart = Csla.Utilities.CoerceValue<Csla.SmartDate>(typeof(DateTime), new Csla.SmartDate(), date);
      context.Assert.AreEqual(date, smart.Date, "Dates should be equal");
      context.Assert.Success();
      context.Complete();
    }
  }
}