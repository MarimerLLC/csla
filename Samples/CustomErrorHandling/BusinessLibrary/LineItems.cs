using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  public class LineItems : BusinessBindingListBase<LineItems, LineItem>
  {
    public LineItems()
    {
      AllowNew = true;
    }

    protected override object AddNewCore()
    {
      var dp = ApplicationContext.GetRequiredService<IChildDataPortal<LineItem>>();
      var item = dp.CreateChild();
      Add(item);
      return item;
    }

    [CreateChild]
    private void CreateChild() { }
  }
}
