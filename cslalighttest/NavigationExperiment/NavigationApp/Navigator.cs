using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;

namespace NavigationApp
{
  public class Navigator
  {
    #region Private Variables
    private Dictionary<int, BoomarkInformation> _bookmarks;
    private int _nextBookmarkId = 1;
    private static Navigator _current = new Navigator();
    private bool _processBrowserEvents = false;

    private const string BookmarkKey = "bookmark";
    #endregion

    #region Constructor
    private Navigator()
    {
      _bookmarks = new Dictionary<int, BoomarkInformation>();
    }
    #endregion

    #region Events

    public event EventHandler<NavigationEventArgs> BeforeNavigation;

    public event EventHandler AfterNavigation;

    public event EventHandler<NavigationBookmarkProcessedEventArgs>  AfterBookmarkProcessing;

    protected void OnBeforeNavigation(NavigationEventArgs args)
    {
      if (BeforeNavigation != null)
      {
        BeforeNavigation(this, args);

      }
    }

    protected void OnAfterNavigation()
    {
      if (AfterNavigation != null)
      {
        AfterNavigation(this, EventArgs.Empty);

      }
    }

    protected void OnAfterBookmarkProcessing(int bookmarkId, bool success)
    {
      if (AfterBookmarkProcessing != null)
      {
        AfterBookmarkProcessing(this, new NavigationBookmarkProcessedEventArgs(bookmarkId, success));
      }
    }

    [ScriptableMember]
    public void HandleNavigate(ScriptObject state)
    {
      if (_processBrowserEvents)
        Navigate(); 
    }

    #endregion

    #region Initialization

    public void RegisterNavigator()
    {
      RegisterNavigator("Xaml1");
    }

    public void RegisterNavigator(string silverlightControlNameInWebPage)
    {
      try
      {
        _processBrowserEvents = false;

        HtmlPage.RegisterScriptableObject("Navigator", this);

        string initScript = @" 
          var __navigateHandler = new Function('obj','args','document.getElementById(\'Xaml1\').Content.Navigator.HandleNavigate(args.get_state())');
          Sys.Application.add_navigate(__navigateHandler);
          __navigateHandler(this, new Sys.HistoryEventArgs(Sys.Application._state));".Replace("Xaml1", silverlightControlNameInWebPage);
        HtmlPage.Window.Eval(initScript);
      }
      finally
      {
        _processBrowserEvents = true;
      }
    }

    #endregion

    #region Navigation

    public bool RaiseEvents { get; set; }


    public static Navigator Current
    {
      get { return _current; }
    }

    public ContentControl ContentPlaceholder { get; set; }

    public void Navigate(string controlTypeName)
    {
      int bookMarkId = _nextBookmarkId;
      _nextBookmarkId += 1;
      NavigationEventArgs args = new NavigationEventArgs(controlTypeName, bookMarkId, string.Empty);
      OnBeforeNavigation(args);
      // get user modifed value
      string title = string.Empty;
      CreateAndShowControl(bookMarkId, controlTypeName, args.Parameters, title, true);
      OnAfterNavigation();
    }

    private void CreateAndShowControl(int bookMarkId, string controlTypeName, string parameters, string title, bool addBookmark)
    {
      Control newControl = (Control)Activator.CreateInstance(Type.GetType(controlTypeName));
      ISupportsNavigation navigatable = newControl as ISupportsNavigation;
      if (navigatable != null)
      {
        navigatable.SetParameters(parameters);
        title = navigatable.Title;
      }
      ContentPlaceholder.Content = newControl;
      if (addBookmark)
      {
        _bookmarks.Add(bookMarkId, new BoomarkInformation(controlTypeName, parameters, title));
        AddBookmark(bookMarkId, title);
      }
      System.Windows.Browser.HtmlPage.Document.SetProperty("Title", title);
    }

    private void AddBookmark(int bookmarkId, string title)
    {
      try
      {
        _processBrowserEvents = false;
        System.Windows.Browser.BrowserInformation info = System.Windows.Browser.HtmlPage.BrowserInformation;
        if (info.Name.ToUpper().Contains("IE"))
          System.Windows.Browser.HtmlPage.Window.CurrentBookmark = bookmarkId.ToString();
        string script = "Sys.Application.addHistoryPoint({" + Navigator.BookmarkKey + ": ";
        script = script + "\"" + bookmarkId + "\"";
        script = script + "} , \"";
        script = script + title;
        script = script + "\");";
        System.Windows.Browser.HtmlPage.Window.Eval(script);
      }
      finally
      {
        _processBrowserEvents = true;
      }
    }

    private void Navigate()
    {
      int bookmarkId = -1;
      if (!string.IsNullOrEmpty(System.Windows.Browser.HtmlPage.Window.CurrentBookmark))
      {
        string bookmark = HttpUtility.UrlDecode(System.Windows.Browser.HtmlPage.Window.CurrentBookmark).Substring(System.Windows.Browser.HtmlPage.Window.CurrentBookmark.IndexOf("=") + 1);

        if (int.TryParse(bookmark, out bookmarkId))
        {
          if (_bookmarks.ContainsKey(bookmarkId))
          {
            BoomarkInformation info = _bookmarks[bookmarkId];
            CreateAndShowControl(bookmarkId, info.ControlTypeName, info.Parameters, info.Title, false);
            OnAfterBookmarkProcessing(bookmarkId, true);
          }
        }
      }
      if (bookmarkId == -1)
      {
        ContentPlaceholder.Content = null;
        OnAfterBookmarkProcessing(bookmarkId, false);
      }
    }

    #endregion
  }
}
