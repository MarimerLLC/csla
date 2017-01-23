using System.ComponentModel;
using System.Windows;

namespace Rolodex.Silverlight.Core
{
  public static class DesignModeHelper
  {
    public static bool IsInDesignMode
    {
      get { return DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow); }
    }
  }
}