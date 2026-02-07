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

namespace Csla.Generator.DataPortalInterfaces.CSharp.Tests.Helpers
{
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

      var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
          MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location)
        ]);

      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithNullableContextOptions(NullableContextOptions.Enable)
        .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS8019", ReportDiagnostic.Suppress } });

      CSharpCompilation compilation = CSharpCompilation.Create(
          assemblyName: "Tests",
          syntaxTrees: syntaxTrees,
          references: references,
          options: compilationOptions);

      var generator = new T().AsSourceGenerator();
      GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
      driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

      using (new AssertionScope())
      {
        outputCompilation.GetDiagnostics()
          .Where(d => d.Severity == DiagnosticSeverity.Error)
          .Should().BeEmpty();
        diagnostics.Should().BeEmpty();
      }

      return Verifier.Verify(driver).UseDirectory("Snapshots");
    }

    public static Task VerifyWithDiagnostics(string source)
    {
      var syntaxTrees = new List<SyntaxTree>
      {
        CSharpSyntaxTree.ParseText(source)
      };

      var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Concat([
          MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location)
        ]);

      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithNullableContextOptions(NullableContextOptions.Enable)
        .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic> { { "CS8019", ReportDiagnostic.Suppress } });

      CSharpCompilation compilation = CSharpCompilation.Create(
          assemblyName: "Tests",
          syntaxTrees: syntaxTrees,
          references: references,
          options: compilationOptions);

      var generator = new T().AsSourceGenerator();
      GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
      driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

      return Verifier.Verify(driver).UseDirectory("Snapshots");
    }
  }
}
