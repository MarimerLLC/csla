//-----------------------------------------------------------------------
// <copyright file="CustomSerializerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class CustomSerializerTests
  {
    [TestMethod]
    public void CustomSerializerWorks()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .AddMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap
            {
              OriginalType = typeof(NonSerializableType),
              SerializerType = typeof(NonSerializableTypeSerializer)
            }))));
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);

      Assert.AreEqual(nonSerializable.Name, clone.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), $"{nameof(NonSerializableType)} != IMobileSerializer")]
    public void CustomSerializerRegisteredWrong()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .AddMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap
            {
              OriginalType = typeof(NonSerializableType),
              SerializerType = typeof(NonSerializableType)
            }))));
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), "Type NonSerializableType must implement IMobileObject")]
    public void CustomSerializerNotRegistered()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void CustomSerializerRegisteredBad()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .AddMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap
            {
              OriginalType = typeof(NonSerializableType),
              SerializerType = typeof(BadSerializer)
            }))));
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);
    }
  }

  public class NonSerializableType
  {
    public string Name { get; set; }
  }

  public class BadSerializer : IMobileSerializer
  {
    public object Deserialize(SerializationInfo info) => throw new NotSupportedException();
    public void Serialize(object obj, SerializationInfo info) => throw new NotSupportedException();
  }

  public class NonSerializableTypeSerializer : IMobileSerializer
  {
    public object Deserialize(SerializationInfo info)
    {
      var state = info.GetValue<string>("n");
      return new NonSerializableType { Name = state };
    }

    public void Serialize(object obj, SerializationInfo info)
    {
      if (obj is not NonSerializableType nonSerializableType)
        throw new ArgumentException("obj.GetType() != NonSerializableType", nameof(obj));

      info.AddValue("n", nonSerializableType.Name);
    }
  }
}
