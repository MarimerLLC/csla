//-----------------------------------------------------------------------
// <copyright file="DeclarativeCustomSerializerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for custom serializers declared with attributes</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Security.Claims;
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
  public class DeclarativeCustomSerializerTests
  {
    private static ServiceProvider CreateProvider(Action<MobileFormatterOptions> options = null)
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(s => s
          .UseMobileFormatter(m => options?.Invoke(m))));
      services.AddScoped<IContextManager, ApplicationContextManagerAsyncLocal>();
      return services.BuildServiceProvider();
    }

    private static ApplicationContext CreateApplicationContext(bool scanTestAssembly = true)
      => CreateProvider(m =>
      {
        if (scanTestAssembly)
          m.AddCustomSerializers(typeof(DeclarativeCustomSerializerTests).Assembly);
      }).GetRequiredService<ApplicationContext>();

    private static T Clone<T>(ApplicationContext applicationContext, T graph)
      => (T)new ObjectCloner(applicationContext).Clone(graph);

    private static T RoundTrip<T>(ApplicationContext applicationContext, T graph)
    {
      var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
      var data = formatter.Serialize(graph);
      return (T)formatter.Deserialize(data)!;
    }

    [TestMethod]
    public void GeneratorEmitsRegistrationsForTestAssembly()
    {
      var registrations = typeof(DeclarativeCustomSerializerTests).Assembly
        .GetCustomAttributes<MobileSerializerRegistrationAttribute>()
        .ToList();

      Assert.IsTrue(registrations.Any(r => r.Type == typeof(DeclaredType) && r.SerializerType == typeof(DeclaredTypeSerializer)));
      Assert.IsTrue(registrations.Any(r => r.Type == typeof(ForeignType) && r.SerializerType == typeof(ForeignTypeSerializer)));
      Assert.IsTrue(registrations.Any(r => r.Type == typeof(OtherForeignType) && r.SerializerType == typeof(ForeignTypeSerializer)));
      Assert.IsTrue(registrations.Any(r => r.Type == typeof(DeclaredPoco) && r.SerializerType == typeof(PocoSerializer<DeclaredPoco>)));
    }

    [TestMethod]
    public void CslaAssemblyRegistersClaimsPrincipalSerializer()
    {
      var registrations = typeof(MobileFormatter).Assembly
        .GetCustomAttributes<MobileSerializerRegistrationAttribute>()
        .ToList();

      Assert.AreEqual(1, registrations.Count(r => r.Type == typeof(ClaimsPrincipal)));
      Assert.AreEqual("ClaimsPrincipalSerializer", registrations.Single(r => r.Type == typeof(ClaimsPrincipal)).SerializerType.Name);
    }

    [TestMethod]
    public void UseMobileFormatterRegistersClaimsPrincipalByDefault()
    {
      var options = CreateProvider().GetRequiredService<MobileFormatterOptions>();

      Assert.AreEqual(1, options.CustomSerializers.Count(s => s.CanSerialize(typeof(ClaimsPrincipal))));
    }

    [TestMethod]
    public void ClaimsPrincipalRoundTripsWithDefaultConfiguration()
    {
      var applicationContext = CreateProvider().GetRequiredService<ApplicationContext>();
      var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "rocky")], "test"));

      var clone = Clone(applicationContext, principal);

      Assert.AreEqual("rocky", clone.Identity!.Name);
    }

    [TestMethod]
    public void TypeSideSerializerRoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = Clone(applicationContext, new DeclaredType { Name = "test" });

      Assert.AreEqual("test", clone.Name);
    }

    [TestMethod]
    public void SerializerSideSerializerRoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = Clone(applicationContext, new ForeignType { Name = "test" });
      var other = Clone(applicationContext, new OtherForeignType { Name = "other" });

      Assert.AreEqual("test", clone.Name);
      Assert.AreEqual("other", other.Name);
    }

    [TestMethod]
    public void TypeSidePocoSerializerRoundTrips()
    {
      var applicationContext = CreateApplicationContext();

      var clone = Clone(applicationContext, new DeclaredPoco { Name = "test" });

      Assert.AreEqual("test", clone.Name);
    }

    [TestMethod]
    public void CriteriaWithDeclaredTypeRoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var criteria = new PrimitiveCriteria(new DeclaredType { Name = "test" });

      var clone = RoundTrip(applicationContext, criteria);

      Assert.IsInstanceOfType(clone.Value, typeof(DeclaredType));
      Assert.AreEqual("test", ((DeclaredType)clone.Value).Name);
    }

    [TestMethod]
    public void MobileListWithDeclaredTypesRoundTrips()
    {
      var applicationContext = CreateApplicationContext();
      var list = new MobileList<object>([new DeclaredType { Name = "a" }, new ForeignType { Name = "b" }, 1]);

      var clone = RoundTrip(applicationContext, list);

      Assert.AreEqual("a", ((DeclaredType)clone[0]).Name);
      Assert.AreEqual("b", ((ForeignType)clone[1]).Name);
      Assert.AreEqual(1, clone[2]);
    }

    [TestMethod]
    public void DeclaredTypeWithoutScanThrows()
    {
      var applicationContext = CreateApplicationContext(scanTestAssembly: false);

      Assert.ThrowsException<InvalidOperationException>(() => Clone(applicationContext, new DeclaredType { Name = "test" }));
    }

    [TestMethod]
    public void DerivedTypeIsNotCovered()
    {
      var applicationContext = CreateApplicationContext();

      Assert.ThrowsException<InvalidOperationException>(() => Clone(applicationContext, new DerivedDeclaredType { Name = "test" }));
    }

    [TestMethod]
    public void AddCustomSerializersTwiceAddsNoDuplicates()
    {
      var options = new MobileFormatterOptions();
      options.AddCustomSerializers(typeof(DeclarativeCustomSerializerTests).Assembly);
      var count = options.CustomSerializers.Count;

      options.AddCustomSerializers(typeof(DeclarativeCustomSerializerTests).Assembly, typeof(DeclarativeCustomSerializerTests).Assembly);

      Assert.IsTrue(count >= 4);
      Assert.AreEqual(count, options.CustomSerializers.Count);
    }

    [TestMethod]
    public void AddCustomSerializersForCslaAssemblyAgainAddsNothing()
    {
      var options = CreateProvider().GetRequiredService<MobileFormatterOptions>();
      var count = options.CustomSerializers.Count;

      var result = options.AddCustomSerializers(typeof(MobileFormatter).Assembly);

      Assert.AreSame(options, result);
      Assert.AreEqual(count, options.CustomSerializers.Count);
    }

    [TestMethod]
    public void AddCustomSerializersWithNullThrows()
    {
      var options = new MobileFormatterOptions();

      Assert.ThrowsException<ArgumentNullException>(() => options.AddCustomSerializers(null));
      Assert.ThrowsException<ArgumentNullException>(() => options.AddCustomSerializers([null]));
    }
  }

  [MobileSerializer(typeof(DeclaredTypeSerializer))]
  public class DeclaredType
  {
    public string Name { get; set; }
  }

  public class DerivedDeclaredType : DeclaredType { }

  public class DeclaredTypeSerializer : IMobileSerializer
  {
    public object Deserialize(SerializationInfo info) => new DeclaredType { Name = info.GetValue<string>("n") };

    public void Serialize(object obj, SerializationInfo info) => info.AddValue("n", ((DeclaredType)obj).Name);
  }

  public class ForeignType
  {
    public string Name { get; set; }
  }

  public class OtherForeignType
  {
    public string Name { get; set; }
  }

  [MobileSerializerFor(typeof(ForeignType), typeof(OtherForeignType))]
  public class ForeignTypeSerializer : IMobileSerializer
  {
    public object Deserialize(SerializationInfo info)
    {
      var name = info.GetValue<string>("n");
      return info.GetValue<bool>("o")
        ? new OtherForeignType { Name = name }
        : new ForeignType { Name = name };
    }

    public void Serialize(object obj, SerializationInfo info)
    {
      switch (obj)
      {
        case ForeignType foreign:
          info.AddValue("n", foreign.Name);
          info.AddValue("o", false);
          break;
        case OtherForeignType other:
          info.AddValue("n", other.Name);
          info.AddValue("o", true);
          break;
        default:
          throw new ArgumentException(obj.GetType().FullName, nameof(obj));
      }
    }
  }

  [MobileSerializer(typeof(PocoSerializer<DeclaredPoco>))]
  public class DeclaredPoco
  {
    public string Name { get; set; }
  }
}
