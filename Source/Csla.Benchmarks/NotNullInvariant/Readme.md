### Benchmark for the not-null property check (#5047)
----

`SaveGraphBenchmark` saves a root with 10,000 new (dirty) children through the
local data portal. The data portal and the child data portal check the not-null
property invariant of every object they create, fetch or update, so this measures
what that check costs a type that does not use not-null properties.

### How to run the benchmark

1. Pack `Csla` in Release from `main` and from the branch into a local folder
   (for example `dotnet pack Csla\Csla.csproj -c Release -o <feed>`; pack `main`
   from a separate clone). Each package gets its own version from Nerdbank.GitVersioning.
2. In `Program.cs`, run the `SaveGraphBenchmark` class.
3. For each package, from the `Csla.Benchmarks` folder:

```powershell
$env:RestoreAdditionalProjectSources = '<feed>'
$env:CslaVersion = '<package version>'
dotnet run -c Release --framework net10.0
```

`CslaVersion` is set as an environment variable so the projects BenchmarkDotNet
generates use the same package.

### Results

AMD Ryzen 9 5900X, .NET 10.0.15, two runs each, alternating:

| Package | Run 1 | Run 2 | Allocated |
| --- | ---: | ---: | ---: |
| `main` (2b40ef7b4) | 170.3 ms ± 5.7 | 166.6 ms ± 3.6 | 97.17 MB |
| branch (540497eb9) | 169.8 ms ± 7.6 | 169.8 ms ± 5.2 | 96.63 MB |

The mean is 0.8% higher on the branch, within the run-to-run noise, and well
under the 5% limit set for the change.

### Not-null properties: `NotNullGraphBenchmark`

`NotNullGraphBenchmark` saves a root with 10,000 new children, each holding a
new detail child, so 20,000 objects go through the child data portal. The same
graph is registered either with `PropertyInfo` or with not-null properties (a
`string` with a default and a detail child without one), so the difference is
what the check costs objects that use not-null properties. It compiles only
against CSLA 11 or later. Run it like `SaveGraphBenchmark`, with
`BenchmarkRunner.Run<NotNullGraphBenchmark>()` in `Program.cs`.

AMD Ryzen 9 5900X, .NET 10, branch package (42d674d17), one run:

| Registration | Mean | Allocated |
| --- | ---: | ---: |
| `PropertyInfo` | 358.5 ms ± 2.5 | 173.99 MB |
| not-null properties | 360.2 ms ± 7.2 | 174.07 MB |

Checking every child and detail on entry to and exit from its update costs about
0.5%, within run-to-run noise.
