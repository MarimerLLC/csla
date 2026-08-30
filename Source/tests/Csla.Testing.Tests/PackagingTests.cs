//-----------------------------------------------------------------------
// <copyright file="PackagingTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests that the Csla.Testing assembly is built and signed correctly</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Testing.Tests
{
  /// <summary>
  /// Verifies the identity of the Csla.Testing assembly. The helper types are
  /// still being added, so for now these tests guard the packaging story: the
  /// assembly must exist and must carry the same strong name key as Csla.
  /// </summary>
  [TestClass]
  public class PackagingTests
  {
    private static Assembly LoadCslaTesting() => Assembly.Load(new AssemblyName("Csla.Testing"));

    [TestMethod]
    public void CslaTestingAssemblyCanBeLoaded()
    {
      var assembly = LoadCslaTesting();

      assembly.Should().NotBeNull();
      assembly.GetName().Name.Should().Be("Csla.Testing");
    }

    [TestMethod]
    public void CslaTestingIsSignedWithTheCslaKey()
    {
      var cslaTestingKey = LoadCslaTesting().GetName().GetPublicKey();
      var cslaKey = typeof(BusinessBase<>).Assembly.GetName().GetPublicKey();

      cslaTestingKey.Should().NotBeNullOrEmpty();
      cslaTestingKey.Should().Equal(cslaKey);
    }
  }
}
