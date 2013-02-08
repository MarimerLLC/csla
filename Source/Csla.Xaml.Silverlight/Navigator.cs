//-----------------------------------------------------------------------
// <copyright file="Navigator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Class that shows control in a specified container in</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;

namespace Csla.Xaml
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
    private static Navigator _current = new Navigator();
    private bool _processBrowserEvents = false;
    private string _orignalTitle = string.Empty;
    private IControlNameFactory _controlNameFactory;
    private string _bookmarkPartsSeparator = ";";
    private string _bookmarkKey = "bookmark";

    /// <summary>
    /// Default bookmark key
    /// </summary>
    public string BookmarkKey
    {
      get { return _bookmarkKey; }
      set { _bookmarkKey = value; }
    }

    /// <summary>
    /// Default bookmakr parts (control name, title, parameters) separator
    /// </summary>
    public string BookmarkPartsSeparator
    {
      get { return _bookmarkPartsSeparator; }
      set { _bookmarkPartsSeparator = value; }
    }
    #endregion

    #region Constructor
    private Navigator()
    { }
    #endregion

    #region Properties

    /// <summary>
    /// Control name factory object
    /// </summary>
    public IControlNameFactory ControlNameFactory
    {
      get
      {
        if (_controlNameFactory == null)
          _controlNameFactory = new ControlNameFactory();
        return _controlNameFactory;
      }
      set
      {
        _controlNameFactory = value;
      }
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
    /// <param name="success">
    /// Indicates if bookmakr was successfully processed
    /// </param>
    protected void OnAfterBookmarkProcessing(bool success)
    {
      if (AfterBookmarkProcessing != null)
      {
        AfterBookmarkProcessing(this, new NavigationBookmarkProcessedEventArgs(success));
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
    /// <param name="controlNameFactory">
    /// Control name factory to be used with Navigator
    /// </param>
    public void RegisterNavigator(string silverlightControlNameInWebPage, IControlNameFactory controlNameFactory)
    {
      _controlNameFactory = controlNameFactory;
      RegisterNavigator(silverlightControlNameInWebPage);
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
      Navigate(controlTypeName, string.Empty);
    }

    /// <summary>
    /// Show specified control in ContentControl
    /// specified by ContentPlaceholder property.
    /// </summary>
    /// <param name="controlTypeName">
    /// Assembly qualified control name to show.
    /// </param>
    /// <param name="paramerers">
    /// String that incorporates parameters needed for new control
    /// </param>
    public void Navigate(string controlTypeName, string paramerers)
    {
      NavigationEventArgs args = new NavigationEventArgs(controlTypeName, paramerers, false);
      OnBeforeNavigation(args);
      if (!args.Cancel)
      {
        string title = string.Empty;
        CreateAndShowControl((Control)Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(controlTypeName)), args.Parameters, title, true, false);

        OnAfterNavigation();
      }
      else
      {
        if (args.RedirectToOnCancel != null)
        {
          CreateAndShowControl((Control)Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(args.RedirectToOnCancel.ControlTypeName)), args.RedirectToOnCancel.Parameters, args.RedirectToOnCancel.Title, true, true);
        }
      }
    }

    /// <summary>
    /// Call this after setting up content placeholder in order to 
    /// handle cold navigation to a bookrmak via Favorites features
    /// for example.
    /// </summary>
    public void ProcessInitialNavigation()
    {
      string currentAddress = System.Windows.Browser.HtmlPage.Document.DocumentUri.AbsoluteUri.ToString();
      if (currentAddress.Contains(_bookmarkKey))
      {
        string bookmark = HttpUtility.UrlDecode(currentAddress).Substring(currentAddress.IndexOf(_bookmarkKey) + _bookmarkKey.Length + 1);
        ParseAndProcessBookmark(bookmark);
      }
    }

    private void CreateAndShowControl(Control newControl, string parameters, string title, bool addBookmark, bool isRedirected)
    {
      bool hasBookmarkBeenAdded = false;
      if (!(newControl == null))
      {
        ISupportNavigation navigatable = newControl as ISupportNavigation;
        if (navigatable != null)
        {
          navigatable.SetParameters(parameters);
          if (addBookmark && !isRedirected)
            title = navigatable.Title;

          if (navigatable.CreateBookmarkAfterLoadCompleted)
          {
            hasBookmarkBeenAdded = true;
            navigatable.LoadCompleted += (o, e) =>
              {
                title = navigatable.Title;
                SetTitle(title);
                if (addBookmark)
                {
                  AddBookmark(title, ControlNameFactory.ControlToControlName(newControl), parameters);
                }
              };
          }
        }
        if (!hasBookmarkBeenAdded)
        {
          SetTitle(title);
          if (addBookmark)
          {
            AddBookmark(title, ControlNameFactory.ControlToControlName(newControl), parameters);
          }
        }


        ContentPlaceholder.Content = newControl;
      }

    }

    private void SetTitle(string title)
    {
      string script = string.Concat("document.title ='", title, "'");
      System.Windows.Browser.HtmlPage.Window.Eval(script);
    }


    private void AddBookmark(string title, string controlName, string parameters)
    {
      try
      {
        _processBrowserEvents = false;
        string addHistoryScript =
          "Sys.Application.addHistoryPoint({{ {0}:'{1}' }}, '{2}');";
        string script = string.Format(addHistoryScript, _bookmarkKey, controlName + _bookmarkPartsSeparator + title + _bookmarkPartsSeparator + parameters + _bookmarkPartsSeparator, title);
        System.Windows.Browser.HtmlPage.Window.Eval(script);
      }
      finally
      {
        _processBrowserEvents = true;
      }
    }

    private void Navigate()
    {
      bool success = false;
      if (!string.IsNullOrEmpty(System.Windows.Browser.HtmlPage.Window.CurrentBookmark))
      {
        string bookmark = HttpUtility.UrlDecode(System.Windows.Browser.HtmlPage.Window.CurrentBookmark);
        bookmark = bookmark.Substring(bookmark.IndexOf(_bookmarkKey) + _bookmarkKey.Length + 1);
        success = ParseAndProcessBookmark(bookmark);
      }
      if (success == false)
      {
        ContentPlaceholder.Content = null;
        SetTitle(_orignalTitle);
        OnAfterBookmarkProcessing(false);
      }
    }

    private bool ParseAndProcessBookmark(string bookmark)
    {
      bool success = false;
      if (bookmark.Length > 0 && bookmark.Contains(_bookmarkPartsSeparator))
      {
        string[] bookmarkParts = bookmark.Split((new string[] { _bookmarkPartsSeparator }), StringSplitOptions.None);
        if (bookmarkParts.Length >= 3)
        {
          string controlName = bookmarkParts[0];
          string title = bookmarkParts[1];
          string parameters = bookmarkParts[2];
          Control newControl = ControlNameFactory.ControlNameToControl(controlName);

          if (newControl != null)
          {
            NavigationEventArgs args = new NavigationEventArgs(newControl.GetType().AssemblyQualifiedName, parameters, true);
            OnBeforeNavigation(args);
            if (!args.Cancel)
            {
              CreateAndShowControl(newControl, args.Parameters, title, false, false);
              OnAfterBookmarkProcessing(true);
              success = true;
            }
            else
            {
              if (args.RedirectToOnCancel != null)
              {
                CreateAndShowControl((Control)Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(args.RedirectToOnCancel.ControlTypeName)), args.RedirectToOnCancel.Parameters, args.RedirectToOnCancel.Title, true, true);
                OnAfterBookmarkProcessing(true);
                success = true;
              }
            }
          }
        }
      }
      return success;
    }

    #endregion
  }
}