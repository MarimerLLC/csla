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

namespace SilverlightSOA
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private void CslaDataProvider_DataChanged(object sender, EventArgs e)
    {
      var dp = (Csla.Xaml.CslaDataProvider)sender;
      if (dp.Error != null)
        MessageBox.Show(dp.Error.ToString());
    }
  }
}
