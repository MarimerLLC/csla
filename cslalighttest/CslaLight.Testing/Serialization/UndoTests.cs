using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using cslalighttest.Engine;
using Csla.Core;
using System.Diagnostics;

namespace cslalighttest.Serialization
{
  [TestClass]
  public class UndoTests
  {
    [TestMethod]
    public void EditUndoSuccess()
    {
      int expected = 1;

      Person p = new Person();
      p.Age = expected;
      p.BeginEdit();
      p.Age = 2;
      p.CancelEdit();

      Assert.AreEqual(expected, p.Age);      
    }

    [TestMethod]
    public void MultiEditUndoSuccess()
    {
      Person p = new Person();
      p.Age = 1;
      p.BeginEdit();

      p.Age = 2;
      p.BeginEdit();

      p.Age = 3;

      Assert.AreEqual(3, p.Age);
      p.CancelEdit();
      Assert.AreEqual(2, p.Age);
      p.CancelEdit();
      Assert.AreEqual(1, p.Age);
    }

    [TestMethod]
    public void UndoUninitializedValues()
    {
      Person p = new Person();
      int expected = p.Age;
      p.BeginEdit();
      p.Age = 100;
      p.CancelEdit();

      Assert.AreEqual(expected, p.Age);
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    [DebuggerNonUserCode]
    public void UndoFail()
    {
      Person p = new Person();
      p.CancelEdit();
    }

    [TestMethod]
    public void UndoWithChild()
    {
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      int age1 = p.Age;
      string city1 = a.City;
      p.BeginEdit();

      int age2 = p.Age = 2;
      string city2 = a.City = "two";

      Assert.AreEqual(age2, p.Age);
      Assert.AreEqual(city2, a.City);
      p.CancelEdit();

      Assert.AreEqual(age1, p.Age);
      Assert.AreEqual(city1, a.City);
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    [DebuggerNonUserCode]
    public void UndoChildFail()
    {
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.BeginEdit();
      p.CancelEdit();
      a.CancelEdit();
    }

    [TestMethod]
    public void UndoChildSuccess()
    {
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

      Assert.AreEqual(age2, p.Age);
      Assert.AreEqual(city2, a.City);
      p.CancelEdit();

      Assert.AreEqual(age1, p.Age);
      Assert.AreEqual(city1, a.City);
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    [DebuggerNonUserCode]
    public void UndoAfterApplyEditFail()
    {
      Person p = new Person();
      p.BeginEdit();
      p.ApplyEdit();
      p.CancelEdit();
    }

    [TestMethod]
    public void ApplyEditSuccess()
    {
      Person p = new Person();
      p.Age = 1;
      p.BeginEdit();
      p.Age = 2;
      p.ApplyEdit();

      Assert.AreEqual(2, p.Age);
    }

    [TestMethod]
    public void ApplyEditWithChildSuccess()
    {
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

      Assert.AreEqual(2, p.Age);
      Assert.AreEqual("two", a.City);
    }

    [TestMethod]
    [ExpectedException(typeof(UndoException))]
    [DebuggerNonUserCode]
    public void ApplyEditOnChildThenRoot()
    {
      Person p = new Person();
      p.Addresses = new AddressList();
      Address a = new Address();
      p.Addresses.Add(a);

      p.BeginEdit();
      a.ApplyEdit();
      p.ApplyEdit();
    }
  }
}
