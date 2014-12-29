using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bxf;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;

namespace WpUI
{
  public class MainWindowPresenter : DependencyObject
  {
    public MainWindowPresenter()
    {
      var presenter = (IPresenter)Shell.Instance;
      presenter.OnShowError += (message, title) =>
        {
          MessageBox.Show(message, title, MessageBoxButton.OK);
        };
      presenter.OnShowStatus += (status) =>
        {
        };
      presenter.OnShowView += (view, region) =>
        {
          PivotItem panel = GetPanel(int.Parse(region));
          var vm = view.Model as IViewModel;
          if (vm != null)
            panel.Header = vm.Header;
          else
            panel.Header = string.Empty;
          panel.Content = view.ViewInstance;
        };

      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        try
        {
          Shell.Instance.ShowView(
            typeof(OrderEdit).AssemblyQualifiedName,
            "orderVmViewSource",
            new OrderVm(),
            "1");
        }
        catch (Exception ex)
        {
          Shell.Instance.ShowError(ex.Message, "Startup error");
        }
      }
    }

    private PivotItem GetPanel(int id)
    {
      while (Panels.Count < id) Panels.Add(new PivotItem());
      return Panels[id - 1];
    }

    public static readonly DependencyProperty PanelsProperty =
        DependencyProperty.Register("Panels", typeof(ObservableCollection<PivotItem>), typeof(MainWindowPresenter), 
        new PropertyMetadata(new ObservableCollection<PivotItem>()));
    public ObservableCollection<PivotItem> Panels
    {
      get { return (ObservableCollection<PivotItem>)GetValue(PanelsProperty); }
      set { SetValue(PanelsProperty, value); }
    }
  }
}
