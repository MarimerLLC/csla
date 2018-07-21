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

namespace SilverlightApplication9
{
  public partial class MainPage : UserControl
  {
    public MainPage()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {

      // Do not load your data at design time.
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        //Load your data here and assign the result to the CollectionViewSource.
        var rootVM = new RootVM();
        System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["rootVMViewSource"];
        myCollectionViewSource.Source = new List<object> { rootVM };
      }
    }
  }
}
