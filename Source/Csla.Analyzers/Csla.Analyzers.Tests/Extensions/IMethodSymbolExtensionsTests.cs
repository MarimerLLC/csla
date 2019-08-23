using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class IMethodSymbolExtensionsTests
  {
    private const string AMethod = nameof(AMethod);
    private const string DP_Create = nameof(DP_Create);
    private const string DP_Fetch = nameof(DP_Fetch);
    private const string DP_Insert = nameof(DP_Insert);
    private const string DP_Update = nameof(DP_Update);
    private const string DP_Delete = nameof(DP_Delete);
    private const string DP_DeleteSelf = nameof(DP_DeleteSelf);
    private const string DP_Execute = nameof(DP_Execute);
    private const string C_Create = nameof(C_Create);
    private const string C_Fetch = nameof(C_Fetch);
    private const string C_Insert = nameof(C_Insert);
    private const string C_Update = nameof(C_Update);
    private const string C_DeleteSelf = nameof(C_DeleteSelf);
    private const string C_Execute = nameof(C_Execute);

    private static readonly string DataPortalOperationCode =
$@"using Csla;

namespace Csla.Analyzers.Tests.Targets.IMethodSymbolExtensionsTests
{{
  public class DataPortalOperations
  {{
    public void {AMethod} () {{ }}
    
    public void {CslaMemberConstants.Operations.DataPortalCreate}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalFetch}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalInsert}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalUpdate}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalDelete}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalDeleteSelf}() {{ }}
    public void {CslaMemberConstants.Operations.DataPortalExecute}() {{ }}
    public void {CslaMemberConstants.Operations.ChildCreate}() {{ }}
    public void {CslaMemberConstants.Operations.ChildFetch}() {{ }}
    public void {CslaMemberConstants.Operations.ChildInsert}() {{ }}
    public void {CslaMemberConstants.Operations.ChildUpdate}() {{ }}
    public void {CslaMemberConstants.Operations.ChildDeleteSelf}() {{ }}
    public void {CslaMemberConstants.Operations.ChildExecute}() {{ }}

    [Create] public void {DP_Create}() {{ }}
    [Fetch] public void {DP_Fetch}() {{ }}
    [Insert] public void {DP_Insert}() {{ }}
    [Update] public void {DP_Update}() {{ }}
    [Delete] public void {DP_Delete}() {{ }}
    [DeleteSelf] public void {DP_DeleteSelf}() {{ }}
    [Execute] public void {DP_Execute}() {{ }}
    [CreateChild] public void {C_Create}() {{ }}
    [FetchChild] public void {C_Fetch}() {{ }}
    [InsertChild] public void {C_Insert}() {{ }}
    [UpdateChild] public void {C_Update}() {{ }}
    [DeleteSelfChild] public void {C_DeleteSelf}() {{ }}
    [ExecuteChild] public void {C_Execute}() {{ }}
  }}
}}";
    private const string PropertyInfoManagementCode =
