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
    public void HasUsingWhenSymbolIsNull()
    {
      Assert.IsFalse((null as SyntaxNode).HasUsing("System"));
    }

    [TestMethod]
    public async Task HasUsingWhenNodeHasUsingStatememt()
    {
      var code =
@"using System.Collections.Generic;

namespace Csla.Analyzers.Tests.Targets.SyntaxNodeExtensionsTests
{
  public class HasUsingWhenNodeHasUsingStatememt { }
}";
      Assert.IsTrue((await this.GetRootAsync(code)).HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task HasUsingWhenNodeDoesNotHaveUsingStatememt()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.SyntaxNodeExtensionsTests
{
  public class HasUsingWhenNodeDoesNotHaveUsingStatememt { }
}";
      Assert.IsFalse((await this.GetRootAsync(code)).HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeExists()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.SyntaxNodeExtensionsTests
{
  public class FindParentWhenParentTypeExists
  {
    public Guid NewGuid()
    {
      return Guid.NewGuid();
    }
  }
}";
      var rootNode = await this.GetRootAsync(code);
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNotNull(invocationNode.FindParent<BlockSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeDoesNotExists()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.SyntaxNodeExtensionsTests
{
  public class FindParentWhenParentTypeDoesNotExists
  {
    public Guid NewGuid()
    {
      return Guid.NewGuid();
    }
  }
}";
      var rootNode = await this.GetRootAsync(code);
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNull(invocationNode.FindParent<AwaitExpressionSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentIsNull()
    {
      var code =
@"using System;

namespace Csla.Analyzers.Tests.Targets.SyntaxNodeExtensionsTests
{
  public class FindParentWhenParentIsNull { }
}";
      var rootNode = await this.GetRootAsync(code);
      Assert.IsNull(rootNode.FindParent<AwaitExpressionSyntax>());
    }

    private async Task<SyntaxNode> GetRootAsync(string code)
    {
      return await CSharpSyntaxTree.ParseText(code).GetRootAsync();
    }
  }
}