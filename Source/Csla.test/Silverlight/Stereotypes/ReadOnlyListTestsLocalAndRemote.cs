﻿//-----------------------------------------------------------------------
// <copyright file="ReadOnlyListTestsLocalAndRemote.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using UnitDriven;
using Csla.DataPortalClient;

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
    public void ReadOnlyListFetchLocal()
    {

#if SILVERLIGHT
      Csla.DataPortal.ProxyTypeName = "Local";
#endif
      var context = GetContext();
      ReadOnlyPersonList.Fetch((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual("John Doe", e.Object[0].Name);
        context.Assert.AreEqual(new DateTime(1982, 1, 1), e.Object[1].Birthdate);
        context.Assert.AreEqual(2, e.Object.Count);
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void ReadOnlyListFetchRemote()
    {
#if SILVERLIGHT
      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
#endif
      var context = GetContext();
      ReadOnlyPersonList.Fetch((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual("John Doe", e.Object[0].Name);
        context.Assert.AreEqual(new DateTime(1982, 1, 1), e.Object[1].Birthdate);
        context.Assert.AreEqual(2, e.Object.Count);
        context.Assert.Success();
      });
      context.Complete();
    }
  }
}