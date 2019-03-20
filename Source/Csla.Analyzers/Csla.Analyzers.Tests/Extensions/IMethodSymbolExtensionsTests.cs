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
    private static string AMethodName = "AMethod";

    private readonly string _dpOperationCode =
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
    private readonly string _propertyInfoManagementCode =
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
      Assert.IsFalse((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, "Something", typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.SetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.SetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.GetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.GetPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyMarkDirty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyMarkDirty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementCode, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public void IsDataPortalOperationWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsDataPortalOperation());
    }

    [TestMethod]
    public void IsRootDataPortalOperationWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsRootDataPortalOperation());
    }

    [TestMethod]
    public void IsChildDataPortalOperationWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, IMethodSymbolExtensionsTests.AMethodName)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, IMethodSymbolExtensionsTests.AMethodName)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, IMethodSymbolExtensionsTests.AMethodName)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalCreate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalCreate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalFetch()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalFetch)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalInsert()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalInsert)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalUpdate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDelete()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDelete)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalExecute()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.DataPortalExecute)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildCreate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildCreate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildFetch()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildFetch)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildInsert()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildInsert)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildUpdate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildUpdate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationCode, CslaMemberConstants.Operations.ChildDeleteSelf)).IsChildDataPortalOperation());
    }

    private static async Task<(SemanticModel, SyntaxNode)> ParseFileAsync(
      string code, params PortableExecutableReference[] references)
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
      var (model, root) = await IMethodSymbolExtensionsTests.ParseFileAsync(code);

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
      var (model, root) = await IMethodSymbolExtensionsTests.ParseFileAsync(
        code, MetadataReference.CreateFromFile(type.Assembly.Location));

      foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        var symbol = model.GetSymbolInfo(invocation);
        var methodSymbol = symbol.Symbol as IMethodSymbol;

        if (methodSymbol == null && symbol.CandidateReason == CandidateReason.OverloadResolutionFailure &&
          symbol.CandidateSymbols.Length > 0)
        {
          methodSymbol = symbol.CandidateSymbols[0] as IMethodSymbol;
        }

        if (methodSymbol?.Name == name)
        {
          return methodSymbol;
        }
      }

      return null;
    }
  }
}
