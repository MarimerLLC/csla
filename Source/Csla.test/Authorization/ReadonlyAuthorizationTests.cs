﻿//-----------------------------------------------------------------------
// <copyright file="ReadonlyAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Core;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
#endif
  [TestClass()]
  public class ReadonlyAuthorizationTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context) {
      _ = context;
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    [TestMethod]
    public void WhenReadOnlyBaseHasAuthorizationRuleChecksDisabledThePropertiesShouldBeReadableEvenThoughIDontHaveTheNeededRule() {
      var person = ReadOnlyPerson.GetReadOnlyPerson();
      ((IUseApplicationContext)person).ApplicationContext = _testDIContext.CreateTestApplicationContext();

      person.SetDisableCanReadAuthorizationChecks(isCanReadAuthorizationChecksDisabled: true);

      _ = person.FirstName; // When no exception happens this test is successful
    }

    //[TestMethod()]
    //public void TestAllowInstanceAndShared()
    //{
    //  TestResults.Reinitialise();
    //  Csla.ApplicationContext.User = GetPrincipal("Admin");
    //  ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
    //  Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
    //  Assert.AreEqual("John", person.FirstName,"Should be able read first name");
    //  Assert.AreEqual("Doe", person.LastName, "Should be able read first name");
    //  Csla.ApplicationContext.User = new ClaimsPrincipal();
    //  TestResults.Reinitialise();
    //  Csla.ApplicationContext.User = GetPrincipal("Admin");
    //  ReadOnlyPerson clone = person.Clone();
    //  Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
    //  Assert.AreEqual("John", clone.FirstName, "Should be able read first name of clone");
    //  Assert.AreEqual("Doe", clone.LastName, "Should be able read first name of clone");
    //  Csla.ApplicationContext.User = new ClaimsPrincipal();
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(Csla.Security.SecurityException))]
    //public void TestDenyInstanceAndShared()
    //{
      
    //  TestResults.Reinitialise();
    //  Csla.ApplicationContext.User = GetPrincipal("Admin");
    //  ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson();
    //  Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
    //  Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
    //  Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
    //  Csla.ApplicationContext.User = new ClaimsPrincipal();
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(Csla.Security.SecurityException))]
    //public void TestDenyInstanceAndSharedForClone()
    //{

    //  TestResults.Reinitialise();
    //  Csla.ApplicationContext.User = GetPrincipal("Admin");
    //  ReadOnlyPerson person = ReadOnlyPerson.GetReadOnlyPerson().Clone();
    //  Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
    //  Assert.AreEqual("!", person.MiddleName, "Should not be able read middle name");
    //  Assert.AreEqual("!", person.PlaceOfBirth, "Should not be able read place of birth");
    //  Csla.ApplicationContext.User = new ClaimsPrincipal();

    //}
  }
}