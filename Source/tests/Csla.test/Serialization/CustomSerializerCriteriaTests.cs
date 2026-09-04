//-----------------------------------------------------------------------
// <copyright file="CustomSerializerCriteriaTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for data portal criteria that rely on custom serializers</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.DataPortalClient;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Serialization.Mobile.CustomSerializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class CustomSerializerCriteriaTests
  {
    private static ApplicationContext CreateApplicationContext(bool registerPocoSerializer = true)
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o =>
        {
          if (registerPocoSerializer)
            o.UseMobileFormatter(o => o
              .CustomSerializers.Add(new TypeMap<object, PocoSerializer<SerializablePoco>>(PocoSerializer<SerializablePoco>.CanSerialize)));
        }));
      services.AddScoped<IContextManager, ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      return provider.GetRequiredService<ApplicationContext>();
    }

    private static T RoundTrip<T>(ApplicationContext applicationContext, T graph)
    {
      var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
      var data = formatter.Serialize(graph);
      return (T)formatter.Deserialize(data)!;
    }

    [TestMethod]
    public void PrimitiveCriteria_WithCustomSerializedValue_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var criteria = new PrimitiveCriteria(new SerializablePoco { Name = "test" });

      var clone = RoundTrip(applicationContext, criteria);

      Assert.IsInstanceOfType(clone.Value, typeof(SerializablePoco));
      Assert.AreEqual("test", ((SerializablePoco)clone.Value).Name);
    }

    [TestMethod]
    public void PrimitiveCriteria_WithIntValue_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = RoundTrip(applicationContext, new PrimitiveCriteria(42));

      Assert.AreEqual(42, clone.Value);
    }

    [TestMethod]
    public void PrimitiveCriteria_WithStringValue_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = RoundTrip(applicationContext, new PrimitiveCriteria("abc"));

      Assert.AreEqual("abc", clone.Value);
    }

    [TestMethod]
    public void PrimitiveCriteria_WithGuidValue_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var id = Guid.NewGuid();

      var clone = RoundTrip(applicationContext, new PrimitiveCriteria(id));

      Assert.AreEqual(id, clone.Value);
    }

    [TestMethod]
    public void PrimitiveCriteria_WithNull_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = RoundTrip(applicationContext, new PrimitiveCriteria(null));

      Assert.IsNull(clone.Value);
    }

    [TestMethod]
    public void PrimitiveCriteria_WithUnregisteredType_Throws()
    {
      var applicationContext = CreateApplicationContext(registerPocoSerializer: false);
      var criteria = new PrimitiveCriteria(new SerializablePoco { Name = "test" });

      Assert.ThrowsException<NotSupportedException>(() => RoundTrip(applicationContext, criteria));
    }

    [TestMethod]
    public void MobileList_WithCustomSerializedItems_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var list = new MobileList<object>([1, new SerializablePoco { Name = "test" }, "x"]);

      var clone = RoundTrip(applicationContext, list);

      Assert.AreEqual(3, clone.Count);
      Assert.AreEqual(1, clone[0]);
      Assert.IsInstanceOfType(clone[1], typeof(SerializablePoco));
      Assert.AreEqual("test", ((SerializablePoco)clone[1]).Name);
      Assert.AreEqual("x", clone[2]);
    }

    [TestMethod]
    public void MobileList_WithNativeItems_RoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var list = new MobileList<object>([1, "x", null]);

      var clone = RoundTrip(applicationContext, list);

      Assert.AreEqual(3, clone.Count);
      Assert.AreEqual(1, clone[0]);
      Assert.AreEqual("x", clone[1]);
      Assert.IsNull(clone[2]);
    }
  }
}
