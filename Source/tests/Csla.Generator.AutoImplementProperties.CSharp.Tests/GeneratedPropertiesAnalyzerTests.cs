using System.Collections.Immutable;
using Csla.Analyzers;
using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.Tests
{
  /// <summary>
  /// Runs CSLA0007 (EvaluatePropertiesForSimplicityAnalyzer) over the code that
  /// [CslaImplementProperties] and [CslaImplementPropertiesInterface] generate,
  /// which isn't wrapped in a pragma.
  /// </summary>
  [TestClass]
  public class GeneratedPropertiesAnalyzerTests
  {
    private const string CSLA0007 = "CSLA0007";

    private const string DocumentChild = """
      namespace Test;

      public class DocumentChild : Csla.BusinessBase<DocumentChild>
      {
      }
      """;

    [TestMethod]
    [DataRow("Csla.BusinessBase<TestClass>", "")]
    [DataRow("Csla.ReadOnlyBase<TestClass>", "")]
    [DataRow("Csla.CommandBase<TestClass>", "")]
    [DataRow("Csla.BusinessDocumentBase<TestClass, DocumentChild>", DocumentChild)]
    public async Task GeneratedPropertiesHaveNoCsla0007(string baseClass, string additionalSource)
    {
      var source = $$"""
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class TestClass : {{baseClass}}
        {
          public partial string Name { get; set; }
          public partial string? Nickname { get; set; }
          public partial int Quantity { get; set; }
          public partial int? Age { get; set; }
          public partial string Code { get; private set; }
        }
        """;

      var diagnostics = await RunCsla0007OnGeneratedCode(
        new IncrementalAutoImplementPropertiesPartialsGenerator(), [source, additionalSource]);

      diagnostics.Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("Csla.BusinessBase<TestClass>", "")]
    [DataRow("Csla.ReadOnlyBase<TestClass>", "")]
    [DataRow("Csla.CommandBase<TestClass>", "")]
    [DataRow("Csla.BusinessDocumentBase<TestClass, DocumentChild>", DocumentChild)]
    public async Task GeneratedInterfacePropertiesHaveNoCsla0007(string baseClass, string additionalSource)
    {
      var source = $$"""
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<ITestClass>]
        public partial class TestClass : {{baseClass}}
        {
          private interface ITestClass
          {
            string Name { get; set; }
            string? Nickname { get; set; }
            int Quantity { get; set; }
            int? Age { get; set; }
            string Code { get; }
          }
        }
        """;

      var diagnostics = await RunCsla0007OnGeneratedCode(
        new IncrementalAutoImplementInterfacePartialsGenerator(), [source, additionalSource]);

      diagnostics.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Csla0007ReportsHandWrittenGetterInGeneratedClass()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
          public partial string Name { get; set; }
        }

        public partial class TestClass
        {
          public static readonly Csla.PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code));
          public string Code => GetProperty(CodeProperty)!.ToUpper();
        }
        """;

      var diagnostics = await RunCsla0007OnGeneratedCode(
        new IncrementalAutoImplementPropertiesPartialsGenerator(), [source]);

      diagnostics.Should().ContainSingle();
      var diagnostic = diagnostics[0];
      diagnostic.Id.Should().Be(CSLA0007);
      diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan)
        .Should().Be("GetProperty(CodeProperty)!.ToUpper()");
    }

    private static async Task<ImmutableArray<Diagnostic>> RunCsla0007OnGeneratedCode(IIncrementalGenerator generator, string[] sources)
    {
      sources = [.. sources.Where(s => s.Length > 0)];
      var compilation = TestCompilation.Create(sources);

      var driver = CSharpGeneratorDriver.Create(generator.AsSourceGenerator());
      driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

      generatorDiagnostics.Should().BeEmpty();
      outputCompilation.SyntaxTrees.Should().HaveCount(sources.Length + 1, "the generator should add one file");
      outputCompilation.SyntaxTrees.Last().GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>()
        .Should().NotBeEmpty("the generator should implement the properties");
      outputCompilation.GetDiagnostics().Should().BeEmpty();

      return await outputCompilation
        .WithAnalyzers([new EvaluatePropertiesForSimplicityAnalyzer()])
        .GetAnalyzerDiagnosticsAsync();
    }
  }
}
