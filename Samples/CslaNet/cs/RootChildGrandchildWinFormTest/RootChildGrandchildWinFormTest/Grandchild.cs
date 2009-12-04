using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Grandchild : BusinessBase<Grandchild>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(Grandchild), new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(Grandchild), new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    protected override object GetIdValue()
    {
      return ReadProperty<int>(IdProperty);
    }

    private static int _lastId;

    public Grandchild()
    {
      LoadProperty<int>(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
      MarkAsChild();
    }

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("        {0} {1}: {2} {3}\r", this.GetType().Name, this.GetIdValue().ToString(), this.EditLevel, this.BindingEdit);
    }
  }
}
