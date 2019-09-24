using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BusinessLibrary;
using Csla;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoExample
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class EditPerson : Page
  {
    public EditPerson()
    {
      this.InitializeComponent();
    }

    public int PersonId { get; set; } = -1;

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      if (e.Parameter != null)
        PersonId = (int)e.Parameter;

      PersonEdit person;
      this.InfoText.Text = "Loading ...";
      if (PersonId > -1)
        person = await DataPortal.FetchAsync<PersonEdit>(PersonId);
      else
        person = await DataPortal.CreateAsync<PersonEdit>();
      DataContext = person;
      this.InfoText.Text = "Loaded";
    }

    private async void SavePerson(object sender, RoutedEventArgs e)
    {
      try
      {
        var person = (PersonEdit)DataContext;
        person = await person.SaveAsync();
        var rootFrame = Window.Current.Content as Frame;
        rootFrame.Navigate(typeof(MainPage));
      }
      catch (Exception ex)
      {
        OutputText.Text = ex.ToString();
      }
    }

    private void ListPeople(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      rootFrame.Navigate(typeof(MainPage));
    }
  }
}
