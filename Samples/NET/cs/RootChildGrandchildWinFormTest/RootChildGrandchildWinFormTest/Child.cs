using System;
using System.Collections.Generic;
using System.Text;
using Csla;
namespace WindowsApplication2
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public static readonly PropertyInfo<GrandchildList> GrandchildrenProperty = RegisterProperty<GrandchildList>(p => p.Grandchildren, RelationshipTypes.LazyLoad);
    public GrandchildList Grandchildren
    {
      get
      {
        if (!FieldManager.FieldExists(GrandchildrenProperty))
          LoadProperty<GrandchildList>(GrandchildrenProperty, new GrandchildList());
        return GetProperty<GrandchildList>(GrandchildrenProperty);
      }
    }

    protected override object GetIdValue()
    {
      return ReadProperty<int>(IdProperty);
    }

    private static int _lastId;

    public Child()
    {
      LoadProperty<int>(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
      MarkAsChild();
    }

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("    {0} {1}: {2} {3}\r", this.GetType().Name, this.GetIdValue().ToString(), this.EditLevel, this.BindingEdit);
      var gc = ReadProperty<GrandchildList>(GrandchildrenProperty);
      if (gc != null)
        gc.DumpEditLevels(sb);
      else
        sb.AppendFormat("      GrandchildList <null>\r");
    }
  }
}
