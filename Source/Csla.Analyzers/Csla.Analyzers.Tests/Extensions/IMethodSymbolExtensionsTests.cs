using Csla.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class IMethodSymbolExtensionsTests
  {
    private const string AMethodName = "AMethod";

    private const string DataPortalOperationCode =
@"namespace Csla.Analyzers.Tests.Targets.IMethodSymbolExtensionsTests
{
  public class DataPortalOperations
  {
    public void AMethod() { }
    public void DataPortal_Create() { }
    public void DataPortal_Fetch() { }
    public void DataPortal_Insert() { }
    public void DataPortal_Update() { }
    public void DataPortal_Delete() { }
    public void DataPortal_DeleteSelf() { }
    public void DataPortal_Execute() { }
    public void Child_Create() { }
    public void Child_Fetch() { }
    public void Child_Insert() { }
    public void Child_Update() { }
    public void Child_DeleteSelf() { }
  }
}";
    private const string PropertyInfoManagementCode =
@"namespace Csla.Analyzers.Tests.Targets.IMethodSymbolExtensionsTests
{
  public class PropertyInfoManagementMethods
    : BusinessBase<PropertyInfoManagementMethods>
  {
    public void AMethod()
    {
      this.GetProperty((string)null);
      this.GetPropertyConvert<string, string>(null, (string)null);
      this.SetProperty(null, null);
      this.SetPropertyConvert<string, string>(null, null);
      this.LoadProperty(null, null);
      this.LoadPropertyAsync<string>(null, null);
      this.LoadPropertyConvert<string, string>(null, null);
      this.LoadPropertyMarkDirty(null, null);
      this.ReadProperty(null);
      this.ReadPropertyConvert<string, string>(null);
      this.LazyGetProperty<string>(null, (string)null);
      this.LazyGetPropertyAsync<string>(null, (string)null);
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
        PropertyInfoManagementCode, "Something", typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.SetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.SetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyAsync()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyMarkDirty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadProperty()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyConvert()
    {
      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await GetMethodReferenceSymbolAsync(
        PropertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
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
        DataPortalOperationCode, AMethodName)).IsDataPortalOperation());

    [TestMethod]
    public async Task IsRootDataPortalOperationForMethodThatIsNotADataPortalOperation() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, AMethodName)).IsRootDataPortalOperation());

    [TestMethod]
    public async Task IsChildDataPortalOperationForMethodThatIsNotADataPortalOperation() => 
      Assert.IsFalse((await GetMethodSymbolAsync(
        DataPortalOperationCode, AMethodName)).IsChildDataPortalOperation());

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

    private static async Task<(SemanticModel, SyntaxNode)> ParseFileAsync(string code, params PortableExecutableReference[] references)
    {
      var tree = CSharpSyntaxTree.ParseText(code);
      var refs = new List<PortableExecutableReference> { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
      refs.AddRange(references);

      var compilation = CSharpCompilation.Create(
        Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: refs);

      var model = compilation.GetSemanticModel(tree);
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

    private async Task<IMethodSymbol> GetMethodReferenceSymbolAsync(string code, string name, Type type)
    {
      var (model, root) = await ParseFileAsync(
        code, MetadataReference.CreateFromFile(type.Assembly.Location));

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