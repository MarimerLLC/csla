using BenchmarkDotNet.Running;
using Csla.Benchmarks.PerformanceCloner;

var summary = BenchmarkRunner.Run<PerformanceClonerBenchmark>();
