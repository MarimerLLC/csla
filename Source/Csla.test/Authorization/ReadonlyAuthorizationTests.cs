//-----------------------------------------------------------------------
// <copyright file="ReadonlyAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.Security;
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

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
#endif
  [TestClass()]
  public class ReadonlyAuthorizationTests
  {
    [TestMethod()]
    public void TestAllowInstanceAndShared()
    {
      ApplicationContext.GlobalContext.Clear();
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("John", person.FirstName,"Should be able read first name");
      Assert.AreEqual("Doe", person.LastName, "Should be able read first name");
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type
      ApplicationContext.GlobalContext.Clear();
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
      ReadOnlyPerson clone = person.Clone();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("John", clone.FirstName, "Should be able read first name of clone");
      Assert.AreEqual("Doe", clone.LastName, "Should be able read first name of clone");

    }

    [TestMethod()]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestDenyInstanceAndShared()
    {
      
      ApplicationContext.GlobalContext.Clear();
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
      Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type

    }

    [TestMethod()]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestDenyInstanceAndSharedForClone()
    {

      ApplicationContext.GlobalContext.Clear();
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogin();
#pragma warning restore CS0436 // Type conflicts with imported type
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson().Clone();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
      Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
#pragma warning disable CS0436 // Type conflicts with imported type
      Security.TestPrincipal.SimulateLogout();
#pragma warning restore CS0436 // Type conflicts with imported type

    }
  }
}