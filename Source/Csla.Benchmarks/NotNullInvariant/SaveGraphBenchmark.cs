using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Benchmarks.NotNullInvariant;

/// <summary>
/// Saves a root with 10,000 new (dirty) children through the local data
/// portal. Measures the cost the not-null property check (#5047) adds to
/// the data portal and child data portal for types that do not use
/// not-null properties. Run against packages of main and of the branch;
/// see Readme.md.
/// </summary>
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class SaveGraphBenchmark
{
  private ServiceProvider _serviceProvider = default!;
  private IDataPortal<GraphRoot> _rootPortal = default!;
  private IChildDataPortal<GraphChild> _childPortal = default!;
  private GraphRoot _root = default!;

  [Params(10_000)]
  public int ChildCount { get; set; }

  [GlobalSetup]
  public void GlobalSetup()
  {
    _serviceProvider = new ServiceCollection()
      .AddCsla(o => o.AddConsoleApp().DataPortal(dpo => dpo.AddServerSideDataPortal().AddClientSideDataPortal(co => co.UseLocalProxy())))
      .BuildServiceProvider();
    _rootPortal = _serviceProvider.GetRequiredService<IDataPortal<GraphRoot>>();
    _childPortal = _serviceProvider.GetRequiredService<IChildDataPortal<GraphChild>>();
  }

  [GlobalCleanup]
  public async Task GlobalCleanup()
  {
    await _serviceProvider.DisposeAsync();
  }

  [IterationSetup]
  public void BuildGraph()
  {
    _root = _rootPortal.Create();
    for (var i = 0; i < ChildCount; i++)
      _root.Children.Add(_childPortal.CreateChild(i));
  }

  [Benchmark]
  public async Task<GraphRoot> SaveDirtyGraph()
  {
    return await _root.SaveAsync();
  }
}

[Serializable]
public class GraphRoot : BusinessBase<GraphRoot>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty)!;
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<GraphChildList> ChildrenProperty = RegisterProperty<GraphChildList>(nameof(Children));
  public GraphChildList Children => GetProperty(ChildrenProperty)!;

  [Create]
  private void Create([Inject] IChildDataPortal<GraphChildList> childListPortal)
  {
    LoadProperty(NameProperty, "root");
    LoadProperty(ChildrenProperty, childListPortal.CreateChild());
  }

  [Insert]
  private async Task Insert()
  {
    await FieldManager.UpdateChildrenAsync();
  }
}

[Serializable]
public class GraphChildList : BusinessListBase<GraphChildList, GraphChild>
{
  [CreateChild]
  private void Create()
  {
  }
}

[Serializable]
public class GraphChild : BusinessBase<GraphChild>
{
  public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
  public int Id
  {
    get => GetProperty(IdProperty);
    set => SetProperty(IdProperty, value);
  }

  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty)!;
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code));
  public string Code
  {
    get => GetProperty(CodeProperty)!;
    set => SetProperty(CodeProperty, value);
  }

  [CreateChild]
  private void Create(int id)
  {
    LoadProperty(IdProperty, id);
    LoadProperty(NameProperty, $"child {id}");
    LoadProperty(CodeProperty, "c");
  }

  [InsertChild]
  private void Insert()
  {
  }
}
