using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class IMethodSymbolExtensionsTests
  {
    private string _dpOperationPath;
    private string _propertyInfoManagementPath;

    [TestInitialize]
    public void TestInitialize()
    {
      this._dpOperationPath = $@"Targets\{nameof(IMethodSymbolExtensionsTests)}\IsDataPortalOperation.cs";
      this._propertyInfoManagementPath = $@"Targets\{nameof(IMethodSymbolExtensionsTests)}\IsPropertyInfoManagementMethod.cs";
    }

    [TestMethod]
    public void IsPropertyInfoManagementMethodWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForMethodThatIsNotAPropertyInfoManagementMethod()
    {
      Assert.IsFalse((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, "Something")).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyAsync)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.SetProperty)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.SetPropertyConvert)).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public void IsDataPortalOperationWhenSymbolIsNull()
    {
      Assert.IsFalse((null as IMethodSymbol).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "AMethod")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Create")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Fetch")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Insert")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Update")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Delete")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_DeleteSelf")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "DataPortal_Execute")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "Child_Create")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "Child_Fetch")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "Child_Insert")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "Child_Update")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, "Child_DeleteSelf")).IsDataPortalOperation());
    }

    private static async Task<Tuple<SemanticModel, SyntaxNode>> ParseFileAsync(
      string file, params PortableExecutableReference[] references)
    {
      var code = File.ReadAllText(file);
      var tree = CSharpSyntaxTree.ParseText(code);
      var refs = new List<PortableExecutableReference> { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
      refs.AddRange(references);

      var compilation = CSharpCompilation.Create(
        Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: refs);

      var model = compilation.GetSemanticModel(tree);
      return new Tuple<SemanticModel, SyntaxNode>(compilation.GetSemanticModel(tree),
        await tree.GetRootAsync().ConfigureAwait(false));
    }

    private async Task<IMethodSymbol> GetMethodSymbolAsync(string file, string name)
    {
      var results = await IMethodSymbolExtensionsTests.ParseFileAsync(file);

      foreach (var method in results.Item2.DescendantNodes().OfType<MethodDeclarationSyntax>())
      {
        var methodNode = results.Item1.GetDeclaredSymbol(method) as IMethodSymbol;

        if (methodNode != null && methodNode.Name == name)
        {
          return methodNode;
        }
      }

      return null;
    }

    private async Task<IMethodSymbol> GetMethodReferenceSymbolAsync(string file, string name)
    {
      var results = await IMethodSymbolExtensionsTests.ParseFileAsync(
        file, MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location));

      foreach (var invocation in results.Item2.DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        var methodNode = results.Item1.GetSymbolInfo(invocation);

        if (methodNode.Symbol != null && methodNode.Symbol.Name == name)
        {
          return methodNode.Symbol as IMethodSymbol;
        }
      }

      return null;
    }
  }
}