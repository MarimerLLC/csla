//-----------------------------------------------------------------------
// <copyright file="NotNullReadWriteTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for reads and writes of not-null properties</summary>
//-----------------------------------------------------------------------

#nullable enable

using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullReadWriteTests
  {
    private static CslaTestHost _testHost = default!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    private static T New<T>() => _testHost.ApplicationContext.CreateInstanceDI<T>();

    // Reads

    [TestMethod]
    public void BusinessBase_GetProperty_UnloadedNoDefault_ReturnsNullWithoutStoringIt()
    {
      var obj = New<NotNullEditRoot>();

      using (new AssertionScope())
      {
        obj.Name.Should().BeNull();
        ((IManageProperties)obj).FieldExists(NotNullEditRoot.NameProperty).Should().BeFalse();
      }
    }

    [TestMethod]
    public void BusinessBase_ReadProperty_UnloadedDefault_ReturnsAndStoresDefault()
    {
      var obj = New<NotNullEditRoot>();

      obj.ReadCode().Should().Be("unknown");
      ((IManageProperties)obj).FieldExists(NotNullEditRoot.CodeProperty).Should().BeTrue();
    }

    [TestMethod]
    public void BusinessBase_GetProperty_DeniedRead_ReturnsDefault()
    {
      var obj = New<NotNullEditRoot>();
      obj.LoadSecret("hidden");

      obj.Secret.Should().BeNull("authorization takes precedence over nullability");
    }

    [TestMethod]
    public void ReadOnlyBase_ReadProperty_UnloadedNoDefault_ReturnsNullWithoutStoringIt()
    {
      var obj = New<NotNullInfo>();

      obj.Name.Should().BeNull();
      ((IManageProperties)obj).FieldExists(NotNullInfo.NameProperty).Should().BeFalse();
      obj.Code.Should().Be("unknown");
    }

    [TestMethod]
    public void ManagedObjectBase_ReadProperty_UnloadedNoDefault_ReturnsNull()
    {
      var obj = New<NotNullCommand>();

      using (new AssertionScope())
      {
        obj.Result.Should().BeNull();
        ((IManageProperties)obj).FieldExists(NotNullCommand.ResultProperty).Should().BeFalse();
      }
    }

    // Typed writes

    [TestMethod]
    public void BusinessBase_SetProperty_NullString_StoresEmptyString()
    {
      var obj = New<NotNullEditRoot>();

      obj.Name = null!;

      obj.Name.Should().Be(string.Empty);
    }

    [TestMethod]
    public void BusinessBase_SetProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      FluentActions.Invoking(() => obj.Data = null!).Should().ThrowExactly<ArgumentNullException>()
        .WithParameterName(nameof(NotNullEditRoot.Data));
    }

    [TestMethod]
    public void BusinessBase_SetProperty_Value_Stores()
    {
      var obj = New<NotNullEditRoot>();

      obj.Name = "a";
      obj.Data = [1];

      using (new AssertionScope())
      {
        obj.Name.Should().Be("a");
        obj.Data.Should().Equal(new byte[] { 1 });
      }
    }

    // A not-null string never throws for null: every write path stores
    // string.Empty, as the typed setter always has (Windows Forms data binding
    // needs a string that never returns null, and a setter that never throws)

    [TestMethod]
    public void BusinessBase_StringWritePaths_Null_StoreEmptyString()
    {
      var writes = new (string Path, Action<NotNullEditRoot> Write)[]
      {
        ("LoadProperty", o => o.LoadName(null)),
        ("LoadPropertyMarkDirty", o => o.LoadNameMarkDirty(null)),
        ("LoadPropertyConvert", o => o.LoadNameConvert(null)),
        ("untyped LoadProperty", o => o.LoadUntyped(NotNullEditRoot.NameProperty, null)),
        ("untyped LoadPropertyMarkDirty", o => o.LoadUntypedMarkDirty(NotNullEditRoot.NameProperty, null)),
        ("untyped SetProperty", o => o.SetUntyped(NotNullEditRoot.NameProperty, null)),
        ("untyped SetProperty, value converting to null", o => o.SetUntyped(NotNullEditRoot.NameProperty, new NullToString())),
        ("SetPropertyConvert", o => o.SetNameConvert(null)),
      };

      foreach (var (path, write) in writes)
      {
        var obj = New<NotNullEditRoot>();
        obj.LoadName("old");

        write(obj);

        obj.Name.Should().Be(string.Empty, "the {0} write path stores string.Empty for null", path);
      }
    }

    [TestMethod]
    public void BusinessBase_NonStringWritePaths_Null_ThrowArgumentNullException()
    {
      var writes = new (string Path, Action<NotNullEditRoot> Write)[]
      {
        ("SetProperty", o => o.Data = null!),
        ("LoadPropertyMarkDirty", o => o.LoadDataMarkDirty(null)),
        ("LoadPropertyConvert", o => o.LoadDataConvert(null)),
        ("SetPropertyConvert", o => o.SetDataConvert(null)),
        ("untyped LoadProperty", o => o.LoadUntyped(NotNullEditRoot.DataProperty, null)),
        ("untyped LoadPropertyMarkDirty", o => o.LoadUntypedMarkDirty(NotNullEditRoot.DataProperty, null)),
        ("untyped SetProperty", o => o.SetUntyped(NotNullEditRoot.DataProperty, null)),
        // the value converts to null (TypeConverter fallback), so the converted value is guarded
        ("untyped SetProperty, value converting to null", o => o.SetUntyped(NotNullEditRoot.TokenProperty, "x")),
      };

      foreach (var (path, write) in writes)
      {
        var obj = New<NotNullEditRoot>();

        var ex = FluentActions.Invoking(() => write(obj)).Should().Throw<ArgumentNullException>("the {0} write path rejects null", path).Which;
        using (new AssertionScope())
        {
          ex.Message.Should().Contain("not-null property", "the {0} write path names the not-null property", path);
          // nothing was stored: the properties stay unloaded
          ((IManageProperties)obj).FieldExists(NotNullEditRoot.DataProperty).Should().BeFalse("the {0} write path stores nothing", path);
          ((IManageProperties)obj).FieldExists(NotNullEditRoot.TokenProperty).Should().BeFalse("the {0} write path stores nothing", path);
        }
      }
    }

    [TestMethod]
    public void BusinessBase_UntypedLoadProperty_NullForValueType_StoresDefault()
    {
      var obj = New<NotNullEditRoot>();
      obj.LoadUntyped(NotNullEditRoot.CountProperty, 5);

      obj.LoadUntyped(NotNullEditRoot.CountProperty, null);

      obj.Count.Should().Be(0);
    }

    [TestMethod]
    public void BusinessBase_UntypedSetProperty_ConvertedValue_IsStored()
    {
      var obj = New<NotNullEditRoot>();

      obj.SetUntyped(NotNullEditRoot.NameProperty, 42);

      obj.Name.Should().Be("42");
    }

    private sealed class NullToString
    {
      public override string ToString() => null!;
    }

    [TestMethod]
    public void BusinessBase_UntypedSetProperty_DeniedNullWrite_ThrowsSecurityException()
    {
      var obj = New<NotNullEditRoot>();

      FluentActions.Invoking(() => obj.SetUntyped(NotNullEditRoot.SecretProperty, null)).Should().ThrowExactly<Csla.Security.SecurityException>();
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_NullString_StoresEmptyString()
    {
      foreach (var write in new Action<NotNullInfo>[] { o => o.LoadName(null), o => o.LoadUntyped(NotNullInfo.NameProperty, null), o => o.LoadNameConvert(null) })
      {
        var obj = New<NotNullInfo>();
        write(obj);
        obj.Name.Should().Be(string.Empty);
      }
    }

    [TestMethod]
    public void ReadOnlyBase_LoadPropertyConvert_NullStringOnUnloadedProperty_StoresEmptyString()
    {
      // old and new values are both null, so nothing changes, but the field must not stay null
      var obj = New<NotNullInfo>();

      obj.LoadNameConvert(null);

      using (new AssertionScope())
      {
        obj.Name.Should().Be(string.Empty);
        Csla.Core.NotNullInvariant.GetUnsatisfiedProperties(obj).Should().NotContain(p => p.Name == nameof(NotNullInfo.Name));
      }
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullInfo>();

      using (new AssertionScope())
      {
        FluentActions.Invoking(() => obj.LoadData(null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => obj.LoadUntyped(NotNullInfo.DataProperty, null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => obj.LoadDataConvert(null)).Should().Throw<ArgumentNullException>();
        ((IManageProperties)obj).FieldExists(NotNullInfo.DataProperty).Should().BeFalse();
      }
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_Value_Stores()
    {
      var obj = New<NotNullInfo>();

      obj.LoadName("a");

      obj.Name.Should().Be("a");
    }

    [TestMethod]
    public void ManagedObjectBase_LoadProperty_NullString_StoresEmptyString()
    {
      foreach (var write in new Action<NotNullCommand>[]
      {
        o => o.LoadResult(null),
        o => o.LoadUntyped(NotNullCommand.ResultProperty, null),
        o => o.LoadUntypedMarkDirty(NotNullCommand.ResultProperty, null),
        o => o.LoadResultConvert(null),
        o => ((IManageProperties)o).SetProperty(NotNullCommand.ResultProperty, null),
        o => ((IManageProperties)o).SetProperty(NotNullCommand.ResultProperty, new NullToString()),
      })
      {
        var obj = New<NotNullCommand>();
        write(obj);
        obj.Result.Should().Be(string.Empty);
      }
    }

    [TestMethod]
    public void ManagedObjectBase_LoadProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      using (new AssertionScope())
      {
        FluentActions.Invoking(() => obj.LoadPayload(null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => obj.LoadUntyped(NotNullCommand.PayloadProperty, null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => obj.LoadUntypedMarkDirty(NotNullCommand.PayloadProperty, null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => obj.LoadPayloadConvert(null)).Should().Throw<ArgumentNullException>();
        ((IManageProperties)obj).FieldExists(NotNullCommand.PayloadProperty).Should().BeFalse();
      }
    }

    [TestMethod]
    public void CommandBase_ManagePropertiesSetProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      using (new AssertionScope())
      {
        FluentActions.Invoking(() => ((IManageProperties)obj).SetProperty(NotNullCommand.PayloadProperty, null)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => ((IManageProperties)obj).SetProperty(NotNullCommand.TokenProperty, "x")).Should().ThrowExactly<ArgumentNullException>();
        ((IManageProperties)obj).FieldExists(NotNullCommand.TokenProperty).Should().BeFalse();
      }
    }

    [TestMethod]
    public void RegularProperty_LoadProperty_Null_IsStillAllowed()
    {
      var obj = New<NotNullEditRoot>();

      obj.LoadNullable(null);

      obj.Nullable.Should().BeNull();
    }
  }

  [Serializable]
  public class NotNullEditRoot : BusinessBase<NotNullEditRoot>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, "unknown");
    public string Code
    {
      get => GetProperty(CodeProperty);
      set => SetProperty(CodeProperty, value);
    }

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    public static readonly NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count));
    public int Count => GetProperty(CountProperty);

    public static readonly NotNullPropertyInfo<string> SecretProperty = RegisterNotNullProperty<string>(nameof(Secret));
    public string Secret => GetProperty(SecretProperty);

    public static readonly PropertyInfo<string?> NullableProperty = RegisterProperty<string?>(nameof(Nullable));
    public string? Nullable => GetProperty(NullableProperty);

    public static readonly NotNullPropertyInfo<NullToken> TokenProperty = RegisterNotNullProperty<NullToken>(nameof(Token));
    public NullToken Token => GetProperty(TokenProperty);

    public bool LoadDataMarkDirty(byte[]? value) => LoadPropertyMarkDirty(DataProperty, value!);
    public void LoadDataConvert(object? value) => LoadPropertyConvert<byte[], object>(DataProperty, value);

    public string ReadCode() => ReadProperty(CodeProperty);
    public void LoadSecret(string value) => LoadProperty(SecretProperty, value);
    public void LoadName(string? value) => LoadProperty(NameProperty, value!);
    public bool LoadNameMarkDirty(string? value) => LoadPropertyMarkDirty(NameProperty, value!);
    public void LoadNameConvert(object? value) => LoadPropertyConvert<string, object>(NameProperty, value);
    public void SetNameConvert(string? value) => SetPropertyConvert<string, string>(NameProperty, value);
    public void SetDataConvert(object? value) => SetPropertyConvert<byte[], object>(DataProperty, value);
    public void LoadUntyped(IPropertyInfo property, object? value) => LoadProperty(property, value);
    public bool LoadUntypedMarkDirty(IPropertyInfo property, object? value) => LoadPropertyMarkDirty(property, value);
    public void SetUntyped(IPropertyInfo property, object? value) => SetProperty(property, value);
    public void LoadNullable(string? value) => LoadProperty(NullableProperty, value);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.ReadProperty, SecretProperty, "Admin"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.WriteProperty, SecretProperty, "Admin"));
    }
  }

  [Serializable]
  public class NotNullInfo : ReadOnlyBase<NotNullInfo>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    public static readonly NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, "unknown");
    public string Code => ReadProperty(CodeProperty);

    public void LoadName(string? value) => LoadProperty(NameProperty, value!);
    public void LoadNameConvert(object? value) => LoadPropertyConvert<string, object>(NameProperty, value);
    public void LoadUntyped(IPropertyInfo property, object? value) => LoadProperty(property, value);

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data => GetProperty(DataProperty);

    public void LoadData(byte[]? value) => LoadProperty(DataProperty, value!);
    public void LoadDataConvert(object? value) => LoadPropertyConvert<byte[], object>(DataProperty, value);
  }

  [Serializable]
  public class NotNullCommand : CommandBase<NotNullCommand>
  {
    public static readonly NotNullPropertyInfo<string> ResultProperty = RegisterNotNullProperty<string>(nameof(Result));
    public string Result => ReadProperty(ResultProperty);

    public void LoadResult(string? value) => LoadProperty(ResultProperty, value!);
    public void LoadResultConvert(object? value) => LoadPropertyConvert<string, object>(ResultProperty, value);
    public void LoadUntyped(IPropertyInfo property, object? value) => LoadProperty(property, value);
    public bool LoadUntypedMarkDirty(IPropertyInfo property, object? value) => LoadPropertyMarkDirty(property, value);

    public static readonly NotNullPropertyInfo<byte[]> PayloadProperty = RegisterNotNullProperty<byte[]>(nameof(Payload));
    public byte[] Payload => ReadProperty(PayloadProperty);

    public static readonly NotNullPropertyInfo<NullToken> TokenProperty = RegisterNotNullProperty<NullToken>(nameof(Token));
    public NullToken Token => ReadProperty(TokenProperty);

    public void LoadPayload(byte[]? value) => LoadProperty(PayloadProperty, value!);
    public void LoadPayloadConvert(object? value) => LoadPropertyConvert<byte[], object>(PayloadProperty, value);
  }

  /// <summary>
  /// A type whose TypeConverter converts any string to null, so an untyped
  /// write of a non-null string converts to null.
  /// </summary>
  [System.ComponentModel.TypeConverter(typeof(NullTokenConverter))]
  public sealed class NullToken
  {
  }

  public sealed class NullTokenConverter : System.ComponentModel.TypeConverter
  {
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
      => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
      => value is string ? null : base.ConvertFrom(context, culture, value);
  }
}
