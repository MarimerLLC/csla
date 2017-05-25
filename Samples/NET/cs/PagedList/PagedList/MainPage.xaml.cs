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

namespace PagedList
{
  public partial class MainPage : UserControl
  {
    public MainPage()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {

       if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
       {
       	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["mainViewModelViewSource"];
        myCollectionViewSource.Source = new List<object> { new MainViewModel() };
       }
    }
  }
}
