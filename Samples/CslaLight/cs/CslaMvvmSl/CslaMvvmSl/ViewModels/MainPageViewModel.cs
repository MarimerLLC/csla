using System;
using System.Windows;
using System.Windows.Controls;

namespace CslaMvvmSl.ViewModels
{
  public class MainPageViewModel : DependencyObject
  {
    public void NewPerson()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(EditPerson).AssemblyQualifiedName,
        "editPersonViewModelViewSource",
        new ViewModels.EditPersonViewModel(),
        "Content");
    }

    public void ListPersons()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(ListPersons).AssemblyQualifiedName,
        "listPersonsViewModelViewSource",
        new ViewModels.ListPersonsViewModel(),
        "Content");
    }

    public MainPageViewModel()
    {
      var presenter = (Bxf.IPresenter)Bxf.Shell.Instance;
      presenter.OnShowError += (message, caption) =>
        {
          MessageBox.Show(message, caption, MessageBoxButton.OK);
        };

      presenter.OnShowStatus += (status) =>
        {
        };

      presenter.OnShowView += (view, region) =>
        {
          if (region == "Content")
            MainContent = view.ViewInstance;
        };
    }

    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register("MainContent", typeof(UserControl), typeof(MainPageViewModel), null);

    public UserControl MainContent
    {
      get { return (UserControl)GetValue(MainContentProperty); }
      set { SetValue(MainContentProperty, value); }
    }
  }
}
