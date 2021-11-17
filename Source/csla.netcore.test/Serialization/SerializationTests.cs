//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.TestHelpers;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 
using Csla;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class SerializationTests
  {
    [TestMethod]
    public void CorrectDefaultSerializer()
    {
      var serializer = ApplicationContext.SerializationFormatter;
      Assert.AreEqual(ApplicationContext.SerializationFormatters.MobileFormatter, serializer);
    }

    [TestMethod]
    public void UseMobileFormatter()
    {
      ApplicationContext applicationContext;
      applicationContext = ApplicationContextFactory.CreateTestApplicationContext();
      try
      {
        Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
        var serializer = ApplicationContext.SerializationFormatter;
        Assert.AreEqual(ApplicationContext.SerializationFormatters.MobileFormatter, serializer);
        var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
        Assert.IsInstanceOfType(s, typeof(Csla.Serialization.Mobile.MobileFormatter));
      }
      finally
      {
        Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
      }
    }

    // BinaryFormatter dropped as of CSLA 6?
    //[TestMethod]
    //public void UseBinaryFormatter()
    //{
    //  ApplicationContext applicationContext;
    //  applicationContext = ApplicationContextFactory.CreateTestApplicationContext();
    //  try
    //  {
    //    Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "BinaryFormatter";
    //    var serializer = ApplicationContext.SerializationFormatter;
    //    Assert.AreEqual(ApplicationContext.SerializationFormatters.BinaryFormatter, serializer);
    //    var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
    //    Assert.IsInstanceOfType(s, typeof(Csla.Serialization.BinaryFormatterWrapper));
    //  }
    //  finally
    //  {
    //    Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
    //  }
    //}

    [TestMethod]
    public void UseCustomFormatter()
    {
      ApplicationContext applicationContext;
      applicationContext = ApplicationContextFactory.CreateTestApplicationContext();
      try
      {
        Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "Csla.Serialization.Mobile.MobileFormatter,Csla";
        var serializer = ApplicationContext.SerializationFormatter;
        Assert.AreEqual(ApplicationContext.SerializationFormatters.CustomFormatter, serializer);
        var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
        Assert.IsInstanceOfType(s, typeof(Csla.Serialization.Mobile.MobileFormatter));
      }
      finally
      {
        Csla.Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
      }
    }
  }
}
