using System;
using System.Linq;
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
using System.Reflection;

namespace cslalighttest.Engine
{
  public class TestContext : ObservableObject
  {
    private ObservableCollection<TypeTester> _testers = new ObservableCollection<TypeTester>();
    private int _total;
    private int _succeeded;
    private bool _isRunning;

    public bool IsRunning
    {
      get { return _isRunning; }
      private set
      {
        _isRunning = value;
        OnPropertyChanged("IsRunning");
        OnPropertyChanged("IsNotRunning");
      }
    }
    public bool IsNotRunning
    {
      get { return !_isRunning; }
    }

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
      get
      {
        var count = (from t in Testers
                     select 
                     (
                       from m in t.Methods
                       where m.Status == TestResult.Success
                       select m 
                     )
                     .Count())
                     .Sum();

        return count;
      }
    }

    public TestContext()
    {
      Assembly a = this.GetType().Assembly;
      foreach (Type t in a.GetTypes())
      {
        if (t.IsPublic && t.IsDefined(typeof(TestClassAttribute), true))
        {
          TypeTester tester = new TypeTester(t);
          Total += tester.Methods.Count;
          tester.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(tester_PropertyChanged);
          Testers.Add(tester);
        }
      }
    }

    void tester_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsRunning")
      {
        IsRunning = Testers.Any(t => t.IsRunning);
        OnPropertyChanged("Succeeded");
      }
    }

    public ObservableCollection<TypeTester> Testers
    {
      get { return _testers; }
    }

    public void Run()
    {
      IsRunning = true;
      foreach (TypeTester tester in Testers)
        tester.RunTests();
    }
  }
}
