using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [CslaImplementProperties]
  public partial class Grandchild : BusinessBase<Grandchild>
  {
    public partial int Id { get; set; }
    public partial string Name { get; set; }

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
