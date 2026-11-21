//-----------------------------------------------------------------------
// <copyright file="NotNullPropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for NotNullPropertyInfo and RegisterNotNullProperty</summary>
//-----------------------------------------------------------------------

#nullable enable

using Csla.Core;
using Csla.Core.FieldManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullPropertyInfoTests
  {
    [TestMethod]
    public void NotNullPropertyInfo_NoDefault_HasNullDefaultValue()
    {
      var pi = new NotNullPropertyInfo<string>("Name");

      Assert.IsFalse(pi.HasDefaultValue);
      Assert.IsNull(pi.DefaultValue, "No-default form must not use the data binding friendly string.Empty");
      Assert.AreEqual(RelationshipTypes.None, pi.RelationshipType);
      Assert.IsInstanceOfType<INotNullPropertyInfo>(pi);
    }

    [TestMethod]
    public void NotNullPropertyInfo_NoDefault_ValueTypeDefaultsToDefaultOfT()
    {
      var pi = new NotNullPropertyInfo<int>("Id", "Identifier");

      Assert.IsFalse(pi.HasDefaultValue);
      Assert.AreEqual(0, pi.DefaultValue);
      Assert.AreEqual("Identifier", pi.FriendlyName);
    }

    [TestMethod]
    public void NotNullPropertyInfo_WithDefault_HasDefaultValue()
    {
      var pi = new NotNullPropertyInfo<string>("Name", null, null, string.Empty);

      Assert.IsTrue(pi.HasDefaultValue);
      Assert.AreEqual(string.Empty, pi.DefaultValue);
    }

    [TestMethod]
    public void NotNullPropertyInfo_NullDefault_Throws()
    {
      Assert.ThrowsExactly<ArgumentNullException>(() => new NotNullPropertyInfo<string>("Name", null, null, null!));
    }

    [TestMethod]
    public void Factory_CreateNotNull_ReturnsNotNullPropertyInfo()
    {
      var factory = PropertyInfoFactory.Factory;

      var noDefault = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name");
      var friendly = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", "Friendly");
      var withDefault = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", null, "x");

      Assert.IsFalse(noDefault.HasDefaultValue);
      Assert.IsFalse(friendly.HasDefaultValue);
      Assert.AreEqual("Friendly", friendly.FriendlyName);
      Assert.IsTrue(withDefault.HasDefaultValue);
      Assert.AreEqual("x", withDefault.DefaultValue);
    }

    [TestMethod]
    public void Factory_CreateNotNull_NullArguments_Throw()
    {
      var factory = PropertyInfoFactory.Factory;

      Assert.ThrowsExactly<ArgumentNullException>(() => factory.CreateNotNull<string>(null!, "Name"));
      Assert.ThrowsExactly<ArgumentNullException>(() => factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), null!));
      Assert.ThrowsExactly<ArgumentNullException>(() => factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", null, null!));
    }

    [TestMethod]
    public void BusinessBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.ByNameProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.ByLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.FriendlyProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.FriendlyLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.DefaultedProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.DefaultedLambdaProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.InstanceProperty, hasDefault: false);

      Assert.AreEqual("Friendly name", NotNullRegistrationRoot.FriendlyProperty.FriendlyName);
      Assert.AreEqual("default", NotNullRegistrationRoot.DefaultedProperty.DefaultValue);
    }

    [TestMethod]
    public void BusinessBase_RegisterNotNullProperty_TwoArgumentStringBindsToFriendlyName()
    {
      // (name, friendlyName) is the no-default form, also for P == string
      Assert.IsFalse(NotNullRegistrationRoot.FriendlyProperty.HasDefaultValue);
      Assert.IsNull(NotNullRegistrationRoot.FriendlyProperty.DefaultValue);
    }

    [TestMethod]
    public void ReadOnlyBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.ByNameProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.ByLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.FriendlyProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.FriendlyLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.DefaultedProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.DefaultedLambdaProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.InstanceProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationInfo), NotNullRegistrationInfo.TypedInstanceProperty, hasDefault: false);
    }

    [TestMethod]
    public void ReadOnlyBase_RegisterNotNullProperty_EmptyName_Throws()
    {
      Assert.ThrowsExactly<ArgumentException>(() => NotNullRegistrationInfo.RegisterEmptyName());
    }

    [TestMethod]
    public void CommandBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.ByNameProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.ByLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.FriendlyProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.FriendlyLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.DefaultedProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.DefaultedLambdaProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.InstanceProperty, hasDefault: false);
    }

    [TestMethod]
    public void ManagedObjectBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.ByLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.FriendlyLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.DefaultedLambdaProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.InstanceProperty, hasDefault: false);
    }

    [TestMethod]
    public void BusinessDocumentBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.ByNameProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.ByLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.FriendlyProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.FriendlyLambdaProperty, hasDefault: false);
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.DefaultedProperty, hasDefault: true);
      AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.DefaultedLambdaProperty, hasDefault: true);
    }

    [TestMethod]
    public void BusinessDocumentBase_RegisterNotNullProperty_EmptyName_Throws()
    {
      Assert.ThrowsExactly<ArgumentException>(() => NotNullRegistrationDocument.RegisterEmptyName());
    }

    private static void AssertRegistered(Type type, IPropertyInfo expected, bool hasDefault)
    {
      var registered = PropertyInfoManager.GetRegisteredProperties(type);
      Assert.IsTrue(registered.Contains(expected), $"{expected.Name} is not registered on {type.Name}");
      Assert.IsInstanceOfType<INotNullPropertyInfo>(expected);
      var hasDefaultValue = (bool)expected.GetType().GetProperty("HasDefaultValue")!.GetValue(expected)!;
      Assert.AreEqual(hasDefault, hasDefaultValue, $"{expected.Name}.HasDefaultValue");
    }
  }

  [Serializable]
  public class NotNullRegistrationRoot : BusinessBase<NotNullRegistrationRoot>
  {
    public static readonly NotNullPropertyInfo<string> ByNameProperty = RegisterNotNullProperty<string>(nameof(ByName));
    public string ByName => GetProperty(ByNameProperty)!;

    public static readonly NotNullPropertyInfo<string> ByLambdaProperty = RegisterNotNullProperty<string>(c => c.ByLambda);
    public string ByLambda => GetProperty(ByLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyProperty = RegisterNotNullProperty<string>(nameof(Friendly), "Friendly name");
    public string Friendly => GetProperty(FriendlyProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyLambdaProperty = RegisterNotNullProperty<string>(c => c.FriendlyLambda, "Friendly lambda");
    public string FriendlyLambda => GetProperty(FriendlyLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> DefaultedProperty = RegisterNotNullProperty<string>(nameof(Defaulted), null, "default");
    public string Defaulted => GetProperty(DefaultedProperty)!;

    public static readonly NotNullPropertyInfo<int> DefaultedLambdaProperty = RegisterNotNullProperty<int>(c => c.DefaultedLambda, null, 42);
    public int DefaultedLambda => GetProperty(DefaultedLambdaProperty);

    public static readonly NotNullPropertyInfo<string> InstanceProperty = RegisterNotNullProperty(new NotNullPropertyInfo<string>(nameof(Instance)));
    public string Instance => GetProperty(InstanceProperty)!;
  }

  [Serializable]
  public class NotNullRegistrationInfo : ReadOnlyBase<NotNullRegistrationInfo>
  {
    public static readonly NotNullPropertyInfo<string> ByNameProperty = RegisterNotNullProperty<string>(nameof(ByName));
    public string ByName => GetProperty(ByNameProperty)!;

    public static readonly NotNullPropertyInfo<string> ByLambdaProperty = RegisterNotNullProperty<string>(c => c.ByLambda);
    public string ByLambda => GetProperty(ByLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyProperty = RegisterNotNullProperty<string>(nameof(Friendly), "Friendly name");
    public string Friendly => GetProperty(FriendlyProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyLambdaProperty = RegisterNotNullProperty<string>(c => c.FriendlyLambda, "Friendly lambda");
    public string FriendlyLambda => GetProperty(FriendlyLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> DefaultedProperty = RegisterNotNullProperty<string>(nameof(Defaulted), null, "default");
    public string Defaulted => GetProperty(DefaultedProperty)!;

    public static readonly NotNullPropertyInfo<int> DefaultedLambdaProperty = RegisterNotNullProperty<int>(c => c.DefaultedLambda, null, 42);
    public int DefaultedLambda => GetProperty(DefaultedLambdaProperty);

    public static readonly NotNullPropertyInfo<string> InstanceProperty = RegisterNotNullProperty(new NotNullPropertyInfo<string>(nameof(Instance)));
    public string Instance => GetProperty(InstanceProperty)!;

    public static readonly NotNullPropertyInfo<string> TypedInstanceProperty = RegisterNotNullProperty(typeof(NotNullRegistrationInfo), new NotNullPropertyInfo<string>(nameof(TypedInstance)));
    public string TypedInstance => GetProperty(TypedInstanceProperty)!;

    public static void RegisterEmptyName() => RegisterNotNullProperty<string>(" ");
  }

  [Serializable]
  public class NotNullRegistrationCommand : CommandBase<NotNullRegistrationCommand>
  {
    public static readonly NotNullPropertyInfo<string> ByNameProperty = RegisterNotNullProperty<string>(nameof(ByName));
    public string ByName => ReadProperty(ByNameProperty)!;

    public static readonly NotNullPropertyInfo<string> ByLambdaProperty = RegisterNotNullProperty<string>(c => c.ByLambda);
    public string ByLambda => ReadProperty(ByLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyProperty = RegisterNotNullProperty<string>(nameof(Friendly), "Friendly name");
    public string Friendly => ReadProperty(FriendlyProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyLambdaProperty = RegisterNotNullProperty<string>(c => c.FriendlyLambda, "Friendly lambda");
    public string FriendlyLambda => ReadProperty(FriendlyLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> DefaultedProperty = RegisterNotNullProperty<string>(nameof(Defaulted), null, "default");
    public string Defaulted => ReadProperty(DefaultedProperty)!;

    public static readonly NotNullPropertyInfo<int> DefaultedLambdaProperty = RegisterNotNullProperty<int>(c => c.DefaultedLambda, null, 42);
    public int DefaultedLambda => ReadProperty(DefaultedLambdaProperty);

    public static readonly NotNullPropertyInfo<string> InstanceProperty = RegisterNotNullProperty(new NotNullPropertyInfo<string>(nameof(Instance)));
    public string Instance => ReadProperty(InstanceProperty)!;
  }

  [Serializable]
  public class NotNullRegistrationCriteria : ManagedObjectBase
  {
    public static readonly NotNullPropertyInfo<string> ByLambdaProperty = RegisterNotNullProperty<NotNullRegistrationCriteria, string>(c => c.ByLambda);
    public string ByLambda => ReadProperty(ByLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyLambdaProperty = RegisterNotNullProperty<NotNullRegistrationCriteria, string>(c => c.FriendlyLambda, "Friendly lambda");
    public string FriendlyLambda => ReadProperty(FriendlyLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> DefaultedLambdaProperty = RegisterNotNullProperty<NotNullRegistrationCriteria, string>(c => c.DefaultedLambda, null, "default");
    public string DefaultedLambda => ReadProperty(DefaultedLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> InstanceProperty = RegisterNotNullProperty(typeof(NotNullRegistrationCriteria), new NotNullPropertyInfo<string>(nameof(Instance)));
    public string Instance => ReadProperty(InstanceProperty)!;
  }

  [Serializable]
  public class NotNullRegistrationDocument : BusinessDocumentBase<NotNullRegistrationDocument, NotNullRegistrationLine>
  {
    public static readonly NotNullPropertyInfo<string> ByNameProperty = RegisterNotNullProperty<string>(nameof(ByName));
    public string ByName => GetProperty(ByNameProperty)!;

    public static readonly NotNullPropertyInfo<string> ByLambdaProperty = RegisterNotNullProperty<string>(c => c.ByLambda);
    public string ByLambda => GetProperty(ByLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyProperty = RegisterNotNullProperty<string>(nameof(Friendly), "Friendly name");
    public string Friendly => GetProperty(FriendlyProperty)!;

    public static readonly NotNullPropertyInfo<string> FriendlyLambdaProperty = RegisterNotNullProperty<string>(c => c.FriendlyLambda, "Friendly lambda");
    public string FriendlyLambda => GetProperty(FriendlyLambdaProperty)!;

    public static readonly NotNullPropertyInfo<string> DefaultedProperty = RegisterNotNullProperty<string>(nameof(Defaulted), null, "default");
    public string Defaulted => GetProperty(DefaultedProperty)!;

    public static readonly NotNullPropertyInfo<int> DefaultedLambdaProperty = RegisterNotNullProperty<int>(c => c.DefaultedLambda, null, 42);
    public int DefaultedLambda => GetProperty(DefaultedLambdaProperty);

    public static void RegisterEmptyName() => RegisterNotNullProperty<string>(" ");
  }

  [Serializable]
  public class NotNullRegistrationLine : BusinessBase<NotNullRegistrationLine>
  {
  }
}
