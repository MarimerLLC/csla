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

      Assert.IsNull(obj.Name);
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullEditRoot.NameProperty));
    }

    [TestMethod]
    public void BusinessBase_ReadProperty_UnloadedDefault_ReturnsAndStoresDefault()
    {
      var obj = New<NotNullEditRoot>();

      Assert.AreEqual("unknown", obj.ReadCode());
      Assert.IsTrue(((IManageProperties)obj).FieldExists(NotNullEditRoot.CodeProperty));
    }

    [TestMethod]
    public void BusinessBase_GetProperty_DeniedRead_ReturnsDefault()
    {
      var obj = New<NotNullEditRoot>();
      obj.LoadSecret("hidden");

      Assert.IsNull(obj.Secret, "Authorization takes precedence over nullability");
    }

    [TestMethod]
    public void ReadOnlyBase_ReadProperty_UnloadedNoDefault_ReturnsNullWithoutStoringIt()
    {
      var obj = New<NotNullInfo>();

      Assert.IsNull(obj.Name);
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullInfo.NameProperty));
      Assert.AreEqual("unknown", obj.Code);
    }

    [TestMethod]
    public void ManagedObjectBase_ReadProperty_UnloadedNoDefault_ReturnsNull()
    {
      var obj = New<NotNullCommand>();

      Assert.IsNull(obj.Result);
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullCommand.ResultProperty));
    }

    // Typed writes

    [TestMethod]
    public void BusinessBase_SetProperty_NullString_StoresEmptyString()
    {
      var obj = New<NotNullEditRoot>();

      obj.Name = null!;

      Assert.AreEqual(string.Empty, obj.Name);
    }

    [TestMethod]
    public void BusinessBase_SetProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      var ex = Assert.ThrowsExactly<ArgumentNullException>(() => obj.Data = null!);
      Assert.AreEqual(nameof(NotNullEditRoot.Data), ex.ParamName);
    }

    [TestMethod]
    public void BusinessBase_SetProperty_Value_Stores()
    {
      var obj = New<NotNullEditRoot>();

      obj.Name = "a";
      obj.Data = [1];

      Assert.AreEqual("a", obj.Name);
      CollectionAssert.AreEqual(new byte[] { 1 }, obj.Data);
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

        Assert.AreEqual(string.Empty, obj.Name, path);
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

        var ex = Assert.Throws<ArgumentNullException>(() => write(obj), path);
        StringAssert.Contains(ex.Message, "not-null property", path);
        // nothing was stored: both properties still fail the invariant (the *Convert
        // methods materialise the null default before converting, as for any property)
        var unsatisfied = Csla.Core.NotNullInvariant.GetUnsatisfiedProperties(obj).Select(p => p.Name).ToList();
        CollectionAssert.IsSubsetOf(new[] { nameof(NotNullEditRoot.Data), nameof(NotNullEditRoot.Token) }, unsatisfied, path);
      }
    }

    [TestMethod]
    public void BusinessBase_UntypedLoadProperty_NullForValueType_StoresDefault()
    {
      var obj = New<NotNullEditRoot>();
      obj.LoadUntyped(NotNullEditRoot.CountProperty, 5);

      obj.LoadUntyped(NotNullEditRoot.CountProperty, null);

      Assert.AreEqual(0, obj.Count);
    }

    [TestMethod]
    public void BusinessBase_UntypedSetProperty_ConvertedValue_IsStored()
    {
      var obj = New<NotNullEditRoot>();

      obj.SetUntyped(NotNullEditRoot.NameProperty, 42);

      Assert.AreEqual("42", obj.Name);
    }

    private sealed class NullToString
    {
      public override string ToString() => null!;
    }

    [TestMethod]
    public void BusinessBase_UntypedSetProperty_DeniedNullWrite_ThrowsSecurityException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.ThrowsExactly<Csla.Security.SecurityException>(() => obj.SetUntyped(NotNullEditRoot.SecretProperty, null));
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_NullString_StoresEmptyString()
    {
      foreach (var write in new Action<NotNullInfo>[] { o => o.LoadName(null), o => o.LoadUntyped(NotNullInfo.NameProperty, null), o => o.LoadNameConvert(null) })
      {
        var obj = New<NotNullInfo>();
        write(obj);
        Assert.AreEqual(string.Empty, obj.Name);
      }
    }

    [TestMethod]
    public void ReadOnlyBase_LoadPropertyConvert_NullStringOnUnloadedProperty_StoresEmptyString()
    {
      // old and new values are both null, so nothing changes, but the field must not stay null
      var obj = New<NotNullInfo>();

      obj.LoadNameConvert(null);

      Assert.AreEqual(string.Empty, obj.Name);
      Assert.AreEqual(0, Csla.Core.NotNullInvariant.GetUnsatisfiedProperties(obj).Count(p => p.Name == nameof(NotNullInfo.Name)));
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullInfo>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadData(null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntyped(NotNullInfo.DataProperty, null));
      Assert.Throws<ArgumentNullException>(() => obj.LoadDataConvert(null));
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_Value_Stores()
    {
      var obj = New<NotNullInfo>();

      obj.LoadName("a");

      Assert.AreEqual("a", obj.Name);
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
        Assert.AreEqual(string.Empty, obj.Result);
      }
    }

    [TestMethod]
    public void ManagedObjectBase_LoadProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadPayload(null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntyped(NotNullCommand.PayloadProperty, null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntypedMarkDirty(NotNullCommand.PayloadProperty, null));
      Assert.Throws<ArgumentNullException>(() => obj.LoadPayloadConvert(null));
    }

    [TestMethod]
    public void CommandBase_ManagePropertiesSetProperty_NullReference_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      Assert.ThrowsExactly<ArgumentNullException>(() => ((IManageProperties)obj).SetProperty(NotNullCommand.PayloadProperty, null));
      Assert.ThrowsExactly<ArgumentNullException>(() => ((IManageProperties)obj).SetProperty(NotNullCommand.TokenProperty, "x"));
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullCommand.TokenProperty));
    }

    [TestMethod]
    public void RegularProperty_LoadProperty_Null_IsStillAllowed()
    {
      var obj = New<NotNullEditRoot>();

      obj.LoadNullable(null);

      Assert.IsNull(obj.Nullable);
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
