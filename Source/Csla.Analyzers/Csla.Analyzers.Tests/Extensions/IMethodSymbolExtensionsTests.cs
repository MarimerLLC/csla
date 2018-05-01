using Csla.Core;
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
    private static string AMethodName = "AMethod";

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
        this._propertyInfoManagementPath, "Something", typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.SetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForSetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.SetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForGetPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.GetPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyGetPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyGetPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLazyReadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LazyReadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyAsync()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyAsync, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForReadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.ReadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadProperty()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadProperty, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadProperty, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadProperty, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
    }

    [TestMethod]
    public async Task IsPropertyInfoManagementMethodForLoadPropertyConvert()
    {
      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(BusinessBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ReadOnlyBase<>))).IsPropertyInfoManagementMethod());

      Assert.IsTrue((await this.GetMethodReferenceSymbolAsync(
        this._propertyInfoManagementPath, CslaMemberConstants.Properties.LoadPropertyConvert, typeof(ManagedObjectBase))).IsPropertyInfoManagementMethod());
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
        this._dpOperationPath, IMethodSymbolExtensionsTests.AMethodName)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, IMethodSymbolExtensionsTests.AMethodName)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForMethodThatIsNotADataPortalOperation()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, IMethodSymbolExtensionsTests.AMethodName)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalCreate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalCreate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalCreate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalCreate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalFetch)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalFetch)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalFetch()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalFetch)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalInsert)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalInsert)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalInsert()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalInsert)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalUpdate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalUpdate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalUpdate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalUpdate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDelete)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDelete()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDelete)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDelete()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDelete)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalDeleteSelf()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalDeleteSelf)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalExecute)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForDataPortalExecute()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalExecute)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForDataPortalExecute()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.DataPortalExecute)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildCreate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildCreate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildCreate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildCreate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildCreate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildFetch)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildFetch()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildFetch)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildFetch()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildFetch)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildInsert)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildInsert()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildInsert)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildInsert()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildInsert)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildUpdate)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildUpdate()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildUpdate)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildUpdate()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildUpdate)).IsChildDataPortalOperation());
    }

    [TestMethod]
    public async Task IsDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildDeleteSelf)).IsDataPortalOperation());
    }

    [TestMethod]
    public async Task IsRootDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsFalse((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildDeleteSelf)).IsRootDataPortalOperation());
    }

    [TestMethod]
    public async Task IsChildDataPortalOperationForChildDeleteSelf()
    {
      Assert.IsTrue((await this.GetMethodSymbolAsync(
        this._dpOperationPath, CslaMemberConstants.Operations.ChildDeleteSelf)).IsChildDataPortalOperation());
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

    private async Task<IMethodSymbol> GetMethodReferenceSymbolAsync(string file, string name, Type type)
    {
      var results = await IMethodSymbolExtensionsTests.ParseFileAsync(
        file, MetadataReference.CreateFromFile(type.Assembly.Location));

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
