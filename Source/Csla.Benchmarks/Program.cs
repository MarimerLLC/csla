using BenchmarkDotNet.Running;
using Csla.Benchmarks.CollectionIdentity;

var summary = BenchmarkRunner.Run<CollectionIdentityBenchmark>();
