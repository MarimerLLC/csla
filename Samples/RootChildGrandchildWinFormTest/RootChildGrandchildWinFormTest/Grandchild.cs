using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Grandchild : BusinessBase<Grandchild>
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

    private static int _lastId;

    [CreateChild]
    private void Create()
    {
      LoadProperty<int>(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
    }

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("        {0} {1}: {2} {3}\r", this.GetType().Name, this.Id, this.EditLevel, this.BindingEdit);
    }
  }
}
