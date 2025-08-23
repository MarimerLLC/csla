### Benchmark for IdentityManager usage in csla lists
----

In the `results` folder you find two benchmark runs.
* v9 - The implementation which uses `items.Max(i => i.Identity)`
* v10 - The improved implementation which uses `item.Identity`

### How to run the Benchmark

Tell BenchmarkDotNet in the `Program.cs` to use the `CollectionIdentityBenchmark` class to run the benchmarks.

*Beware: The v9 version takes a lot of time to run (30min+)* 