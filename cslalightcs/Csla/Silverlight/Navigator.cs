using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;

namespace Csla.Silverlight
{
  /// <summary>
  /// Class that shows control in a specified container in
  /// responce to other controls' events.
  /// It is implemented using Singleton pattern, ensuring 
  /// one instance per application.
  /// </summary>
  public class Navigator
  {
    #region Private Variables
    private Dictionary<int, BoomarkInformation> _bookmarks;
    private int _nextBookmarkId = 1;
    private static Navigator _current = new Navigator();
    private bool _processBrowserEvents = false;
    private string _orignalTitle = string.Empty;

    private const string BookmarkKey = "bookmark";
    #endregion

    #region Constructor
    private Navigator()
    {
      _bookmarks = new Dictionary<int, BoomarkInformation>();
    }
    #endregion

    #region Events

    /// <summary>
    /// Event that occurs priot to showing of a target control
    /// </summary>
    public event EventHandler<NavigationEventArgs> BeforeNavigation;

    /// <summary>
    /// Event that occurs after control is shown
    /// </summary>
    public event EventHandler AfterNavigation;

    /// <summary>
    /// Event that occurs after a bookark has been retrived and processed
    /// in response to browser's back or forward buttons.
    /// </summary>
    public event EventHandler<NavigationBookmarkProcessedEventArgs> AfterBookmarkProcessing;

    /// <summary>
    /// Method that raises BeforeNavigation event.
    /// </summary>
    /// <param name="args">
    /// Event argeuments for the event.
    /// </param>
    protected void OnBeforeNavigation(NavigationEventArgs args)
    {
      if (BeforeNavigation != null)
      {
        BeforeNavigation(this, args);

      }
    }

    /// <summary>
    /// Method that raises AfterNavigation event.
    /// </summary>
    /// <param name="args">
    /// Event argeuments for the event.
    /// </param>
    protected void OnAfterNavigation()
    {
      if (AfterNavigation != null)
      {
        AfterNavigation(this, EventArgs.Empty);

      }
    }

    /// <summary>
    /// Method that raises AfterBookmarkProcessing event.
    /// </summary>
    /// <param name="args">
    /// Event argeuments for the event.
    /// </param>
    protected void OnAfterBookmarkProcessing(int bookmarkId, bool success)
    {
      if (AfterBookmarkProcessing != null)
      {
        AfterBookmarkProcessing(this, new NavigationBookmarkProcessedEventArgs(bookmarkId, success));
      }
    }

    /// <summary>
    /// Invoked by the browser
    /// </summary>
    /// <param name="state">State of the browser.</param>
    [ScriptableMember]
    public void HandleNavigate(ScriptObject state)
    {
      if (_processBrowserEvents)
        Navigate();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Register navigatgor with the browser in order to subscribe
    /// to history events.  This methos assumes that 
    /// Silverlight control that is embedded in the web page
    /// has Id of Xaml1
    /// </summary>
    public void RegisterNavigator()
    {
      RegisterNavigator("Xaml1");
    }

    /// <summary>
    /// Register navigatgor with the browser in order to subscribe
    /// to history events.
    /// </summary>
    /// <param name="silverlightControlNameInWebPage">
    /// Name of the Silverlight control that is embedded in the web page.
    /// Default value is Xaml1
    /// </param>
    public void RegisterNavigator(string silverlightControlNameInWebPage)
    {
      try
      {
        _processBrowserEvents = false;
        object title = HtmlPage.Window.Eval("document.title");
        if (title != null)
          _orignalTitle = title.ToString();

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

    /// <summary>
    /// Get the current instance of navigator for the application.
    /// </summary>
    public static Navigator Current
    {
      get { return _current; }
    }

    /// <summary>
    /// ContentControl in which the desired control will be shown.
    /// </summary>
    public ContentControl ContentPlaceholder { get; set; }

    /// <summary>
    /// Show specified control in ContentControl
    /// specified by ContentPlaceholder property.
    /// </summary>
    /// <param name="controlTypeName">
    /// Assembly qualified control name to show.
    /// </param>
    public void Navigate(string controlTypeName)
    {
      int bookMarkId = GetBookmarkId();
      NavigationEventArgs args = new NavigationEventArgs(controlTypeName, bookMarkId, string.Empty);
      OnBeforeNavigation(args);
      // get user modifed value
      string title = string.Empty;
      CreateAndShowControl(bookMarkId, controlTypeName, args.Parameters, title, true);
      OnAfterNavigation();
    }

    private int GetBookmarkId()
    {
      int returnValue = _nextBookmarkId;
      _nextBookmarkId += 1;
      return returnValue;
    }

    private void CreateAndShowControl(int bookMarkId, string controlTypeName, string parameters, string title, bool addBookmark)
    {
      Control newControl = null;
      if (!string.IsNullOrEmpty(controlTypeName))
      {
        newControl = (Control)Activator.CreateInstance(Type.GetType(controlTypeName));
        ISupportNavigation navigatable = newControl as ISupportNavigation;
        if (navigatable != null)
        {
          navigatable.SetParameters(parameters);
          if (addBookmark)
            title = navigatable.Title;
        }
      }
      SetTitle(title);
      if (addBookmark)
      {
        _bookmarks.Add(bookMarkId, new BoomarkInformation(controlTypeName, parameters, title));
        AddBookmark(bookMarkId, title);
      }

      ContentPlaceholder.Content = newControl;
    }

    private void SetTitle(string title)
    {
      string script = string.Concat("document.title ='", title, "'");
      System.Windows.Browser.HtmlPage.Window.Eval(script);
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
        string bookmark = HttpUtility.UrlDecode(System.Windows.Browser.HtmlPage.Window.CurrentBookmark).Substring(System.Windows.Browser.HtmlPage.Window.CurrentBookmark.LastIndexOf("=") + 1);

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
        SetTitle(_orignalTitle);
        OnAfterBookmarkProcessing(bookmarkId, false);
      }
    }

    #endregion
  }
}
