using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bxf;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI
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
          MainContent = view.ViewInstance;
        };

      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        try
        {
          Init();
        }
        catch (Exception ex)
        {
          Shell.Instance.ShowError(ex.Message, "Startup error");
        }
      }
    }

    private void Init()
    {
      Shell.Instance.ShowView(
        typeof(OrderEdit).AssemblyQualifiedName,
        "orderVmViewSource",
        new OrderVm(),
        "Main");
    }

    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register("MainContent", typeof(UserControl), typeof(MainWindowPresenter), null);
    public UserControl MainContent
    {
      get { return (UserControl)GetValue(MainContentProperty); }
      set { SetValue(MainContentProperty, value); }
    }
  }
}
