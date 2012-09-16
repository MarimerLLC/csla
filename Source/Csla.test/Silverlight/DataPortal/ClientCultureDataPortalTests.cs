//-----------------------------------------------------------------------
// <copyright file="ClientCultureDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Set of integration tests for IAUthorizeDataPortal</summary>
//-----------------------------------------------------------------------
using Csla.Testing.Business.DataPortal;
using UnitDriven;
using Csla.DataPortalClient;
using System.Threading;
using System.Globalization;

#if SILVERLIGHT

#else
using System;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 
#endif


namespace Csla.Test.Silverlight.DataPortal
{
  /// <summary>
  /// Set of integration tests for IAUthorizeDataPortal
  /// Notice 2 roundtrips to the server in each test, first being Execute Command,
  /// and second being a Data Portal call being tested. (Create, Fatch, Delete, and Update).
  /// The reason for the first roundtrip is to set the appSetting in App.Config to value
  /// needed by the test
  /// </summary>
  [TestClass]
  public class ClientCultureDataPortalTests : TestBase
  {
    [TestInitialize]
    public void Setup()
    {
      #if SILVERLIGHT
      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy<>).AssemblyQualifiedName;
      #endif
    }

    [TestCleanup]
    public void Teardown()
    {

    }

    #region DataPortal.Create() Tests


#if SILVERLIGHT
    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_DoesNotAllow_Creation()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.DontAuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.CreateCompleted += ((sender, e1) =>
             {
               context.Assert.IsNotNull(e1.Error);
               //TODO: perhaps check to assure that exception type is Csla.Security.SecurityException
               //context.Assert.IsTrue(((Csla.DataPortalException)(e1.Error)).ErrorInfo.InnerError.ExceptionTypeName=="Csla.Security.SecurityException");
               context.Assert.Success();
             });
            dp.BeginCreate();
          });

      context.Complete();
    }
#else
#endif

    #endregion
  }
}