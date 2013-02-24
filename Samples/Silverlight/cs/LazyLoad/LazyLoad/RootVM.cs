using System;
using Csla.Xaml;

namespace SilverlightApplication9
{
  public class RootVM : ViewModel<Root>
  {
    public RootVM()
    {
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        BeginRefresh("GetRoot");
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
    }
  }
}
