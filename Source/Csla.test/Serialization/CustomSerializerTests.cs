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
using Csla.Serialization.Mobile.CustomSerializers;

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
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(
            new TypeMap<NonSerializableType, NonSerializableTypeSerializer>(NonSerializableTypeSerializer.CanSerialize))
            )));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var options = applicationContext.GetRequiredService<CslaOptions>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);

      Assert.AreEqual(nonSerializable.Name, clone.Name);
    }

    [TestMethod]
    public void CustomSerializerInterfaceMap()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(
            new TypeMap<INonSerializableType, NonSerializableTypeSerializer>((t) => t.IsAssignableTo(typeof(INonSerializableType))))
            )));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (NonSerializableType)cloner.Clone(nonSerializable);

      Assert.AreEqual(nonSerializable.Name, clone.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CustomSerializerNotRegistered()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      _ = (NonSerializableType)cloner.Clone(nonSerializable);
    }

    [TestMethod]
    [ExpectedException(typeof(MobileFormatterException), "CustomSerializerType:Csla.Test.Serialization.BadSerializer; objectType:Csla.Test.Serialization.NonSerializableType")]
    public void CustomSerializerRegisteredBad()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap<NonSerializableType, BadSerializer>(BadSerializer.CanSerialize)))));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var nonSerializable = new NonSerializableType { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      _ = (NonSerializableType)cloner.Clone(nonSerializable);
    }

    [TestMethod]
    public void SerializePoco()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap<object, PocoSerializer<SerializablePoco>>(PocoSerializer<SerializablePoco>.CanSerialize)))));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var poco = new SerializablePoco { Name = "test" };
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (SerializablePoco)cloner.Clone(poco);
      Assert.AreEqual(poco.Name, clone.Name);
    }

    [TestMethod]
    public async Task ClonePoco()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap<object, PocoSerializer<SerializablePoco>>(PocoSerializer<SerializablePoco>.CanSerialize)))));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var portal = applicationContext.GetRequiredService<IDataPortal<PocoContainer>>();
      var pocoContainer = await portal.CreateAsync();
      Assert.AreEqual("test", pocoContainer.Poco.Name);
      var cloner = new Core.ObjectCloner(applicationContext);
      var clone = (PocoContainer)cloner.Clone(pocoContainer);
      Assert.AreEqual(pocoContainer.Poco.Name, clone.Poco.Name);
    }

    [TestMethod]
    public void NLevelPoco()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap<object, PocoSerializer<SerializablePoco>>(PocoSerializer<SerializablePoco>.CanSerialize)))));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var portal = applicationContext.GetRequiredService<IDataPortal<PocoContainer>>();
      var pocoContainer = portal.Create();
      Assert.AreEqual("test", pocoContainer.Poco.Name);
      pocoContainer.BeginEdit();
      pocoContainer.Poco.Name = "test2";
      pocoContainer.CancelEdit();
      Assert.AreEqual("test", pocoContainer.Poco.Name);
    }

    [TestMethod]
    public void NLevelNullPoco()
    {
      var services = new ServiceCollection();
      services.AddCsla(o => o
        .Serialization(o => o
          .UseMobileFormatter(o => o
            .CustomSerializers.Add(new TypeMap<object, PocoSerializer<SerializablePoco>>(PocoSerializer<SerializablePoco>.CanSerialize)))));
      services.AddScoped<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerAsyncLocal>();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var portal = applicationContext.GetRequiredService<IDataPortal<PocoContainer>>();
      var pocoContainer = portal.Create();
      pocoContainer.Poco = null;
      Assert.IsNull(pocoContainer.Poco);
      pocoContainer.BeginEdit();
      pocoContainer.Poco = new SerializablePoco { Name = "test" };
      pocoContainer.CancelEdit();
      Assert.IsNull(pocoContainer.Poco);
    }
  }

  public class PocoContainer : BusinessBase<PocoContainer>
  {
    public static readonly PropertyInfo<SerializablePoco> PocoProperty = RegisterProperty<SerializablePoco>(c => c.Poco);
    public SerializablePoco Poco
    {
      get => GetProperty(PocoProperty);
      set => SetProperty(PocoProperty, value);
    }

    [Create]
    private void Create()
    {
      Poco = new SerializablePoco { Name = "test" };
    }
  }

  public class SerializablePoco
  {
    public string Name { get; set; }
  }

  public interface INonSerializableType
  {
    string Name { get; set; }
  }
  
  public class NonSerializableType : INonSerializableType
  {
    public string Name { get; set; }
  }

  public class BadSerializer : IMobileSerializer
  {
    public static bool CanSerialize(Type type) => true;

    public object Deserialize(SerializationInfo info) => throw new NotSupportedException();
    public void Serialize(object obj, SerializationInfo info) => throw new NotSupportedException();
  }

  public class NonSerializableTypeSerializer : IMobileSerializer
  {
    public static bool CanSerialize(Type type) => type == typeof(NonSerializableType);

    public object Deserialize(SerializationInfo info)
    {
      var state = info.GetValue<string>("n");
      return new NonSerializableType { Name = state };
    }

    public void Serialize(object obj, SerializationInfo info)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (!CanSerialize(obj.GetType()))
        throw new ArgumentException($"{obj.GetType()} != NonSerializableType", nameof(obj));

      var nonSerializableType = (NonSerializableType)obj;
      info.AddValue("n", nonSerializableType.Name);
    }
  }
}
