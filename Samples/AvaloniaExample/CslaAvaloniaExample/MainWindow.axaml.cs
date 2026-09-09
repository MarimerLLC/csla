using Avalonia.Controls;
using CslaAvaloniaExample.Views;

namespace CslaAvaloniaExample;

public partial class MainWindow : Window
{
  public MainWindow(PersonEditPage personEditPage, PersonListPage personListPage)
  {
    InitializeComponent();

    PersonEditHost.Content = personEditPage;
    PersonListHost.Content = personListPage;
  }
}
