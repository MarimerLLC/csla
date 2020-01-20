using BenchmarkDotNet.Attributes;
using Csla;

namespace DataPortalPerf
{
  [MemoryDiagnoser]
  public class RetrieveEntities
  {
    [Benchmark(OperationsPerInvoke = 100)]
    public PersonEdit GetAll() => DataPortal.Fetch<PersonEdit>();
  }
}