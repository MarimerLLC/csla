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
using AwesomeAssertions;
using AwesomeAssertions.Execution;
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

      using (new AssertionScope())
      {
        pi.HasDefaultValue.Should().BeFalse();
        pi.DefaultValue.Should().BeNull("the no-default form must not use the data binding friendly string.Empty");
        pi.RelationshipType.Should().Be(RelationshipTypes.None);
        pi.Should().BeAssignableTo<INotNullPropertyInfo>();
      }
    }

    [TestMethod]
    public void NotNullPropertyInfo_NoDefault_ValueTypeDefaultsToDefaultOfT()
    {
      var pi = new NotNullPropertyInfo<int>("Id", "Identifier");

      using (new AssertionScope())
      {
        pi.HasDefaultValue.Should().BeFalse();
        pi.DefaultValue.Should().Be(0);
        pi.FriendlyName.Should().Be("Identifier");
      }
    }

    [TestMethod]
    public void NotNullPropertyInfo_WithDefault_HasDefaultValue()
    {
      var pi = new NotNullPropertyInfo<string>("Name", null, null, string.Empty);

      using (new AssertionScope())
      {
        pi.HasDefaultValue.Should().BeTrue();
        pi.DefaultValue.Should().Be(string.Empty);
      }
    }

    [TestMethod]
    public void NotNullPropertyInfo_NullDefault_Throws()
    {
      FluentActions.Invoking(() => new NotNullPropertyInfo<string>("Name", null, null, null!)).Should().ThrowExactly<ArgumentNullException>();
    }

    [TestMethod]
    public void Factory_CreateNotNull_ReturnsNotNullPropertyInfo()
    {
      var factory = PropertyInfoFactory.Factory;

      var noDefault = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name");
      var friendly = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", "Friendly");
      var withDefault = factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", null, "x");

      using (new AssertionScope())
      {
        noDefault.HasDefaultValue.Should().BeFalse();
        friendly.HasDefaultValue.Should().BeFalse();
        friendly.FriendlyName.Should().Be("Friendly");
        withDefault.HasDefaultValue.Should().BeTrue();
        withDefault.DefaultValue.Should().Be("x");
      }
    }

    [TestMethod]
    public void Factory_CreateNotNull_NullArguments_Throw()
    {
      var factory = PropertyInfoFactory.Factory;

      using (new AssertionScope())
      {
        FluentActions.Invoking(() => factory.CreateNotNull<string>(null!, "Name")).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), null!)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => factory.CreateNotNull<string>(typeof(NotNullRegistrationRoot), "Name", null, null!)).Should().ThrowExactly<ArgumentNullException>();
      }
    }

    [TestMethod]
    public void BusinessBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      using (new AssertionScope())
      {
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.ByNameProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.ByLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.FriendlyProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.FriendlyLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.DefaultedProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.DefaultedLambdaProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationRoot), NotNullRegistrationRoot.InstanceProperty, hasDefault: false);

        NotNullRegistrationRoot.FriendlyProperty.FriendlyName.Should().Be("Friendly name");
        NotNullRegistrationRoot.DefaultedProperty.DefaultValue.Should().Be("default");
      }
    }

    [TestMethod]
    public void BusinessBase_RegisterNotNullProperty_TwoArgumentStringBindsToFriendlyName()
    {
      // (name, friendlyName) is the no-default form, also for P == string
      using (new AssertionScope())
      {
        NotNullRegistrationRoot.FriendlyProperty.HasDefaultValue.Should().BeFalse();
        NotNullRegistrationRoot.FriendlyProperty.DefaultValue.Should().BeNull();
      }
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void BusinessBase_RegisterNotNullPropertyByName_InvalidName_ThrowsArgumentException(string? propertyName)
    {
      FluentActions.Invoking(() => NotNullRegistrationRoot.RegisterByName(propertyName))
        .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void BusinessBase_RegisterNotNullPropertyByNameAndFriendlyName_InvalidName_ThrowsArgumentException(string? propertyName)
    {
      FluentActions.Invoking(() => NotNullRegistrationRoot.RegisterByNameAndFriendlyName(propertyName))
        .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void BusinessBase_RegisterNotNullPropertyWithDefault_InvalidName_ThrowsArgumentException(string? propertyName)
    {
      FluentActions.Invoking(() => NotNullRegistrationRoot.RegisterWithDefault(propertyName))
        .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
    }

    [TestMethod]
    public void ReadOnlyBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      using (new AssertionScope())
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
    }

    [TestMethod]
    public void ReadOnlyBase_RegisterNotNullProperty_EmptyName_Throws()
    {
      FluentActions.Invoking(() => NotNullRegistrationInfo.RegisterEmptyName()).Should().ThrowExactly<ArgumentException>();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ReadOnlyBase_RegisterNotNullPropertyByNameOverloads_InvalidName_ThrowArgumentException(string? propertyName)
    {
      using (new AssertionScope())
      {
        FluentActions.Invoking(() => NotNullRegistrationInfo.RegisterByName(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
        FluentActions.Invoking(() => NotNullRegistrationInfo.RegisterByNameAndFriendlyName(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
        FluentActions.Invoking(() => NotNullRegistrationInfo.RegisterWithDefault(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
      }
    }

    [TestMethod]
    public void CommandBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      using (new AssertionScope())
      {
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.ByNameProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.ByLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.FriendlyProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.FriendlyLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.DefaultedProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.DefaultedLambdaProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationCommand), NotNullRegistrationCommand.InstanceProperty, hasDefault: false);
      }
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void CommandBase_RegisterNotNullPropertyByNameOverloads_InvalidName_ThrowArgumentException(string? propertyName)
    {
      using (new AssertionScope())
      {
        FluentActions.Invoking(() => NotNullRegistrationCommand.RegisterByName(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
        FluentActions.Invoking(() => NotNullRegistrationCommand.RegisterByNameAndFriendlyName(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
        FluentActions.Invoking(() => NotNullRegistrationCommand.RegisterWithDefault(propertyName))
          .Should().ThrowExactly<ArgumentException>().WithParameterName("propertyName");
      }
    }

    [TestMethod]
    public void ManagedObjectBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      using (new AssertionScope())
      {
        AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.ByLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.FriendlyLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.DefaultedLambdaProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationCriteria), NotNullRegistrationCriteria.InstanceProperty, hasDefault: false);
      }
    }

    [TestMethod]
    public void BusinessDocumentBase_RegisterNotNullProperty_RegistersAllShapes()
    {
      using (new AssertionScope())
      {
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.ByNameProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.ByLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.FriendlyProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.FriendlyLambdaProperty, hasDefault: false);
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.DefaultedProperty, hasDefault: true);
        AssertRegistered(typeof(NotNullRegistrationDocument), NotNullRegistrationDocument.DefaultedLambdaProperty, hasDefault: true);
      }
    }

    [TestMethod]
    public void BusinessDocumentBase_RegisterNotNullProperty_EmptyName_Throws()
    {
      FluentActions.Invoking(() => NotNullRegistrationDocument.RegisterEmptyName()).Should().ThrowExactly<ArgumentException>();
    }

    private static void AssertRegistered(Type type, IPropertyInfo expected, bool hasDefault)
    {
      var registered = PropertyInfoManager.GetRegisteredProperties(type);
      registered.Should().Contain(expected, "{0} must be registered on {1}", expected.Name, type.Name);
      expected.Should().BeAssignableTo<INotNullPropertyInfo>();
      var hasDefaultValue = (bool)expected.GetType().GetProperty("HasDefaultValue")!.GetValue(expected)!;
      hasDefaultValue.Should().Be(hasDefault, "that is the HasDefaultValue {0} was registered with", expected.Name);
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

    public static void RegisterByName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!);
    public static void RegisterByNameAndFriendlyName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name");
    public static void RegisterWithDefault(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name", "default");
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

    public static void RegisterByName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!);
    public static void RegisterByNameAndFriendlyName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name");
    public static void RegisterWithDefault(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name", "default");
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

    public static void RegisterByName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!);
    public static void RegisterByNameAndFriendlyName(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name");
    public static void RegisterWithDefault(string? propertyName) => RegisterNotNullProperty<string>(propertyName!, "Friendly name", "default");
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
