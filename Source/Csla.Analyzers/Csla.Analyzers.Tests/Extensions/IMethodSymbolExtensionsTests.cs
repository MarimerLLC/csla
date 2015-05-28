using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.IMethodSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class IMethodSymbolExtensionsTests
  {
    private string _path;

    [TestInitialize]
    public void TestInitialize()
    {
      this._path = $@"Targets\{nameof(IMethodSymbolExtensionsTests)}.IsDataPortalOperation.cs";
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
        this._path, "AMethod")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Create")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Fetch")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Insert")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Update")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Delete")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_DeleteSelf")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "DataPortal_Execute")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "Child_Create")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "Child_Fetch")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "Child_Insert")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "Child_Update")).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._path, "Child_DeleteSelf")).IsDataPortalOperation());
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
          MetadataReference.CreateFromAssembly(typeof(object).Assembly),
        });

      var model = compilation.GetSemanticModel(tree);
      var root = await tree.GetRootAsync().ConfigureAwait(false);
      return model.GetDeclaredSymbol(IMethodSymbolExtensionsTests.FindMethodDeclaration(root, name));
    }

    private static MethodDeclarationSyntax FindMethodDeclaration(SyntaxNode node, string name)
    {
      if (node.Kind() == SyntaxKind.MethodDeclaration)
      {
        var methodNode = node as MethodDeclarationSyntax;

        if (methodNode.Identifier.ValueText == name)
        {
          return methodNode;
        }
      }

      foreach (var childNode in node.ChildNodes())
      {
        var childMethodNode = IMethodSymbolExtensionsTests.FindMethodDeclaration(childNode, name);

        if (childMethodNode != null)
        {
          return childMethodNode;
        }
      }

      return null;
    }
  }
}