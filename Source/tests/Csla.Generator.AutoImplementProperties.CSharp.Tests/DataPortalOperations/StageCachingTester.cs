//-----------------------------------------------------------------------
// <copyright file="StageCachingTester.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Verifies incremental generator pipeline stages are cacheable</summary>
//-----------------------------------------------------------------------
// Adapted from Csla.DataPortalExtensions by Stefan Ossendorf
// (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// licensed under the MIT License. Copyright (c) 2023 Stefan Ossendorf.
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations
{
  internal static class StageCachingTester
  {
    public static void VerifyStageCaching<T>(string source, Type trackingNamesType) where T : IIncrementalGenerator, new()
    {
      var (driver, compilation) = DataPortalOperationsTestHelper<T>.Setup(source, trackSteps: true);
      var clonedCompilation = compilation.Clone();

      driver = driver.RunGenerators(compilation);
      var runResult1 = driver.GetRunResult();
      var runResult2 = driver.RunGenerators(clonedCompilation).GetRunResult();

      AssertRunsEqual(runResult1, runResult2, GetTrackingNames(trackingNamesType));

      runResult2.Results[0]
        .TrackedOutputSteps
        .SelectMany(step => step.Value)
        .SelectMany(execution => execution.Outputs)
        .Should()
        .OnlyContain(output => output.Reason == IncrementalStepRunReason.Cached);
    }

    private static string[] GetTrackingNames(Type trackingNamesType)
      => trackingNamesType
        .GetFields()
        .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
        .Select(field => (string?)field.GetRawConstantValue())
        .Where(name => !string.IsNullOrEmpty(name))
        .ToArray()!;

    private static void AssertRunsEqual(GeneratorDriverRunResult runResult1, GeneratorDriverRunResult runResult2, string[] trackingNames)
    {
      var trackedSteps1 = GetTrackedSteps(runResult1, trackingNames);
      var trackedSteps2 = GetTrackedSteps(runResult2, trackingNames);

      trackedSteps1.Should()
        .NotBeEmpty()
        .And.HaveSameCount(trackedSteps2)
        .And.ContainKeys(trackedSteps2.Keys);

      foreach (var (trackingName, runSteps1) in trackedSteps1)
        AssertEqual(runSteps1, trackedSteps2[trackingName], trackingName);

      static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(GeneratorDriverRunResult runResult, string[] trackingNames)
        => runResult
          .Results[0]
          .TrackedSteps
          .Where(step => trackingNames.Contains(step.Key))
          .ToDictionary(step => step.Key, step => step.Value);
    }

    private static void AssertEqual(ImmutableArray<IncrementalGeneratorRunStep> runSteps1, ImmutableArray<IncrementalGeneratorRunStep> runSteps2, string stepName)
    {
      runSteps1.Should().HaveSameCount(runSteps2);

      for (var i = 0; i < runSteps1.Length; i++)
      {
        var runStep1 = runSteps1[i];
        var runStep2 = runSteps2[i];

        var outputs1 = runStep1.Outputs.Select(output => output.Value);
        var outputs2 = runStep2.Outputs.Select(output => output.Value);
        outputs1.Should().Equal(outputs2, $"because {stepName} should produce cacheable outputs");

        runStep2.Outputs.Should().OnlyContain(
          output => output.Reason == IncrementalStepRunReason.Cached || output.Reason == IncrementalStepRunReason.Unchanged,
          $"{stepName} expected to have reason {IncrementalStepRunReason.Cached} or {IncrementalStepRunReason.Unchanged}");

        AssertObjectGraph(runStep1, stepName);
      }
    }

    private static void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
    {
      var because = $"{stepName} shouldn't contain banned symbols";
      var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

      foreach (var (value, _) in runStep.Outputs)
        Visit(value);

      void Visit(object? node)
      {
        if (node is null || !visited.Add(node))
          return;

        node.Should().NotBeAssignableTo<Compilation>(because)
          .And.NotBeAssignableTo<ISymbol>(because)
          .And.NotBeAssignableTo<SyntaxNode>(because)
          .And.NotBeAssignableTo<Location>(because);

        var type = node.GetType();
        if (type.IsPrimitive || type.IsEnum || type == typeof(string))
          return;

        if (node is IEnumerable collection)
        {
          foreach (var element in collection)
            Visit(element);
          return;
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
          Visit(field.GetValue(node));
      }
    }
  }
}
