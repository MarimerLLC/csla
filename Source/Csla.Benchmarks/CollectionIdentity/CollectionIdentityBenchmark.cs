using BenchmarkDotNet.Attributes;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Benchmarks.CollectionIdentity;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net462)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net48)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90, baseline: true)]
[HtmlExporter, JsonExporterAttribute.FullCompressed]
public class CollectionIdentityBenchmark
{
  private ServiceProvider _serviceProvider = default!;
  private IDataPortal<CollectionIdentityBenchmarkTestList> _listPortal = default!;

  [Params(1, 10, 100, 1_000, 10_000, 100_000)]
  public int NumberOfItems;

  [GlobalSetup]
  public void GlobalSetup()
  {
    _serviceProvider = new ServiceCollection()
           .AddCsla(o => o.AddConsoleApp().DataPortal(dpo => dpo.AddServerSideDataPortal().AddClientSideDataPortal(co => co.UseLocalProxy())).Serialization(so => so.UseMobileFormatter()))
           .BuildServiceProvider();

    _listPortal = _serviceProvider.GetRequiredService<IDataPortal<CollectionIdentityBenchmarkTestList>>();
  }

  [GlobalCleanup]
  public async Task GlobalCleanup()
  {
    await _serviceProvider.DisposeAsync();
  }

  [Benchmark]
  public Task<CollectionIdentityBenchmarkTestList> Create() => _listPortal.CreateAsync(NumberOfItems);
}

public class CollectionIdentityBenchmarkTestList : BusinessListBase<CollectionIdentityBenchmarkTestList, CollectionIdentityBenchmarkTestChild>
{
  [Create]
  private async Task Create(int numberOfItems, [Inject] IChildDataPortal<CollectionIdentityBenchmarkTestChild> cdp)
  {
    using (SuppressListChangedEvents)
    {
      for (int i = 0; i < numberOfItems; i++)
      {
        Add(await cdp.CreateChildAsync(i));
      }
    }
  }
}

[CslaImplementProperties]
public partial class CollectionIdentityBenchmarkTestChild : BusinessBase<CollectionIdentityBenchmarkTestChild>
{
  public partial int Number { get; set; }

  [CreateChild]
  private void CreateChild(int number)
  {
    Number = number;
  }
}