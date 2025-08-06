using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Generator.Tests;

internal sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider {

    public static TestAnalyzerConfigOptionsProvider Empty { get; } = new TestAnalyzerConfigOptionsProvider(TestAnalyzerConfigOptions.Empty);

    public override AnalyzerConfigOptions GlobalOptions { get; }

    private TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions) {
        GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) 
        => TestAnalyzerConfigOptions.Empty;
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) 
        => TestAnalyzerConfigOptions.Empty;

    public static TestAnalyzerConfigOptionsProvider Create(string key, string value) 
        => Create(new[] { KeyValuePair.Create(key, value) });

    public static TestAnalyzerConfigOptionsProvider Create(IEnumerable<KeyValuePair<string,string>> config) 
        => new(new TestAnalyzerConfigOptions(ImmutableDictionary.CreateRange(config))
);
}
