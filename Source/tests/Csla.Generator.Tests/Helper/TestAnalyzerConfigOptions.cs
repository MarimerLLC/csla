using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Csla.Generator.Tests;

internal sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions {
    internal static readonly ImmutableDictionary<string, string> EmptyDictionary = ImmutableDictionary.Create<string, string>(KeyComparer);

    public static TestAnalyzerConfigOptions Empty { get; } = new TestAnalyzerConfigOptions(EmptyDictionary);

    private readonly ImmutableDictionary<string, string> _options;

    public TestAnalyzerConfigOptions(ImmutableDictionary<string, string> options) {
        _options = options;
    }

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _options.TryGetValue(key[15..], out value);

    public override IEnumerable<string> Keys => _options.Keys;
}