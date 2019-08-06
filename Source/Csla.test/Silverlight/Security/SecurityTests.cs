//-----------------------------------------------------------------------
// <copyright file="SecurityTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using UnitDriven;

#if NUNIT
using TestClass = NUnit.Framework.TestFixtureAttribute;

#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Silverlight.Security
{
  [TestClass]
  public partial class SecurityTests : TestBase
  {
    protected string AdminRoleName = "Admin Role";
    protected string WcfProxyTypeName = "Csla.DataPortalClient.SynchronizedWcfProxy`1, Csla, Version=3.6.0.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30";

  }
}