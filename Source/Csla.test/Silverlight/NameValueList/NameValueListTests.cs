//-----------------------------------------------------------------------
// <copyright file="NameValueListTests.cs" company="Marimer LLC">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.NameValueList
{
  [TestClass]
  public class NameValueListTests : TestBase
  {
    [TestMethod]
    public void TestNameValueList()
    {
      BasicNameValueList list = BasicNameValueList.GetBasicNameValueList();
      Assert.AreEqual(10, list.Count, "Items are not caught via fetch");
      Assert.AreEqual("element_2", list.GetItemByKey(2).Value, "NameValue collection fetch failed.");
    }
  }
}