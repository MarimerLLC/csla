//-----------------------------------------------------------------------
// <copyright file="ReadOnlyTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace cslalighttest.ReadOnly
{
#if TESTING
  [DebuggerStepThrough]
#endif
  [TestClass]
  public class ReadOnlyTests : TestBase
  {
    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void AddItemToReadOnlyFail()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList();
      MockReadOnly mock = new MockReadOnly();
      context.Assert.Try(()=>list.Add(mock));
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void InsertItemToReadOnlyFail()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList();
      MockReadOnly mock = new MockReadOnly();
      context.Assert.Try(() => list.Insert(0, mock));
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void IndexInsertItemToReadOnlyFail()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList(new MockReadOnly());
      context.Assert.Try(() => list[0] = new MockReadOnly());
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void AddNewToReadOnlyFail()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList();
      context.Assert.Try(() => list.AddNew());
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ClearReadOnlyFail()
    {
      UnitTestContext context = GetContext();
      MockReadOnlyList list = new MockReadOnlyList();
      context.Assert.Try(() => list.Clear());
      context.Complete();
    }
  }
}