using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class ITypeSymbolExtensionsTests
  {
    [TestMethod]
    public void IsBusinessBaseWhenSymbolIsNull()
    {
      Assert.IsFalse((null as ITypeSymbol).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo()
    {
      Assert.IsFalse((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo))).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo))).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsNotABusinessBase()
    {
      Assert.IsFalse((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsBusinessBaseWhenSymbolIsNotABusinessBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsBusinessBaseWhenSymbolIsNotABusinessBase))).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsABusinessBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsBusinessBaseWhenSymbolIsABusinessBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsBusinessBaseWhenSymbolIsABusinessBase))).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsEditableStereotypeWhenSymbolIsABusinessBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessListBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsEditableStereotypeWhenSymbolIsABusinessListBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsADynamicListBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsEditableStereotypeWhenSymbolIsADynamicListBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsADynamicListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBindingListBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsEditableStereotypeWhenSymbolIsABusinessBindingListBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessBindingListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsACommandBase()
    {
      Assert.IsFalse((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsEditableStereotypeWhenSymbolIsACommandBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsACommandBase))).IsEditableStereotype());
    }

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
    public async Task IsSerializableWhenSymbolHasSerializableAttribute()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsSerializableWhenSymbolHasSerializableAttribute))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolHasSerializableAttribute))).IsSerializable());
    }

    [TestMethod]
    public async Task IsSerializableWhenSymbolImplementsISerializable()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsSerializableWhenSymbolImplementsISerializable))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolImplementsISerializable))).IsSerializable());
    }

    [TestMethod]
    public async Task IsSerializableWhenSymbolIsValueType()
    {
      var x = typeof(Targets.ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsValueType);

      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsSerializableWhenSymbolIsValueType))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsValueType))).IsSerializable());
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

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsDynamicListBase()
    {
      Assert.IsTrue((await this.GetTypeSymbolAsync(
        $@"Targets\{nameof(ITypeSymbolExtensionsTests)}\{(nameof(this.IsStereotypeWhenSymbolIsDynamicListBase))}.cs",
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsDynamicListBase))).IsStereotype());
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

      foreach (var classNode in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
      {
        if (classNode.Identifier.ValueText == name)
        {
          return model.GetDeclaredSymbol(classNode);
        }
      }

      return null;
    }
  }
}
