//-----------------------------------------------------------------------
// <copyright file="ClaimsPrincipalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Linq;
using System.Security.Claims;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.Serialization
{
  [TestClass]
  public class ClaimsPrincipalTests
  {
    [TestMethod]
    public void CloneClaimsPrincipal()
    {
      Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
      var i = new ClaimsIdentity();
      i.AddClaim(new Claim("name", "Franklin"));
      var p = new ClaimsPrincipal(i);
      var p1 = (ClaimsPrincipal)Core.ObjectCloner.Clone(p);
      Assert.AreNotSame(p, p1, "Should be different instances");
      Assert.AreEqual(p.Claims.Count(), p1.Claims.Count(), "Should have same number of claims");
      var c = p1.Claims.Where(r => r.Type == "name").First();
      Assert.AreEqual("Franklin", c.Value, "Claim value should match");
    }
  }
}
