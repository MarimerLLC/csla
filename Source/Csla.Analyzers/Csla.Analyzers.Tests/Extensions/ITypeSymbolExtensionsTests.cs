using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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
			Assert.IsFalse((await this.GetTypeSymbol(
				$@"Targets\{nameof(ITypeSymbolExtensionsTests)}.{(nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsNotSerializable))}.cs",
				new TextSpan(0, 34))).IsSerializable());
		}

		[TestMethod]
		public async Task IsSerializableWhenSymbolIsSerializable()
		{
			Assert.IsTrue((await this.GetTypeSymbol(
				$@"Targets\{nameof(ITypeSymbolExtensionsTests)}.{(nameof(ITypeSymbolExtensionsTests.IsSerializableWhenSymbolIsSerializable))}.cs",
				new TextSpan(17, 47))).IsSerializable());
		}

		[TestMethod]
		public void IsStereotypeWhenSymbolIsNull()
		{
			Assert.IsFalse((null as ITypeSymbol).IsStereotype());
		}

		[TestMethod]
		public async Task IsStereotypeWhenSymbolIsNotAStereotype()
		{
			Assert.IsFalse((await this.GetTypeSymbol(
				$@"Targets\{nameof(ITypeSymbolExtensionsTests)}.{(nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsNotAStereotype))}.cs",
				new TextSpan(0, 23))).IsStereotype());
		}

		[TestMethod]
		public async Task IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject()
		{
			Assert.IsTrue((await this.GetTypeSymbol(
				$@"Targets\{nameof(ITypeSymbolExtensionsTests)}.{(nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaIBusinessObject))}.cs",
				new TextSpan(20, 44))).IsStereotype());
		}

		[TestMethod]
		public async Task IsStereotypeWhenSymbolIsStereotypeViaBusinessBase()
		{
			Assert.IsTrue((await this.GetTypeSymbol(
				$@"Targets\{nameof(ITypeSymbolExtensionsTests)}.{(nameof(ITypeSymbolExtensionsTests.IsStereotypeWhenSymbolIsStereotypeViaBusinessBase))}.cs",
				new TextSpan(15, 50))).IsStereotype());
		}

		private async Task<ITypeSymbol> GetTypeSymbol(string file, TextSpan span)
		{
			var code = File.ReadAllText(file);
			var tree = CSharpSyntaxTree.ParseText(code);

			var compilation = CSharpCompilation.Create(
				Guid.NewGuid().ToString("N"),
				syntaxTrees: new[] { tree },
				references: new[]
				{
					MetadataReference.CreateFromAssembly(typeof(object).Assembly),
					MetadataReference.CreateFromAssembly(typeof(BusinessBase<>).Assembly)
				});

			var model = compilation.GetSemanticModel(tree);
			var root = await tree.GetRootAsync().ConfigureAwait(false);
			return model.GetDeclaredSymbol(root.FindNode(span) as ClassDeclarationSyntax);
		}
	}
}
