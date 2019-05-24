//-----------------------------------------------------------------------
// <copyright file="ReadOnlyListTestsLocalAndRemote.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using UnitDriven;
using Csla.DataPortalClient;
using System.Threading.Tasks;

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

namespace cslalighttest.Stereotypes
{
  [TestClass]
 public class ReadOnlyListTestsLocalAndRemote : TestBase
  {
    [TestMethod]
    public async Task ReadOnlyListFetchLocal()
    {
      var result = await Csla.DataPortal.FetchAsync<ReadOnlyPersonList>();
      Assert.AreEqual("John Doe", result[0].Name);
      Assert.AreEqual(new DateTime(1982, 1, 1), result[1].Birthdate);
      Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task ReadOnlyListFetchRemote()
    {
      var result = await Csla.DataPortal.FetchAsync<ReadOnlyPersonList>();
      Assert.AreEqual("John Doe", result[0].Name);
      Assert.AreEqual(new DateTime(1982, 1, 1), result[1].Birthdate);
      Assert.AreEqual(2, result.Count);
    }
  }
}