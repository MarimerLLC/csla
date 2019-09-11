using System;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Child : BusinessBase<Child>
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
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<GrandchildList> GrandchildrenProperty = RegisterProperty<GrandchildList>(p => p.Grandchildren, RelationshipTypes.LazyLoad);
    public GrandchildList Grandchildren
    {
      get => LazyGetProperty(GrandchildrenProperty, () => DataPortal.CreateChild<GrandchildList>());
    }

    private static int _lastId;

    [CreateChild]
    private void Create()
    {
      LoadProperty<int>(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
    }

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("    {0} {1}: {2} {3}\r", this.GetType().Name, this.Id, this.EditLevel, this.BindingEdit);
      var gc = ReadProperty<GrandchildList>(GrandchildrenProperty);
      if (gc != null)
        gc.DumpEditLevels(sb);
      else
        sb.AppendFormat("      GrandchildList <null>\r");
    }
  }
}
