//-----------------------------------------------------------------------
// <copyright file="PropertGetSetTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test created for Bug Tracker Item 64</summary>
//-----------------------------------------------------------------------
using UnitDriven;
using System.Threading.Tasks;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Silverlight.PropertyGetSet
{
  [TestClass]
  public class PropertGetSetTests : TestBase
  {
#if DEBUG
    /// <remarks>
    /// Test created for Bug Tracker Item 64
    /// Currently the properties we try to load that were declared and registered 
    /// in base class fail to load throwing following exception:
    /// {"Property load or set failed for property [Property Name] (One or more properties are not registered for this type)"}
    /// This is due to property being registered only with Base type
    /// </remarks>
    [TestMethod]

    public async Task ProperyInfoDeclaredInBaseClassShouldLoadInAnotherDomain()
    {
      var item = new InheritedLoadPropertySet();
      item = await item.SaveAsync();
      Assert.AreEqual(1, item.Id);
    }
#endif
  }
}