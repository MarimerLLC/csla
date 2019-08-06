using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
      Assert.IsTrue((await this.GetRootAsync(
        $@"Targets\{nameof(SyntaxNodeExtensionsTests)}\{(nameof(this.HasUsingWhenNodeHasUsingStatememt))}.cs"))
          .HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task HasUsingWhenNodeDoesNotHaveUsingStatememt()
    {
      Assert.IsFalse((await this.GetRootAsync(
        $@"Targets\{nameof(SyntaxNodeExtensionsTests)}\{(nameof(this.HasUsingWhenNodeDoesNotHaveUsingStatememt))}.cs"))
          .HasUsing("System.Collections.Generic"));
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeExists()
    {
      var rootNode = await this.GetRootAsync(
        $@"Targets\{nameof(SyntaxNodeExtensionsTests)}\{(nameof(this.FindParentWhenParentTypeExists))}.cs");
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNotNull(invocationNode.FindParent<BlockSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentTypeDoesNotExists()
    {
      var rootNode = await this.GetRootAsync(
        $@"Targets\{nameof(SyntaxNodeExtensionsTests)}\{(nameof(this.FindParentWhenParentTypeDoesNotExists))}.cs");
      var invocationNode = rootNode.DescendantNodes(_ => true)
        .Where(_ => _.Kind() == SyntaxKind.InvocationExpression).First();

      Assert.IsNull(invocationNode.FindParent<AwaitExpressionSyntax>());
    }

    [TestMethod]
    public async Task FindParentWhenParentIsNull()
    {
      var rootNode = await this.GetRootAsync(
        $@"Targets\{nameof(SyntaxNodeExtensionsTests)}\{(nameof(this.FindParentWhenParentIsNull))}.cs");

      Assert.IsNull(rootNode.FindParent<AwaitExpressionSyntax>());
    }

    private async Task<SyntaxNode> GetRootAsync(string file)
    {
      var code = File.ReadAllText(file);
      return await CSharpSyntaxTree.ParseText(code).GetRootAsync();
    }
  }
}
