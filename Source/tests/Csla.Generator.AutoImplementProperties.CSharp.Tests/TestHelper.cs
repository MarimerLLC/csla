using AwesomeAssertions;
using AwesomeAssertions.Execution;
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
    /// References the assemblies loaded in the test process plus every assembly
    /// in the shared framework. The test host doesn't load every framework
    /// assembly before a test runs, so the loaded assemblies alone can miss
    /// ones such as System.ComponentModel and System.ObjectModel.
    /// </summary>
    public static IEnumerable<MetadataReference> LoadedAndFrameworkReferences()
    {
      var frameworkDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
      var framework = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
        .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
        .Where(p => string.Equals(Path.GetDirectoryName(p), frameworkDirectory, StringComparison.OrdinalIgnoreCase));
      var loaded = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => a.Location);

      return loaded.Concat(framework)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Select(p => MetadataReference.CreateFromFile(p));
    }

    /// <summary>
    /// Creates a nullable-enabled compilation of the sources that references
    /// the assemblies loaded in the test process, including Csla.
    /// </summary>
    public static CSharpCompilation Create(IEnumerable<string> sources)
    {
      var references = LoadedAndFrameworkReferences()
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