using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Csla.Generator.Tests.Helper;

internal static class TestHelper<T> where T : IIncrementalGenerator, new()
{
  public static (GeneratorDriver Driver, CSharpCompilation Compilation) SetupSourceGenerator(string cslaSource, string additionalSource, bool enableNullableContext, TestAnalyzerConfigOptionsProvider globalCompilerOptions)
  {
    var parserOptions = new CSharpParseOptions(LanguageVersion.CSharp13);
    var syntaxTrees = new List<SyntaxTree>() {
            CSharpSyntaxTree.ParseText(cslaSource, parserOptions) // CslaContainingTypeTree
        };

    if (!string.IsNullOrWhiteSpace(additionalSource))
    {
      syntaxTrees.Add(CSharpSyntaxTree.ParseText(additionalSource, parserOptions));
    }

    var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
                MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CslaIgnorePropertyAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayAttribute).Assembly.Location),
        ]);

    var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: syntaxTrees,
            references: references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithNullableContextOptions(enableNullableContext ? NullableContextOptions.Enable : NullableContextOptions.Disable)
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS1701", ReportDiagnostic.Suppress } })
        );

    var generator = new T().AsSourceGenerator();

    var driverOptions = new GeneratorDriverOptions(disabledOutputs: IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true);
    var driver = CSharpGeneratorDriver.Create([generator], driverOptions: driverOptions, parseOptions: parserOptions).WithUpdatedAnalyzerConfigOptions(globalCompilerOptions);

    return (driver, compilation);
  }
}