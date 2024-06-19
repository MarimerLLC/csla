//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;

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
  public class SerializationTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void CorrectDefaultSerializer()
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var serializer = applicationContext.GetRequiredService<ISerializationFormatter>();
      Assert.IsTrue(serializer.GetType() == typeof(MobileFormatter));
    }

    [TestMethod]
    public void UseCustomFormatter()
    {
      try
      {
        IServiceCollection services = new ServiceCollection();
        services.AddCsla(o => o
          .Serialization(o => o.UseSerializationFormatter<CustomFormatter>()));
        var provider = services.BuildServiceProvider();
        var applicationContext = provider.GetRequiredService<ApplicationContext>();
        var options = applicationContext.GetRequiredService<Csla.Configuration.CslaOptions>();

        var serializer = applicationContext.GetRequiredService<ISerializationFormatter>();
        var serializerType = serializer.GetType();
        Assert.AreEqual(typeof(CustomFormatter), serializerType);
        var s = Csla.Serialization.SerializationFormatterFactory.GetFormatter(applicationContext);
        Assert.IsInstanceOfType(s, typeof(CustomFormatter));
      }
      finally
      {
        // reset the serializer back to default
        IServiceCollection services = new ServiceCollection();
        services.AddCsla(o => o
          .Serialization(o => o.UseMobileFormatter()));
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
