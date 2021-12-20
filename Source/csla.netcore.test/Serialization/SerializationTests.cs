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
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using System.IO;

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
      Assert.AreEqual(typeof(Csla.Serialization.Mobile.MobileFormatter), serializer);
    }

    [TestMethod]
    public void UseMobileFormatter()
    {
      ApplicationContext applicationContext;
      applicationContext = ApplicationContextFactory.CreateTestApplicationContext();

      var serializer = ApplicationContext.SerializationFormatter;
      Assert.AreEqual(typeof(Csla.Serialization.Mobile.MobileFormatter), serializer);
      var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
      Assert.IsInstanceOfType(s, typeof(Csla.Serialization.Mobile.MobileFormatter));
    }

    [TestMethod]
    public void UseCustomFormatter()
    {
      try
      {
        IServiceCollection services = new ServiceCollection();
        services.AddCsla(o => o
          .Serialization(o => o.SerializationFormatter(typeof(CustomFormatter))));
        var provider = services.BuildServiceProvider();
        var applicationContext = provider.GetRequiredService<ApplicationContext>();

        var serializerType = ApplicationContext.SerializationFormatter;
        Assert.AreEqual(typeof(CustomFormatter), serializerType);
        var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
        Assert.IsInstanceOfType(s, typeof(CustomFormatter));
      }
      finally
      {
        // reset the serializer back to default
        IServiceCollection services = new ServiceCollection();
        services.AddCsla(o => o
          .Serialization(o => o.SerializationFormatter(typeof(Csla.Serialization.Mobile.MobileFormatter))));
        var provider = services.BuildServiceProvider();
        var applicationContext = provider.GetRequiredService<ApplicationContext>();
      }
    }
  }

  public class CustomFormatter : Csla.Serialization.ISerializationFormatter
  {
    public object Deserialize(Stream serializationStream) => throw new NotImplementedException();
    public object Deserialize(byte[] serializationStream) => throw new NotImplementedException();
    public void Serialize(Stream serializationStream, object graph) => throw new NotImplementedException();
    public byte[] Serialize(object graph) => throw new NotImplementedException();
  }
}
