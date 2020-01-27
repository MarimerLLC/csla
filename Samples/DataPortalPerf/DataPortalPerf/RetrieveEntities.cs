using BenchmarkDotNet.Attributes;
using Csla;

namespace DataPortalPerf
{
  [MemoryDiagnoser]
  public class RetrieveEntities
  {
    [Benchmark()]
    public PersonEdit GetAll() => DataPortal.Fetch<PersonEdit>();
  }
}