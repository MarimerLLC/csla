//-----------------------------------------------------------------------
// <copyright file="CloneTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
#if !__ANDROID__
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif
using UnitDriven;
using System.Diagnostics;
using cslalighttest.ReadOnly;
using Csla.DataPortalClient;
using Csla;
using cslalighttest.NameValueList;
using Csla.Testing.Business.EditableRootTests;
using Csla.Testing.Business.EditableRootListTests;
using Csla.Test.ChildChanged;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace cslalighttest.Clone
{
#if TESTING
  [DebuggerStepThrough]
#endif
  [TestClass]
  public class CloneTests : TestBase
  {
    [TestMethod]
    public void CloneROList()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList(new MockReadOnly());
      MockReadOnlyList clone = list.Clone();
      context.Assert.AreEqual(list.Count, clone.Count, "Failed to clone");
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void CloneRO()
    {
      UnitTestContext context = GetContext();
      MockReadOnly readonlyItem = new MockReadOnly();
      MockReadOnly clone = readonlyItem.Clone();
      context.Assert.AreEqual(readonlyItem.Id, clone.Id, "Failed to clone");
      context.Assert.Success();
      context.Complete();
    }

#if !__ANDROID__
    [TestMethod]
    public void CloneNVList()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      BasicNameValueList.GetBasicNameValueList((o, e) =>
      {
        BasicNameValueList list = e.Object;
        BasicNameValueList clone = (BasicNameValueList)list.Clone();
        context.Assert.AreEqual(list.Count, clone.Count, "Failed to clone");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void CloneBB()
    {
      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;

      var context = GetContext();
      MockEditableRoot.Fetch(
        MockEditableRoot.MockEditableRootId,
        (o, e) =>
        {
          var actual = (MockEditableRoot)e.Object;
          MockEditableRoot clone = actual.Clone();
          context.Assert.AreEqual(actual.Id, clone.Id, "Failed to clone");
          context.Assert.Success();
        });

      context.Complete();
    }
#endif
    [TestMethod]
    public void CloneBBList()
    {

      UnitTestContext context = GetContext();
      ListContainerList list = new ListContainerList();
      ListContainerList clone = list.Clone();
      context.Assert.AreEqual(list.Count, clone.Count, "Failed to clone");
      context.Assert.Success();
      context.Complete();
    }

  }
}