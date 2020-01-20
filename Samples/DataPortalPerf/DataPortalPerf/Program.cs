using BenchmarkDotNet.Running;

namespace DataPortalPerf
{
  class Program
  {
    static void Main() => BenchmarkRunner.Run<RetrieveEntities>();
  }
}
