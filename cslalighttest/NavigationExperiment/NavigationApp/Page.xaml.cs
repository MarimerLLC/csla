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
using System.Windows.Browser;

namespace NavigationApp
{
  public partial class Page : UserControl
  {
    private UserControl _currentControl;
    private Dictionary<string, string> _visitedControls;
    private int _nextId = 1;
    public Page()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(Page_Loaded);
    }

    void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _visitedControls = new Dictionary<string, string>();
      ((App)(App.Current)).NavHandler.Navigate += new EventHandler<StateEventArgs>(_navHandler_Navigate);
    }

    private void ControlOne_Click(object sender, RoutedEventArgs e)
    {
      LoadControl(new ControlOne(), string.Empty);
    }

    private void ControlTwo_Click(object sender, RoutedEventArgs e)
    {
      LoadControl(new ControlTwo(), string.Empty);
    }

    void _navHandler_Navigate(object sender, StateEventArgs e)
    {
      if (!string.IsNullOrEmpty(System.Windows.Browser.HtmlPage.Window.CurrentBookmark))
      {
        string controlId = HttpUtility.UrlDecode(System.Windows.Browser.HtmlPage.Window.CurrentBookmark).Substring(System.Windows.Browser.HtmlPage.Window.CurrentBookmark.IndexOf("=") + 1);
        if (_visitedControls.ContainsKey(controlId))
          LoadControl((UserControl)Activator.CreateInstance(Type.GetType(_visitedControls[controlId])), controlId);

      }
      else
      {
        this.PlaceHolder.Content = null;
      }
    }

    private void LoadControl(UserControl control, string bookMarkId)
    {
      ((App)App.Current).NavHandler.RaiseEvents = false;
      _currentControl = control;
      this.PlaceHolder.Content = _currentControl;
      if (string.IsNullOrEmpty(bookMarkId))
      {
        string typeName = _currentControl.GetType().AssemblyQualifiedName;
        string id = _nextId.ToString();
        _nextId += 1;
        _visitedControls.Add(id, typeName);

        System.Windows.Browser.BrowserInformation info = System.Windows.Browser.HtmlPage.BrowserInformation;
        if (info.Name.ToUpper().Contains("IE"))
          System.Windows.Browser.HtmlPage.Window.CurrentBookmark = id;
        string script = "Sys.Application.addHistoryPoint({" + NavigationHandler.BookmarkKey + ": ";
        script = script + "\"" + id + "\"";
        script = script + "} , \"";
        script = script + typeName;
        script = script + "\");";
        System.Windows.Browser.HtmlPage.Window.Eval(script);
      }
      ((App)App.Current).NavHandler.RaiseEvents = true;
    }
  }
}
