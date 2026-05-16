using BenchmarkDotNet.Attributes;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;
using PropertyPerf.Client.Model;

namespace Csla.Benchmarks.PerformanceCloner;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net462)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net48)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net10_0)]
[HtmlExporter, JsonExporterAttribute.FullCompressed]
[MemoryDiagnoser(true)]
public class PerformanceClonerBenchmark
{
  private ServiceProvider _serviceProvider = default!;
  private IDataPortal<TestItem> _listPortal = default!;
  private TestItem _fetch = new();


  [GlobalSetup]
  public void GlobalSetup()
  {
    _serviceProvider = new ServiceCollection()
           .AddCsla(o => o.AddConsoleApp().DataPortal(dpo => dpo.AddServerSideDataPortal().AddClientSideDataPortal(co => co.UseLocalProxy())).Serialization(so => so.UseMobileFormatter()))
           .BuildServiceProvider();

    _listPortal = _serviceProvider.GetRequiredService<IDataPortal<TestItem>>();
    _fetch = _listPortal.FetchAsync().Result;
  }

  [GlobalCleanup]
  public async Task GlobalCleanup()
  {
    await _serviceProvider.DisposeAsync();
  }

  [Benchmark(Baseline = true)]
  public async Task FetchAndSerialize()
  {
    var clone = _fetch.Clone();

  }

  [Benchmark]
  public async Task FetchAndCloneInternal()
  {
    var formatter = _serviceProvider.GetRequiredService<ISerializationFormatter>() as MobileFormatter;
    if (formatter == null)
    {
      throw new InvalidOperationException("MobileFormatter not found in service provider");
    }
    var clone = formatter.SerializeAsDTO(_fetch);
    var clone2 = formatter.DeserializeAsDTO(clone);
  }
}
