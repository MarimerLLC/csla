using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Silverlight;

namespace NavigationApp
{
  public partial class ControlOne : UserControl, ISupportNavigation
  {
    public ControlOne()
    {
      InitializeComponent();
    }

    public const string Bookmark = "ControlOne";


    #region ISupportsNavigation Members

    public void SetParameters(string parameters)
    {
      this.ParametersBlock.Text = parameters;
    }

    public string Title
    {
      get { return "Control One"; }
    }

    public bool CreateBookmarkAfterLoadCompleted
    {
      get { return false; }
    }

    public event EventHandler LoadCompleted;




    #endregion
  }
}
