using System.Collections.Immutable;
using Csla.Analyzers;
using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.Tests
{
  /// <summary>
  /// Runs CSLA0007 (EvaluatePropertiesForSimplicityAnalyzer) over the code that
  /// [CslaImplementProperties] generates, which isn't wrapped in a pragma.
  /// </summary>
  [TestClass]
  public class GeneratedPropertiesAnalyzerTests
  {
    private const string CSLA0007 = "CSLA0007";

    private const string BusinessDocumentChild = """

      public class DocumentChild : Csla.BusinessBase<DocumentChild>
      {
      }
      """;

    [TestMethod]
    [DataRow("Csla.BusinessBase<TestClass>")]
    [DataRow("Csla.ReadOnlyBase<TestClass>")]
    [DataRow("Csla.CommandBase<TestClass>")]
    [DataRow("Csla.BusinessDocumentBase<TestClass, DocumentChild>")]
    public async Task GeneratedPropertiesHaveNoCsla0007(string baseClass)
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
        {{BusinessDocumentChild}}
        """;

      var diagnostics = await RunCsla0007OnGeneratedCode(source);

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

      var diagnostics = await RunCsla0007OnGeneratedCode(source);

      diagnostics.Should().ContainSingle();
      var diagnostic = diagnostics[0];
      diagnostic.Id.Should().Be(CSLA0007);
      diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan)
        .Should().Be("GetProperty(CodeProperty)!.ToUpper()");
    }

    private static async Task<ImmutableArray<Diagnostic>> RunCsla0007OnGeneratedCode(string source)
    {
      var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
          MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location),
          MetadataReference.CreateFromFile(typeof(CslaIgnorePropertyAttribute).Assembly.Location)
        ]);

      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithNullableContextOptions(NullableContextOptions.Enable)
        .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS8019", ReportDiagnostic.Suppress } });

      var compilation = CSharpCompilation.Create(
        assemblyName: "Tests",
        syntaxTrees: [CSharpSyntaxTree.ParseText(source)],
        references: references,
        options: compilationOptions);

      GeneratorDriver driver = CSharpGeneratorDriver.Create(new IncrementalAutoImplementPropertiesPartialsGenerator().AsSourceGenerator());
      driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

      generatorDiagnostics.Should().BeEmpty();
      outputCompilation.SyntaxTrees.Should().HaveCount(2, "the generator should add one file");
      outputCompilation.GetDiagnostics().Should().BeEmpty();

      var analyzerDiagnostics = await outputCompilation
        .WithAnalyzers([new EvaluatePropertiesForSimplicityAnalyzer()])
        .GetAnalyzerDiagnosticsAsync();

      return analyzerDiagnostics;
    }
  }
}
