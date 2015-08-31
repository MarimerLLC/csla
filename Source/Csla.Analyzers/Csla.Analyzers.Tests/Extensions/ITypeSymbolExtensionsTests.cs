using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class ITypeSymbolExtensionsTests
  {
    [TestMethod]
    public void IsSerializableWhenSymbolIsNull()
    {
      Assert.IsFalse((null as ITypeSymbol).IsSerializable());
    }

    [TestMethod]
    public async Task IsSerializableWhenSymbolIsNotSerializable()
    {
      Assert.IsFalse((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsSerializableWhenSymbolIsNotSerializable))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsNotSerializable))).IsSerializable());
    }

    [TestMethod]
    public async Task IsSerializableWhenSymbolIsSerializable()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsSerializableWhenSymbolIsSerializable))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsSerializable))).IsSerializable());
    }

    [TestMethod]
    public void IsStereotypeWhenSymbolIsNull()
    {
      Assert.IsFalse((null as ITypeSymbol).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsNotAStereotype()
    {
      Assert.IsFalse((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsStereotypeWhenSymbolIsNotAStereotype))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsNotAStereotype))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaBusinessBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsStereotypeWhenSymbolIsStereotypeViaBusinessBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaBusinessBase))).IsStereotype());
    }

    private async Task<ITypeSymbol> GetTypeSymbolAsync(string file, string name)
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
      var root = await tree.GetRootAsync().ConfigureAwait(false);
      return model.GetDeclaredSymbol(ITypeSymbolExtensionsTests.FindClassDeclaration(root, name));
    }

    private static ClassDeclarationSyntax FindClassDeclaration(SyntaxNode node, string name)
    {
      if (node.Kind() == SyntaxKind.ClassDeclaration)
      {
        var classNode = node as ClassDeclarationSyntax;

        if (classNode.Identifier.ValueText == name)
        {
          return classNode;
        }
      }

      foreach (var childNode in node.ChildNodes())
      {
        var childClassNode = ITypeSymbolExtensionsTests.FindClassDeclaration(childNode, name);

        if (childClassNode != null)
        {
          return childClassNode;
        }
      }

      return null;
    }
  }
}
