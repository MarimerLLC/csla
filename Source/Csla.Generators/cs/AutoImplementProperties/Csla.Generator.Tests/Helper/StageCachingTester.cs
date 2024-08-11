using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using FluentAssertions;
using Microsoft.CodeAnalysis;

namespace Csla.Generator.Tests.Helper;

internal static class StageCachingTester<T> where T : IIncrementalGenerator, new()
{
  public static void VerfiyStageCaching(string cslaSource)
  {

    var (driver, compilation) = TestHelper<T>.SetupSourceGenerator(cslaSource, "", false, TestAnalyzerConfigOptionsProvider.Empty);

    var clonedCompilation = compilation.Clone();

    driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

    var runResult1 = driver.GetRunResult();
    var runResult2 = driver.RunGenerators(clonedCompilation).GetRunResult();
    AssertRunsEqual(runResult1, runResult2, GetAllTrackingNames());
    runResult2.Results[0]
                .TrackedOutputSteps
                .SelectMany(x => x.Value) // step executions
                .SelectMany(x => x.Outputs) // execution results
                .Should()
                .OnlyContain(x => x.Reason == IncrementalStepRunReason.Cached);
  }

  private static string[] GetAllTrackingNames()
  {
    return typeof(TrackingNames)
                    .GetFields()
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                    .Select(x => (string?)x.GetRawConstantValue())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray()!;
  }

  private static void AssertRunsEqual(GeneratorDriverRunResult runResult1, GeneratorDriverRunResult runResult2, string[] trackingNames)
  {
    // We're given all the tracking names, but not all the
    // stages will necessarily execute, so extract all the 
    // output steps, and filter to ones we know about
    var trackedSteps1 = GetTrackedSteps(runResult1, trackingNames);
    var trackedSteps2 = GetTrackedSteps(runResult2, trackingNames);

    // Both runs should have the same tracked steps
    trackedSteps1.Should()
                 .NotBeEmpty()
                 .And.HaveSameCount(trackedSteps2)
                 .And.ContainKeys(trackedSteps2.Keys);

    // Get the IncrementalGeneratorRunStep collection for each run
    foreach (var (trackingName, runSteps1) in trackedSteps1)
    {
      // Assert that both runs produced the same outputs
      var runSteps2 = trackedSteps2[trackingName];
      AssertEqual(runSteps1, runSteps2, trackingName);
    }

    // Local function that extracts the tracked steps
    static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(GeneratorDriverRunResult runResult, string[] trackingNames)
        => runResult
                .Results[0] // We're only running a single generator, so this is safe
                .TrackedSteps // Get the pipeline outputs
                .Where(step => trackingNames.Contains(step.Key)) // filter to known steps
                .ToDictionary(x => x.Key, x => x.Value); // Convert to a dictionary
  }

  private static void AssertEqual(ImmutableArray<IncrementalGeneratorRunStep> runSteps1, ImmutableArray<IncrementalGeneratorRunStep> runSteps2, string stepName)
  {
    runSteps1.Should().HaveSameCount(runSteps2);

    for (var i = 0; i < runSteps1.Length; i++)
    {
      var runStep1 = runSteps1[i];
      var runStep2 = runSteps2[i];

      // The outputs should be equal between different runs
      IEnumerable<object> outputs1 = runStep1.Outputs.Select(x => x.Value);
      IEnumerable<object> outputs2 = runStep2.Outputs.Select(x => x.Value);

      outputs1.Should()
              .Equal(outputs2, $"because {stepName} should produce cacheable outputs");

      // Therefore, on the second run the results should always be cached or unchanged!
      // - Unchanged is when the _input_ has changed, but the output hasn't
      // - Cached is when the the input has not changed, so the cached output is used 
      runStep2.Outputs.Should()
          .OnlyContain(
              x => x.Reason == IncrementalStepRunReason.Cached || x.Reason == IncrementalStepRunReason.Unchanged,
              $"{stepName} expected to have reason {IncrementalStepRunReason.Cached} or {IncrementalStepRunReason.Unchanged}");

      // Make sure we're not using anything we shouldn't
      AssertObjectGraph(runStep1, stepName);
    }
  }

  private static void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
  {
    // Including the stepName in error messages to make it easy to isolate issues
    var because = $"{stepName} shouldn't contain banned symbols";
    var visited = new HashSet<object>();

    // Check all of the outputs - probably overkill, but why not
    foreach (var (obj, _) in runStep.Outputs)
    {
      Visit(obj);
    }

    void Visit(object node)
    {
      // If we've already seen this object, or it's null, stop.
      if (node is null || !visited.Add(node))
      {
        return;
      }

      // Make sure it's not a banned type
      node.Should()
          .NotBeOfType<Compilation>(because)
          .And.NotBeOfType<ISymbol>(because)
          .And.NotBeOfType<SyntaxNode>(because);

      // Examine the object
      Type type = node.GetType();
      if (type.IsPrimitive || type.IsEnum || type == typeof(string))
      {
        return;
      }

      // If the object is a collection, check each of the values
      if (node is IEnumerable collection and not string)
      {
        foreach (object element in collection)
        {
          // recursively check each element in the collection
          Visit(element);
        }

        return;
      }

      // Recursively check each field in the object
      foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
      {
        object fieldValue = field.GetValue(node);
        Visit(fieldValue);
      }
    }
  }
}
