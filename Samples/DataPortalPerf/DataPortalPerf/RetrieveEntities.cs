using BenchmarkDotNet.Attributes;
using Csla;

namespace DataPortalPerf
{
  [MemoryDiagnoser]
  public class RetrieveEntities
  {
    [Benchmark()]
    public PersonEdit GetAll()
    {
      var portal = Program.ApplicationContext.GetRequiredService<IDataPortal<PersonEdit>>();
      return portal.Fetch();
    }
  }
}