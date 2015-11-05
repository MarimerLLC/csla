using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        this._propertyInfoManagementPath, CslaMemberConstants.CslaPropertyMethods.GetProperty)).IsPropertyInfoManagementMethod());
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

    private async Task<IMethodSymbol> GetMethodSymbolAsync(string file, string name)
    {
      var code = File.ReadAllText(file);
      var tree = CSharpSyntaxTree.ParseText(code);

      var compilation = CSharpCompilation.Create(
        Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: new[]
        {
          MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        });

      var model = compilation.GetSemanticModel(tree);
      var root = await tree.GetRootAsync().ConfigureAwait(false);
      return IMethodSymbolExtensionsTests.FindMethodDeclaration(root, name, model);
    }

    private async Task<IMethodSymbol> GetMethodReferenceSymbolAsync(string file, string name)
    {
      var code = File.ReadAllText(file);
      var tree = CSharpSyntaxTree.ParseText(code);

      var compilation = CSharpCompilation.Create(
        Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: new[]
        {
          MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
          MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location)
        });

      var model = compilation.GetSemanticModel(tree);

      foreach (var invocation in (await tree.GetRootAsync()).DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        var methodNode = model.GetSymbolInfo(invocation);

        if (methodNode.Symbol != null && methodNode.Symbol.Name == name)
        {
          return methodNode.Symbol as IMethodSymbol;
        }
      }

      return null;
    }

    private static IMethodSymbol FindMethodDeclaration(SyntaxNode node, string name, SemanticModel model)
    {
      if (node.Kind() == SyntaxKind.MethodDeclaration)
      {
        var methodNode = model.GetDeclaredSymbol(node) as IMethodSymbol;

        if (methodNode.Name == name)
        {
          return methodNode;
        }
      }

      foreach (var childNode in node.ChildNodes())
      {
        var childMethodNode = IMethodSymbolExtensionsTests.FindMethodDeclaration(childNode, name, model);

        if (childMethodNode != null)
        {
          return childMethodNode;
        }
      }

      return null;
    }
  }
}