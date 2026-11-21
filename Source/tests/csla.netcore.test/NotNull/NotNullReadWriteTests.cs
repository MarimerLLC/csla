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

    [TestMethod]
    public void BusinessBase_LoadProperty_NullString_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadName(null));
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullEditRoot.NameProperty));
    }

    [TestMethod]
    public void BusinessBase_LoadPropertyMarkDirty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadNameMarkDirty(null));
    }

    [TestMethod]
    public void BusinessBase_LoadPropertyConvert_NullAfterConversion_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      var ex = Assert.Throws<ArgumentNullException>(() => obj.LoadNameConvert(null));
      Assert.AreEqual(nameof(NotNullEditRoot.Name), ex.ParamName);
    }

    [TestMethod]
    public void BusinessBase_SetPropertyConvert_NullAfterConversion_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.Throws<ArgumentNullException>(() => obj.SetDataConvert(null));
    }

    [TestMethod]
    public void BusinessBase_SetPropertyConvert_NullString_StoresEmptyString()
    {
      var obj = New<NotNullEditRoot>();

      obj.SetNameConvert(null);

      Assert.AreEqual(string.Empty, obj.Name);
    }

    // Untyped writes

    [TestMethod]
    public void BusinessBase_UntypedLoadProperty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntyped(NotNullEditRoot.NameProperty, null));
    }

    [TestMethod]
    public void BusinessBase_UntypedLoadPropertyMarkDirty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntypedMarkDirty(NotNullEditRoot.NameProperty, null));
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
    public void BusinessBase_UntypedSetProperty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.Throws<ArgumentNullException>(() => obj.SetUntyped(NotNullEditRoot.NameProperty, null));
    }

    [TestMethod]
    public void BusinessBase_UntypedSetProperty_ValueConvertingToNull_ThrowsArgumentNullException()
    {
      var obj = New<NotNullEditRoot>();

      Assert.Throws<ArgumentNullException>(() => obj.SetUntyped(NotNullEditRoot.NameProperty, new NullToString()));
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullEditRoot.NameProperty));
    }

    [TestMethod]
    public void CommandBase_ManagePropertiesSetProperty_ValueConvertingToNull_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      Assert.ThrowsExactly<ArgumentNullException>(() => ((IManageProperties)obj).SetProperty(NotNullCommand.ResultProperty, new NullToString()));
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullCommand.ResultProperty));
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
    public void ReadOnlyBase_LoadProperty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullInfo>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadName(null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntyped(NotNullInfo.NameProperty, null));
      Assert.Throws<ArgumentNullException>(() => obj.LoadNameConvert(null));
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_Value_Stores()
    {
      var obj = New<NotNullInfo>();

      obj.LoadName("a");

      Assert.AreEqual("a", obj.Name);
    }

    [TestMethod]
    public void ManagedObjectBase_LoadProperty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadResult(null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntyped(NotNullCommand.ResultProperty, null));
      Assert.ThrowsExactly<ArgumentNullException>(() => obj.LoadUntypedMarkDirty(NotNullCommand.ResultProperty, null));
      Assert.Throws<ArgumentNullException>(() => obj.LoadResultConvert(null));
    }

    [TestMethod]
    public void CommandBase_ManagePropertiesSetProperty_Null_ThrowsArgumentNullException()
    {
      var obj = New<NotNullCommand>();

      Assert.ThrowsExactly<ArgumentNullException>(() => ((IManageProperties)obj).SetProperty(NotNullCommand.ResultProperty, null));
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
  }
}
