//-----------------------------------------------------------------------
// <copyright file="UndoTests.cs" company="Marimer LLC">
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
using Csla.Core;
using UnitDriven;
using Csla.Testing.Business.EditableRootTests;
using Csla;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace cslalighttest.Serialization
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class UndoTests : TestBase
  {
    [TestMethod]
    public void EditUndoSuccess()
    {
      UnitTestContext context = GetContext();
      int expected = 1;

      Person p = new Person();
      p.Age = expected;
      p.BeginEdit();
      p.Age = 2;
      p.CancelEdit();

      context.Assert.AreEqual(expected, p.Age);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MultiEditUndoSuccess()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Age = 1;
      p.BeginEdit();

      p.Age = 2;
      p.BeginEdit();

      p.Age = 3;

      context.Assert.AreEqual(3, p.Age);
      p.CancelEdit();
      context.Assert.AreEqual(2, p.Age);
      p.CancelEdit();
      context.Assert.AreEqual(1, p.Age);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void UndoUninitializedValues()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      // p.Age; calling this property initializes it! So set it explicitly.
      int expected = (DateTime.Now - new DateTime(1, 1, 1)).Days / 365; //2008; 
      p.BeginEdit();
      p.Age = 100;
      p.CancelEdit();

      context.Assert.AreEqual(expected, p.Age);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void InvalidUndo()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.CancelEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void InvalidApply()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.ApplyEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void UndoWithChild()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      int age1 = p.Age;
      string city1 = a.City;
      p.BeginEdit();

      int age2 = p.Age = 2;
      string city2 = a.City = "two";

      context.Assert.AreEqual(age2, p.Age);
      context.Assert.AreEqual(city2, a.City);
      p.CancelEdit();

      context.Assert.AreEqual(age1, p.Age);
      context.Assert.AreEqual(city1, a.City);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void UndoChildFail()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.BeginEdit();
      p.CancelEdit();
      a.CancelEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ApplyChildAfterRootUndo()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.BeginEdit();
      p.CancelEdit();
      a.ApplyEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void UndoChildSuccess()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      int age1 = p.Age = 1;
      string city1 = a.City = "one";
      p.BeginEdit();

      int age2 = p.Age = 2;
      string city2 = a.City = "two";
      a.BeginEdit();

      string city3 = a.City = "three";
      a.CancelEdit();

      context.Assert.AreEqual(age2, p.Age);
      context.Assert.AreEqual(city2, a.City);
      p.CancelEdit();

      context.Assert.AreEqual(age1, p.Age);
      context.Assert.AreEqual(city1, a.City);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void UndoAfterApplyEditFail()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.BeginEdit();
      p.ApplyEdit();
      p.CancelEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void DoubleApply()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.BeginEdit();
      p.ApplyEdit();
      p.ApplyEdit();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ApplyEditSuccess()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Age = 1;
      p.BeginEdit();
      p.Age = 2;
      p.ApplyEdit();

      context.Assert.AreEqual(2, p.Age);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ApplyEditWithChildSuccess()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.Age = 1;
      a.City = "one";
      p.BeginEdit();

      p.Age = 2;
      a.City = "two";
      p.ApplyEdit();

      context.Assert.AreEqual(2, p.Age);
      context.Assert.AreEqual("two", a.City);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    public void ApplyEditOnChildThenRoot()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.BeginEdit();
      a.ApplyEdit();
      context.Assert.Try( p.ApplyEdit );
      context.Complete();
    }

    [TestMethod]
    public void UndoWithDeleteAddToList()
    {
      UnitTestContext context = GetContext();
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      a.ZipCode = "0";
      p.Addresses.Add(a);
      Address a1 = new Address();
      a1.ZipCode = "1";
      p.Addresses.Add(a1);

      int age1 = p.Age;
      string city1 = a.City;

      p.BeginEdit();

      int age2 = p.Age = 2;
      string city2 = a.City = "two";
      p.Addresses[0].ZipCode = "000";

      Address a2 = new Address();
      a2.ZipCode = "2";
      p.Addresses.Add(a2);
      Address a3 = new Address();
      a3.ZipCode = "3";
      p.Addresses.Add(a3);
      p.Addresses.RemoveAt(0);

      context.Assert.AreEqual(age2, p.Age);
      context.Assert.AreEqual(city2, a.City);
      p.CancelEdit();

      context.Assert.AreEqual(age1, p.Age);
      context.Assert.AreEqual(city1, a.City);
      context.Assert.AreEqual("0", a.ZipCode);
      context.Assert.AreEqual(2, p.Addresses.Count);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    public void UndoParentThenChildEnsureNoEditLevelMismatch()
    {
      using (var context = GetContext())
      {
        Person p = new Person();
        p.Addresses = new AddressList();
        Address a = new Address();
        p.Addresses.Add(a);

        p.BeginEdit();
        a.BeginEdit();
        context.Assert.Try(p.CancelEdit);
      }
    }
  }
}