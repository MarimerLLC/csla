//-----------------------------------------------------------------------
// <copyright file="ReadonlyAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.Security;
using UnitDriven;
using System.Diagnostics;
using System.Security.Claims;

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
    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    [TestMethod()]
    public void TestAllowInstanceAndShared()
    {
      ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("John", person.FirstName,"Should be able read first name");
      Assert.AreEqual("Doe", person.LastName, "Should be able read first name");
      Csla.ApplicationContext.User = new ClaimsPrincipal();
      ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      ReadOnlyPerson clone = person.Clone();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("John", clone.FirstName, "Should be able read first name of clone");
      Assert.AreEqual("Doe", clone.LastName, "Should be able read first name of clone");
      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod()]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestDenyInstanceAndShared()
    {
      
      ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
      Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod()]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestDenyInstanceAndSharedForClone()
    {

      ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson().Clone();
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
      Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
      Csla.ApplicationContext.User = new ClaimsPrincipal();

    }
  }
}