//-----------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Csla.Generator.AutoSerialization.CSharp.Tests.Helpers
{
  // Necessary to have a copy here. Otherwise verify would place the snapshots in the other project and not here.
  public static class TestHelper<T> where T : IIncrementalGenerator, new()
  {
    public static Task Verify(string source, IEnumerable<string>? additionalSources = null)
    {
      var syntaxTrees = new List<SyntaxTree>
      {
        CSharpSyntaxTree.ParseText(source)
      };

      foreach (var src in additionalSources ?? [])
      {
        syntaxTrees.Add(CSharpSyntaxTree.ParseText(src));
      }

      // Parse the provided string into a C# syntax tree
      var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
          MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location),
          MetadataReference.CreateFromFile(typeof(Csla.Serialization.AutoSerializableAttribute).Assembly.Location)
        ]);

      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithNullableContextOptions(NullableContextOptions.Enable)
        .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS8019", ReportDiagnostic.Suppress } });
      // Create a Roslyn compilation for the syntax tree.
      CSharpCompilation compilation = CSharpCompilation.Create(
          assemblyName: "Tests",
          syntaxTrees: syntaxTrees,
          references: references,
          options: compilationOptions);


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
}
