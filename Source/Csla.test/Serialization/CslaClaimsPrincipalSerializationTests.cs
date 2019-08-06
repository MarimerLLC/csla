//-----------------------------------------------------------------------
// <copyright file="CslaClaimsPrincipalSerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using Csla.Serialization;
using Csla.Test.ValidationRules;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using System.IO;
#endif 

namespace Csla.Test.Serialization
{
  [TestClass()]
  public class CslaClaimsPrincipalSerializationTests
  {
    [TestMethod]
    public void SerializeCslaClaimsPrincipal()
    {
      var identity = new System.Security.Principal.GenericIdentity("rocky", "custom");
      var principal = new Csla.Security.CslaClaimsPrincipal(identity);
      var clone = (Csla.Security.CslaClaimsPrincipal)Core.ObjectCloner.Clone(principal);
      Assert.AreEqual(principal.Identity.Name, clone.Identity.Name);
      Assert.AreEqual(principal.Identity.AuthenticationType, clone.Identity.AuthenticationType);
    }
  }
}
