using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
				$@"Targets\{nameof(SyntaxNodeExtensionsTests)}.{(nameof(this.HasUsingWhenNodeHasUsingStatememt))}.cs"))
					.HasUsing("System.Collections.Generic"));
		}

		[TestMethod]
		public async Task HasUsingWhenNodeDoesNotHaveUsingStatememt()
		{
			Assert.IsFalse((await this.GetRootAsync(
				$@"Targets\{nameof(SyntaxNodeExtensionsTests)}.{(nameof(this.HasUsingWhenNodeDoesNotHaveUsingStatememt))}.cs"))
					.HasUsing("System.Collections.Generic"));
		}

		private async Task<SyntaxNode> GetRootAsync(string file)
		{
			var code = File.ReadAllText(file);
			return await CSharpSyntaxTree.ParseText(code).GetRootAsync();
		}
	}
}
