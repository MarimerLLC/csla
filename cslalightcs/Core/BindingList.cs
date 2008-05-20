using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Csla.Core
{
  public class BindingList<T> : Csla.Silverlight.MobileList<T>
  {
    protected bool AllowEdit { get; set; }
    protected bool AllowNew { get; set; }
    protected bool AllowRemove { get; set; }
    protected bool RaiseListChangedEvents { get; set; }
  }
}
