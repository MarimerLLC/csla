using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Csla.Generator.Tests
{
  public static class TestHelper<T> where T : IIncrementalGenerator, new()
  {
    public static Task Verify(string source, IEnumerable<string>? additionalSources = null)
    {
      var compilation = TestCompilation.Create([source, .. (additionalSources ?? [])]);

      // Create an instance of our EnumGenerator incremental source generator
      var generator = new T().AsSourceGenerator();

      // The GeneratorDriver is used to run our generator against a compilation
      GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

      // Run the source generator!
      driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
      var result = driver.GetRunResult();

      using (new AssertionScope())
      {
        outputCompilation.GetDiagnostics().Should().BeEmpty();
        diagnostics.Should().BeEmpty();
      }


      return Verifier.Verify(driver).UseDirectory("Snapshots");
    }
  }

  public static class TestCompilation
  {
    /// <summary>
    /// Creates a nullable-enabled compilation of the sources that references
    /// the assemblies loaded in the test process, including Csla.
    /// </summary>
    public static CSharpCompilation Create(IEnumerable<string> sources)
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

      return CSharpCompilation.Create(
          assemblyName: "Tests",
          syntaxTrees: sources.Select(s => CSharpSyntaxTree.ParseText(s)),
          references: references,
          options: compilationOptions);
    }
  }
}