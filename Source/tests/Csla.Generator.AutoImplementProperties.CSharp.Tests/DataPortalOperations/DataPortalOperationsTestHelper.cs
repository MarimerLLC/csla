//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationsTestHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using System.Runtime.CompilerServices;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations
{
  public static class DataPortalOperationsTestHelper<T> where T : IIncrementalGenerator, new()
  {
    /// <summary>
    /// Runs the generator and verifies the output against snapshots,
    /// asserting the generator reports no diagnostics and the resulting
    /// compilation has no errors or warnings.
    /// </summary>
    public static Task Verify(string source, IEnumerable<string>? additionalSources = null,
      AnalyzerConfigOptionsProvider? optionsProvider = null, [CallerFilePath] string sourceFile = "")
    {
      var (driver, outputCompilation, diagnostics) = Run(source, additionalSources, optionsProvider);

      using (new AssertionScope())
      {
        outputCompilation.GetDiagnostics()
          .Where(d => d.Severity == DiagnosticSeverity.Error
            || (d.Severity == DiagnosticSeverity.Warning && IsGenerated(d)))
          .Should().BeEmpty();
        diagnostics.Should().BeEmpty();
      }

      return Verifier.Verify(driver, sourceFile: sourceFile).UseDirectory("Snapshots");
    }

    /// <summary>
    /// Runs the generator and the data portal analyzers, and verifies the
    /// generated output and the analyzer diagnostics against snapshots. The
    /// generator must report no diagnostics and the resulting compilation
    /// must have no errors.
    /// </summary>
    public static Task VerifyWithDiagnostics(string source, IEnumerable<string>? additionalSources = null,
      AnalyzerConfigOptionsProvider? optionsProvider = null, [CallerFilePath] string sourceFile = "")
    {
      var (driver, outputCompilation, diagnostics) = Run(source, additionalSources, optionsProvider);

      using (new AssertionScope())
      {
        outputCompilation.GetDiagnostics()
          .Where(d => d.Severity == DiagnosticSeverity.Error)
          .Should().BeEmpty();
        diagnostics.Should().BeEmpty();
      }

      var analyzerDiagnostics = GetAnalyzerDiagnosticsAsync(outputCompilation, optionsProvider).GetAwaiter().GetResult();

      return Verifier.Verify(driver, sourceFile: sourceFile)
        .AppendValue("Diagnostics", analyzerDiagnostics)
        .UseDirectory("Snapshots");
    }

    /// <summary>
    /// Runs the data portal analyzers over a compilation and returns their
    /// diagnostics in source order.
    /// </summary>
    public static async Task<List<Diagnostic>> GetAnalyzerDiagnosticsAsync(Compilation compilation, AnalyzerConfigOptionsProvider? optionsProvider = null)
    {
      System.Collections.Immutable.ImmutableArray<DiagnosticAnalyzer> analyzers =
        [new DataPortalOperationsAnalyzer(), new DataPortalExtensionsAnalyzer()];
      var options = new CompilationWithAnalyzersOptions(
        new AnalyzerOptions([], optionsProvider ?? TestAnalyzerConfigOptionsProvider.Empty),
        onAnalyzerException: null, concurrentAnalysis: false, logAnalyzerExecutionTime: false, reportSuppressedDiagnostics: false);
      var diagnostics = await compilation.WithAnalyzers(analyzers, options).GetAnalyzerDiagnosticsAsync();
      return diagnostics
        .OrderBy(d => d.Location.SourceTree?.FilePath, StringComparer.Ordinal)
        .ThenBy(d => d.Location.SourceSpan.Start)
        .ThenBy(d => d.Id, StringComparer.Ordinal)
        .ThenBy(d => d.GetMessage(System.Globalization.CultureInfo.InvariantCulture), StringComparer.Ordinal)
        .ToList();
    }

    /// <summary>
    /// Creates the compilation and generator driver used by the tests.
    /// </summary>
    public static (GeneratorDriver Driver, CSharpCompilation Compilation) Setup(string source, IEnumerable<string>? additionalSources = null, bool trackSteps = false,
      AnalyzerConfigOptionsProvider? optionsProvider = null)
    {
      var syntaxTrees = new List<SyntaxTree>
      {
        CSharpSyntaxTree.ParseText(source, path: "Source0.cs")
      };

      var index = 1;
      foreach (var additionalSource in additionalSources ?? [])
        syntaxTrees.Add(CSharpSyntaxTree.ParseText(additionalSource, path: $"Source{index++}.cs"));

      var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
          MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location)
        ]);

      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithNullableContextOptions(NullableContextOptions.Enable)
        .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS8019", ReportDiagnostic.Suppress } });

      var compilation = CSharpCompilation.Create(
        assemblyName: "Tests",
        syntaxTrees: syntaxTrees,
        references: references,
        options: compilationOptions);

      var driverOptions = new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: trackSteps);
      GeneratorDriver driver = CSharpGeneratorDriver.Create([new T().AsSourceGenerator()], optionsProvider: optionsProvider, driverOptions: driverOptions);
      return (driver, compilation);
    }

    private static bool IsGenerated(Diagnostic diagnostic)
      => diagnostic.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true;

    private static (GeneratorDriver Driver, Compilation OutputCompilation, System.Collections.Immutable.ImmutableArray<Diagnostic> Diagnostics) Run(string source, IEnumerable<string>? additionalSources,
      AnalyzerConfigOptionsProvider? optionsProvider)
    {
      var (driver, compilation) = Setup(source, additionalSources, optionsProvider: optionsProvider);
      driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
      return (driver, outputCompilation, diagnostics);
    }
  }
}
