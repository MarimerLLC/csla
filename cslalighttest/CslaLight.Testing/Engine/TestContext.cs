using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace cslalighttest.Engine
{
  public class TestContext : ObservableObject
  {
    private ObservableCollection<TypeTester> _testers = new ObservableCollection<TypeTester>();
    private int _total;
    private int _succeeded;

    public int Total
    {
      get { return _total; }
      set
      {
        _total = value;
        OnPropertyChanged("Total");
      }
    }
    public int Succeeded
    {
      get { return _succeeded; }
      set
      {
        _succeeded = value;
        OnPropertyChanged("Succeeded");
      }
    }

    public ObservableCollection<TypeTester> Testers
    {
      get { return _testers; }
    }
  }
}
