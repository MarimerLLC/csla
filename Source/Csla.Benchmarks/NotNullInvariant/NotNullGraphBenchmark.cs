using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Benchmarks.NotNullInvariant;

/// <summary>
/// Saves a root with 10,000 new (dirty) children, each holding a new (dirty)
/// detail child, through the local data portal. The graph is registered
/// either with <c>PropertyInfo</c> or with not-null properties (a string with
/// a default and a detail child without one), so the difference is what the
/// not-null check (#5047) costs objects that use not-null properties: every
/// child and detail is checked on entry to and exit from its update.
/// Compiled only for CSLA 11 and later; see Readme.md.
/// </summary>
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class NotNullGraphBenchmark
{
  private ServiceProvider _serviceProvider = default!;
  private object _root = default!;

  [Params(10_000)]
  public int ChildCount { get; set; }

  [Params("PropertyInfo", "NotNullPropertyInfo")]
  public string Registration { get; set; } = "";

  [GlobalSetup]
  public void GlobalSetup()
  {
    _serviceProvider = new ServiceCollection()
      .AddCsla(o => o.AddConsoleApp().DataPortal(dpo => dpo.AddServerSideDataPortal().AddClientSideDataPortal(co => co.UseLocalProxy())))
      .BuildServiceProvider();
  }

  [GlobalCleanup]
  public async Task GlobalCleanup()
  {
    await _serviceProvider.DisposeAsync();
  }

  [IterationSetup]
  public void BuildGraph()
  {
    if (Registration == "PropertyInfo")
    {
      var root = _serviceProvider.GetRequiredService<IDataPortal<PlainShapeRoot>>().Create();
      var childPortal = _serviceProvider.GetRequiredService<IChildDataPortal<PlainShapeChild>>();
      for (var i = 0; i < ChildCount; i++)
        root.Children.Add(childPortal.CreateChild(i));
      _root = root;
    }
    else
    {
      var root = _serviceProvider.GetRequiredService<IDataPortal<NotNullShapeRoot>>().Create();
      var childPortal = _serviceProvider.GetRequiredService<IChildDataPortal<NotNullShapeChild>>();
      for (var i = 0; i < ChildCount; i++)
        root.Children.Add(childPortal.CreateChild(i));
      _root = root;
    }
  }

  [Benchmark]
  public async Task<object> SaveDirtyGraphWithDetails()
  {
    return _root switch
    {
      PlainShapeRoot plain => await plain.SaveAsync(),
      NotNullShapeRoot notNull => await notNull.SaveAsync(),
      _ => throw new InvalidOperationException()
    };
  }
}

[Serializable]
public partial class ShapeDetail : BusinessBase<ShapeDetail>
{
  public static readonly PropertyInfo<int> ValueProperty = RegisterProperty<int>(nameof(Value));
  public int Value => GetProperty(ValueProperty);

  [CreateChild]
  private void Create(int value)
  {
    LoadProperty(ValueProperty, value);
  }

  [InsertChild]
  private void Insert()
  {
  }
}

// The same graph twice: registered with PropertyInfo, and with not-null properties.

[Serializable]
public partial class PlainShapeRoot : BusinessBase<PlainShapeRoot>
{
  public static readonly PropertyInfo<PlainShapeList> ChildrenProperty = RegisterProperty<PlainShapeList>(nameof(Children));
  public PlainShapeList Children => GetProperty(ChildrenProperty)!;

  [Create]
  private void Create([Inject] IChildDataPortal<PlainShapeList> listPortal)
  {
    LoadProperty(ChildrenProperty, listPortal.CreateChild());
  }

  [Insert]
  private async Task Insert()
  {
    await FieldManager.UpdateChildrenAsync();
  }
}

[Serializable]
public partial class PlainShapeList : BusinessListBase<PlainShapeList, PlainShapeChild>
{
}

[Serializable]
public partial class PlainShapeChild : BusinessBase<PlainShapeChild>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name => GetProperty(NameProperty)!;

  public static readonly PropertyInfo<ShapeDetail> DetailProperty = RegisterProperty<ShapeDetail>(nameof(Detail));
  public ShapeDetail Detail => GetProperty(DetailProperty)!;

  [CreateChild]
  private void Create(int id, [Inject] IChildDataPortal<ShapeDetail> detailPortal)
  {
    LoadProperty(NameProperty, $"child {id}");
    LoadProperty(DetailProperty, detailPortal.CreateChild(id));
  }

  [InsertChild]
  private async Task Insert()
  {
    await FieldManager.UpdateChildrenAsync();
  }
}

[Serializable]
public partial class NotNullShapeRoot : BusinessBase<NotNullShapeRoot>
{
  public static readonly NotNullPropertyInfo<NotNullShapeList> ChildrenProperty = RegisterNotNullProperty<NotNullShapeList>(nameof(Children));
  public NotNullShapeList Children => GetProperty(ChildrenProperty);

  [Create]
  private void Create([Inject] IChildDataPortal<NotNullShapeList> listPortal)
  {
    LoadProperty(ChildrenProperty, listPortal.CreateChild());
  }

  [Insert]
  private async Task Insert()
  {
    await FieldManager.UpdateChildrenAsync();
  }
}

[Serializable]
public partial class NotNullShapeList : BusinessListBase<NotNullShapeList, NotNullShapeChild>
{
}

[Serializable]
public partial class NotNullShapeChild : BusinessBase<NotNullShapeChild>
{
  public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
  public string Name => GetProperty(NameProperty);

  public static readonly NotNullPropertyInfo<ShapeDetail> DetailProperty = RegisterNotNullProperty<ShapeDetail>(nameof(Detail));
  public ShapeDetail Detail => GetProperty(DetailProperty);

  [CreateChild]
  private void Create(int id, [Inject] IChildDataPortal<ShapeDetail> detailPortal)
  {
    LoadProperty(NameProperty, $"child {id}");
    LoadProperty(DetailProperty, detailPortal.CreateChild(id));
  }

  [InsertChild]
  private async Task Insert()
  {
    await FieldManager.UpdateChildrenAsync();
  }
}
