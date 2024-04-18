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
using Csla.TestHelpers;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using System.IO;

namespace Csla.Test.Serialization
{
  [TestClass()]
  public class CslaClaimsPrincipalSerializationTests
  {
    [TestMethod]
    public void SerializeCslaClaimsPrincipalGeneric()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var generic = new System.Security.Principal.GenericIdentity("rocky", "custom");
      var identity = new ClaimsIdentity(generic);
      var principal = new Csla.Security.CslaClaimsPrincipal(identity);
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (Csla.Security.CslaClaimsPrincipal)cloner.Clone(principal);
      Assert.AreEqual(principal.Identity.Name, clone.Identity.Name);
      Assert.AreEqual(principal.Identity.IsAuthenticated, clone.Identity.IsAuthenticated);
    }

    [TestMethod]
    public void SerializeClaimsPrincipal()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var identity = new ClaimsIdentity("custom", "rocky", null);
      var principal = new ClaimsPrincipal(identity);
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (Csla.Security.CslaClaimsPrincipal)cloner.Clone(principal);
      Assert.AreEqual(principal.Identity.Name, clone.Identity.Name);
      Assert.AreEqual(principal.Identity.AuthenticationType, clone.Identity.AuthenticationType);
      Assert.AreEqual(principal.Identity.IsAuthenticated, clone.Identity.IsAuthenticated);
    }
  }
}
