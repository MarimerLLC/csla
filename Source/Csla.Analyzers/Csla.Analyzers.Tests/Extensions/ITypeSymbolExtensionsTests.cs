using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class ITypeSymbolExtensionsTests
  {
    [TestMethod]
    public async Task IsPrimitiveForNonPrimitiveType()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsFalse(model.Compilation.GetTypeByMetadataName("System.Guid").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForBool()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Boolean").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForChar()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Char").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForString()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.String").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForByte()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Byte").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForSByte()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.SByte").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForInt16()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Int16").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForUInt16()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.UInt16").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForInt32()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Int32").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForUInt32()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.UInt32").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForInt64()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Int64").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForUInt64()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.UInt64").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForSingle()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Single").IsPrimitive());
    }

    [TestMethod]
    public async Task IsPrimitiveForDouble()
    {
      var (_, model) = await this.GetRootAndModel(string.Empty, string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName("System.Double").IsPrimitive());
    }

    [TestMethod]
    public void IsBusinessBaseWhenSymbolIsNull()
    {
      Assert.IsFalse((null as ITypeSymbol).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo { }
}";
      Assert.IsFalse((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo))).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo()
    {
      var code =
@"using System;
using Csla.Core;
using Csla.Core.FieldManager;

namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo
    : IPropertyInfo
  {
    public object DefaultValue
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string FriendlyName
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int Index
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public string Name
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public RelationshipTypes RelationshipType
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Type Type
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IFieldData NewFieldData(string name)
    {
      throw new NotImplementedException();
    }
  }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo))).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsNotABusinessBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsBusinessBaseWhenSymbolIsNotABusinessBase { }
}";
      Assert.IsFalse((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsBusinessBaseWhenSymbolIsNotABusinessBase))).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsABusinessBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsBusinessBaseWhenSymbolIsABusinessBase
    : BusinessBase<IsBusinessBaseWhenSymbolIsABusinessBase> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsBusinessBaseWhenSymbolIsABusinessBase))).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsEditableStereotypeWhenSymbolIsABusinessBase
    : BusinessBase<IsEditableStereotypeWhenSymbolIsABusinessBase> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessListBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsEditableStereotypeWhenSymbolIsABusinessListBaseBO
    : BusinessBase<IsEditableStereotypeWhenSymbolIsABusinessListBaseBO>
  { }

  public class IsEditableStereotypeWhenSymbolIsABusinessListBase
    : BusinessListBase<IsEditableStereotypeWhenSymbolIsABusinessListBase, IsEditableStereotypeWhenSymbolIsABusinessListBaseBO> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsADynamicListBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsEditableStereotypeWhenSymbolIsADynamicListBaseBO
    : BusinessBase<IsEditableStereotypeWhenSymbolIsADynamicListBaseBO>
  { }

  public class IsEditableStereotypeWhenSymbolIsADynamicListBase
    : DynamicListBase<IsEditableStereotypeWhenSymbolIsADynamicListBaseBO> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code, 
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsADynamicListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBindingListBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsEditableStereotypeWhenSymbolIsABusinessBindingListBaseBO
    : BusinessBase<IsEditableStereotypeWhenSymbolIsABusinessBindingListBaseBO>
  { }

  public class IsEditableStereotypeWhenSymbolIsABusinessBindingListBase
    : BusinessBindingListBase<IsEditableStereotypeWhenSymbolIsABusinessBindingListBase, IsEditableStereotypeWhenSymbolIsABusinessBindingListBaseBO> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsABusinessBindingListBase))).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsACommandBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsEditableStereotypeWhenSymbolIsACommandBase
    : CommandBase<IsEditableStereotypeWhenSymbolIsACommandBase> { }
}";
      Assert.IsFalse((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsEditableStereotypeWhenSymbolIsACommandBase))).IsEditableStereotype());
    }

    [TestMethod]
    public void IsStereotypeWhenSymbolIsNull()
    {
      Assert.IsFalse((null as ITypeSymbol).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsNotAStereotype()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsStereotypeWhenSymbolIsNotAStereotype { }
}";
      Assert.IsFalse((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsNotAStereotype))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject()
    {
      var code =
@"using Csla.Core;

namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject
    : IBusinessObject
  {
    public int Identity => default(int);
  }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaBusinessBase()
    {
      var code =
@"using Csla;

namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsStereotypeWhenSymbolIsStereotypeViaBusinessBase
    : BusinessBase<IsStereotypeWhenSymbolIsStereotypeViaBusinessBase> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaBusinessBase))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsDynamicListBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsStereotypeWhenSymbolIsDynamicListBaseBO
    : BusinessBase<IsStereotypeWhenSymbolIsDynamicListBaseBO>
  { }

  public class IsStereotypeWhenSymbolIsDynamicListBase
    : DynamicListBase<IsStereotypeWhenSymbolIsDynamicListBaseBO> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsDynamicListBase))).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaCommandBase()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsStereotypeWhenSymbolIsStereotypeViaCommandBase
    : CommandBase<IsStereotypeWhenSymbolIsStereotypeViaCommandBase> { }
}";
      Assert.IsTrue((await this.GetTypeSymbolAsync(code,
        nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaCommandBase))).IsStereotype());
    }

    private async Task<ITypeSymbol> GetTypeSymbolAsync(string code, string name)
    {
      var (root, model) = await this.GetRootAndModel(code, name);

      foreach (var typeNode in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
      {
        if (typeNode.Identifier.ValueText == name)
        {
          return model.GetDeclaredSymbol(typeNode);
        }
      }

      return null;
    }

    private async Task<ITypeSymbol> GetEnumSymbolAsync(string code, string name)
    {
      var (root, model) = await this.GetRootAndModel(code, name);

      foreach (var enumNode in root.DescendantNodes().OfType<EnumDeclarationSyntax>())
      {
        if (enumNode.Identifier.ValueText == name)
        {
          return model.GetDeclaredSymbol(enumNode);
        }
      }

      return null;
    }

    private async Task<ITypeSymbol> GetDelegateSymbolAsync(string code, string name)
    {
      var (root, model) = await this.GetRootAndModel(code, name);

      foreach (var delegateNode in root.DescendantNodes().OfType<DelegateDeclarationSyntax>())
      {
        if (delegateNode.Identifier.ValueText == name)
        {
          return model.GetDeclaredSymbol(delegateNode);
        }
      }

      return null;
    }

    private async Task<(SyntaxNode, SemanticModel)> GetRootAndModel(string code, string name)
    {
      var tree = CSharpSyntaxTree.ParseText(code);

      var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: new[]
        {
          MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
          MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location)
        });

      var model = compilation.GetSemanticModel(tree);
      var root = await tree.GetRootAsync().ConfigureAwait(false);

      return (root, model);
    }
  }
}