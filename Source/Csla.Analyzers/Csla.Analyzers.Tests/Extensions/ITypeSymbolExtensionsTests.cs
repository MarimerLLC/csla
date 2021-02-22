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
    public async Task IsSpecialTypeSerializableForNonPrimitiveType()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsFalse(model.Compilation.GetTypeByMetadataName(typeof(Guid).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForBool()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(bool).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForChar()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(char).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForString()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(string).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForByte()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(byte).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForSByte()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(sbyte).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForInt16()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(short).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForUInt16()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(ushort).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForInt32()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(int).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForUInt32()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(uint).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForInt64()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(long).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForUInt64()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(ulong).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForSingle()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(float).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForDouble()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(double).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForDecimal()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(decimal).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public async Task IsSpecialTypeSerializableForDateTime()
    {
      var (_, model) = await GetRootAndModel(string.Empty);
      Assert.IsTrue(model.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName).IsSpecialTypeSerializable());
    }

    [TestMethod]
    public void IsBusinessBaseWhenSymbolIsNull() => Assert.IsFalse((null as ITypeSymbol).IsBusinessBase());

    [TestMethod]
    public async Task IsBusinessRuleForTypeThatIsNotABusinessRule()
    {
      var code = "public class A { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsBusinessRule());
    }

    [TestMethod]
    public async Task IsSerializableByMobileFormatterForSupportedType()
    {
      var code = "using System; public class A { public void Foo(Guid a) { } }";
      var (type, compilation) = await GetParameterType(code);

      Assert.IsTrue(type.IsSerializableByMobileFormatter(compilation));
    }

    [TestMethod]
    public async Task IsSerializableByMobileFormatterForNonSupportedType()
    {
      var code = "using System; using System.Collections.Generic; public class A { public void Foo(Dictionary<int, string> a) { } }";
      var (type, compilation) = await GetParameterType(code);

      Assert.IsFalse(type.IsSerializableByMobileFormatter(compilation));
    }

    [TestMethod]
    public async Task IsBusinessRuleForTypeThatIsABusinessRule()
    {
      var code = 
@"using Csla.Rules;

public class A : BusinessRule { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsBusinessRule());
    }

    [TestMethod]
    public async Task IsBusinessRuleForTypeThatIsABusinessRuleAsync()
    {
      var code =
@"using Csla.Rules;

public class A : BusinessRuleAsync { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsBusinessRule());
    }

    [TestMethod]
    public async Task IsObjectFactoryForNotObjectFactoryType()
    {
      var code = "public class A { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsObjectFactory());
    }

    [TestMethod]
    public async Task IsObjectFactoryForObjectFactoryType()
    {
      var code = 
@"using Csla.Server;

public class A : ObjectFactory { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsObjectFactory());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDoesNotDeriveFromIPropertyInfo()
    {
      var code = "public class A { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo()
    {
      var code =
@"using System;
using Csla.Core;
using Csla.Core.FieldManager;

public class A
  : IPropertyInfo
{
  public object DefaultValue
  {
    get => throw new NotImplementedException();
  }

  public string FriendlyName
  {
    get => throw new NotImplementedException();
  }

  public int Index
  {
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }

  public string Name
  {
    get => throw new NotImplementedException();
  }

  public RelationshipTypes RelationshipType
  {
    get => throw new NotImplementedException();
  }

  public Type Type
  {
    get => throw new NotImplementedException();
  }

  public IFieldData NewFieldData(string name) =>
    throw new NotImplementedException();
}";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsIPropertyInfo());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsNotABusinessBase()
    {
      var code = 
@"using Csla;

public class A { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsBusinessBaseWhenSymbolIsABusinessBase()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsBusinessBase());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBase()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessListBase()
    {
      var code =
@"using Csla;

public class ABO : BusinessBase<ABO> { }

public class A : BusinessListBase<A, ABO> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsADynamicListBase()
    {
      var code =
@"using Csla;

public class ABO : BusinessBase<ABO> { }

public class A : DynamicListBase<ABO> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsABusinessBindingListBase()
    {
      var code =
@"using Csla;

public class ABO : BusinessBase<ABO> { }

public class A : BusinessBindingListBase<A, ABO> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsEditableStereotype());
    }

    [TestMethod]
    public async Task IsEditableStereotypeWhenSymbolIsACommandBase()
    {
      var code =
@"using Csla;

public class A : CommandBase<A> { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsEditableStereotype());
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
@"public class A { }";
      Assert.IsFalse((await GetTypeSymbolAsync(code, "A")).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject()
    {
      var code =
@"using Csla.Core;

public class A
  : IBusinessObject
{
  public int Identity => default(int);
}";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaBusinessBase()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsDynamicListBase()
    {
      var code =
@"using Csla;

public class ABO : BusinessBase<ABO> { }

public class A : DynamicListBase<ABO> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsStereotype());
    }

    [TestMethod]
    public async Task IsStereotypeWhenSymbolIsStereotypeViaCommandBase()
    {
      var code =
@"using Csla;

public class A : CommandBase<A> { }";
      Assert.IsTrue((await GetTypeSymbolAsync(code, "A")).IsStereotype());
    }

    [TestMethod]
    public async Task IsRunLocalAttribute()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A> 
{ 
  [RunLocal]
  private void B() { }
}";
      Assert.IsTrue((await GetAttributeTypeSymbolAsync(code, "B")).IsRunLocalAttribute());
    }

    [TestMethod]
    public async Task IsArgumentInjectableWithAttribute()
    {
      var code =
@"using Csla;

public class A
{ 
  private void B([Inject] int b) { }
}";
      Assert.IsTrue((await GetArgumentAttributeTypeSymbolAsync(code, "B")).IsInjectable());
    }

    [TestMethod]
    public async Task IsArgumentInjectableWithoutAttribute()
    {
      var code =
@"using System;

public class CAttribute : Attribute { }

public class A
{ 
  private void B([C] int b) { }
}";
      Assert.IsFalse((await GetArgumentAttributeTypeSymbolAsync(code, "B")).IsInjectable());
    }

    private async Task<ITypeSymbol> GetArgumentAttributeTypeSymbolAsync(string code, string methodName)
    {
      var (root, model) = await GetRootAndModel(code);
      var methodSymbol = model.GetDeclaredSymbol(
        root.DescendantNodes().OfType<MethodDeclarationSyntax>().First(_ => _.Identifier.Text == methodName));
      return methodSymbol.Parameters[0].GetAttributes().First().AttributeClass;
    }

    private async Task<ITypeSymbol> GetAttributeTypeSymbolAsync(string code, string methodName)
    {
      var (root, model) = await GetRootAndModel(code);
      var methodSymbol = model.GetDeclaredSymbol(
        root.DescendantNodes().OfType<MethodDeclarationSyntax>().First(_ => _.Identifier.Text == methodName));
      return methodSymbol.GetAttributes().First().AttributeClass;
    }

    private async Task<ITypeSymbol> GetTypeSymbolAsync(string code, string name)
    {
      var (root, model) = await GetRootAndModel(code);
      return model.GetDeclaredSymbol(
        root.DescendantNodes().OfType<TypeDeclarationSyntax>().First(_ => _.Identifier.Text == name));
    }

    private async Task<(ITypeSymbol, Compilation)> GetParameterType(string code)
    {
      var tree = CSharpSyntaxTree.ParseText(code);

      var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: AssemblyReferences.GetMetadataReferences(new[]
        {
          typeof(object).Assembly,
          typeof(BusinessBase<>).Assembly
        }));

      var root = await tree.GetRootAsync().ConfigureAwait(false);
      var model = compilation.GetSemanticModel(tree);
      var methodSymbol = model.GetDeclaredSymbol(
        root.DescendantNodes().OfType<MethodDeclarationSyntax>().First(_ => _.Identifier.Text == "Foo"));
      return (methodSymbol.Parameters[0].Type, compilation);
    }

    private async Task<(SyntaxNode, SemanticModel)> GetRootAndModel(string code)
    {
      var tree = CSharpSyntaxTree.ParseText(code);

      var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
        syntaxTrees: new[] { tree },
        references: AssemblyReferences.GetMetadataReferences(new[]
        {
          typeof(object).Assembly,
          typeof(BusinessBase<>).Assembly
        }));

      var model = compilation.GetSemanticModel(tree);
      var root = await tree.GetRootAsync().ConfigureAwait(false);

      return (root, model);
    }
  }
}