@"namespace Csla.Analyzers.Tests.Targets.IMethodSymbolExtensionsTests
{
  public class PropertyInfoManagementMethods
    : BusinessBase<PropertyInfoManagementMethods>
  {
    public void AMethod()
    {
      this.GetProperty(null);
      this.GetPropertyConvert<string, string>(null, null);
      this.SetProperty(null, null);
      this.SetPropertyConvert<string, string>(null, null);
      this.LoadProperty(null, null);
      this.LoadPropertyAsync<string>(null, null);
      this.LoadPropertyConvert<string, string>(null, null);
      this.LoadPropertyMarkDirty(null, null);
      this.ReadProperty(null);
      this.ReadPropertyConvert<string, string>(null);
      this.LazyGetProperty<string>(null, null);
      this.LazyGetPropertyAsync<string>(null, null);
      this.LazyReadProperty<string>(null, null);
      this.LazyReadPropertyAsync<string>(null, null);
      this.Something();
    }

    private void Something() { }
  }
}";

    [TestMethod]
    public void IsPropertyInfoManagementMethodWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForMethodThatIsNotAPropertyInfoManagementMethod()
    {
      Assert.IsFalse((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, "Something")).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.SetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.SetPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyMarkDirty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert)).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public void IsDataPortalOperationWhenSymbolIsNull() => 
      Assert.IsFalse((null as IMethodSymbol).IsDataPortalOperation());

    [TestMethod]
    public void IsRootDataPortalOperationWhenSymbolIsNull() => 
      Assert.IsFalse((null as IMethodSymbol).IsRootDataPortalOperation());

    [TestMethod]
    public void IsChildDataPortalOperationWhenSymbolIsNull() => 
      Assert.IsFalse((null as IMethodSymbol).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForMethodThatIsNotADataPortalOperation() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, AMethod)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForMethodThatIsNotADataPortalOperation() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, AMethod)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForMethodThatIsNotADataPortalOperation() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, AMethod)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalCreate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalCreate() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Create)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalCreateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Create)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalCreateWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Create)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetch() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalFetch() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalFetch() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetchWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Fetch)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalFetchWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Fetch)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalFetchWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Fetch)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsert() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalInsert() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalInsert() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsertWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Insert)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalInsertWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Insert)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalInsertWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Insert)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalUpdate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalUpdate() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Update)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalUpdateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Update)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalUpdateWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Update)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDelete() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDelete() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDelete() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Delete)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDeleteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Delete)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDeleteWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Delete)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelf() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDeleteSelf() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDeleteSelf() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelfWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_DeleteSelf)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDeleteSelfWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_DeleteSelf)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDeleteSelfWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_DeleteSelf)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecute() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalExecute() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalExecute() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecuteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Execute)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalExecuteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Execute)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalExecuteWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, DP_Execute)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildCreate() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildCreate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Create)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildCreateWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Create)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildCreateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Create)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetch() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildFetch() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildFetch() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetchWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Fetch)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildFetchWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Fetch)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildFetchWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Fetch)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsert() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildInsert() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildInsert() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsertWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Insert)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildInsertWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Insert)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildInsertWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Insert)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildUpdate() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildUpdate() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Update)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildUpdateWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Update)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildUpdateWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Update)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildDeleteSelf() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildDeleteSelf() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildDeleteSelf() => 
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildExecute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildExecute)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildExecute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildExecute)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildExecute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, CslaMemberConstants.Operations.ChildExecute)).IsChildDataPortalOperation());

    [TestMethod]
    public async Task IsDataPortalOperationForChildExecuteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Execute)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildExecuteWithAttribute() =>
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Execute)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildExecuteWithAttribute() =>
      Assert.IsTrue((await GetMethodSymbolAsync(
        DataPortalOperationCode, C_Execute)).IsChildDataPortalOperation());

    private static async Task<(SemanticModel, SyntaxNode)> ParseFileAsync(string code)
    {
      var tree = CSharpSyntaxTree.ParseText(code);
      var compilation = CSharpCompilation.Create(
        Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: AssemblyReferences.GetMetadataReferences(new[]
        {
          typeof(object).Assembly,
          typeof(BusinessBase<>).Assembly,
          typeof(Attribute).Assembly
        }));

      return (compilation.GetSemanticModel(tree), await tree.GetRootAsync().ConfigureAwait(false));
    }

    private async Task<IMethodSymbol> GetMethodSymbolAsync(string code, string name)
    {
      var (model, root) = await ParseFileAsync(code);

      foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
      {
        if (model.GetDeclaredSymbol(method) is IMethodSymbol methodNode && methodNode.Name == name)
        {
          return methodNode;
        }
      }

      return null;
    }

    private async Task<IMethodSymbol> GetMethodReferenceSymbolAsync(string code, string name)
    {
      var (model, root) = await ParseFileAsync(code);

      foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        var symbol = model.GetSymbolInfo(invocation);
        var methodSymbol = symbol.Symbol as IMethodSymbol;

        if (methodSymbol?.Name == name)
        {
          return methodSymbol;
        }
      }

      return null;
    }
  }
}