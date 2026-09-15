//-----------------------------------------------------------------------
// <copyright file="TestAnalyzerConfigOptionsProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations
{
  /// <summary>
  /// Supplies global build properties, such as <c>build_property.*</c> values,
  /// to generators and analyzers under test.
  /// </summary>
  internal sealed class TestAnalyzerConfigOptionsProvider(Dictionary<string, string> globalOptions) : AnalyzerConfigOptionsProvider
  {
    public static TestAnalyzerConfigOptionsProvider Empty { get; } = new([]);

    public override AnalyzerConfigOptions GlobalOptions { get; } = new Options(globalOptions);

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;

    private sealed class Options(Dictionary<string, string> values) : AnalyzerConfigOptions
    {
      public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => values.TryGetValue(key, out value);
    }
  }
}
