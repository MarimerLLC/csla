using BenchmarkDotNet.Running;
using Csla.Benchmarks.NotNullInvariant;

var summary = BenchmarkRunner.Run<SaveGraphBenchmark>();
