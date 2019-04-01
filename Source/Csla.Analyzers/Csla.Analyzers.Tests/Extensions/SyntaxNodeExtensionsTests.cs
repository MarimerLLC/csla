using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using static Csla.Analyzers.Extensions.SyntaxNodeExtensions;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class SyntaxNodeExtensionsTests
  {
    [TestMethod]
    public void HasUsingWhenSymbolIsNull() => Assert.IsFalse((null as SyntaxNode).HasUsing("System"));

    [TestMethod]
    public async Task HasUsingWhenNodeHasUsingStatememt()
    {
      var code =
@"using System.Collections.Generic;

public class A { }";
      Assert.IsTrue((await GetRootAsync(code)).HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task HasUsingWhenNodeDoesNotHaveUsingStatememt()
    {
      var code = "public class HasUsingWhenNodeDoesNotHaveUsingStatememt { }";
      Assert.IsFalse((await GetRootAsync(code)).HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeExists()
    {
      var code =
@"using System;

public class A
{
  public Guid NewGuid() => Guid.NewGuid();
}";
      var rootNode = await GetRootAsync(code);
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNotNull(invocationNode.FindParent<ArrowExpressionClauseSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeDoesNotExists()
    {
      var code =
@"using System;

public class A
{
  public Guid NewGuid() => Guid.NewGuid();
}";
      var rootNode = await GetRootAsync(code);
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNull(invocationNode.FindParent<AwaitExpressionSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentIsNull()
    {
      var code = "public class A { }";
      var rootNode = await GetRootAsync(code);
      Assert.IsNull(rootNode.FindParent<AwaitExpressionSyntax>());
    }

    private async Task<SyntaxNode> GetRootAsync(string code)
    {
      return await CSharpSyntaxTree.ParseText(code).GetRootAsync();
    }
  }
}