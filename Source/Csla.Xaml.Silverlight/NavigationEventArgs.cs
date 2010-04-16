using System;

namespace Csla.Xaml
{
  /// <summary>
  /// This event arguments class is part of BeforeNavigation
  /// event of Navigator.
  /// </summary>
  public class NavigationEventArgs : EventArgs
  {
    private string _controlTypeName;
    private string _parameters;
    private bool _cancel = false;
    private bool _isInitiatedByBrowserButton = false;
    private BookmarkInformation _redirectToOnCancel;

    /// <summary>
    /// New instance of NavigationEventArgs.
    /// </summary>
    /// <param name="controlTypeName">
    /// Assembly qualified name of the control class to display.
    /// </param>
    /// <param name="parameters">
    /// Parameters that will be passed to control.  Single string is passed
    /// in, and controls assumes responcibility for parsing it.
    /// </param>
    /// <param name="isInitiatedByBrowserButton">
    /// Flag indicating whether the action was initiated by a
    /// browser button.
    /// </param>
    public NavigationEventArgs(string controlTypeName, string parameters, bool isInitiatedByBrowserButton)
    {
      _controlTypeName = controlTypeName;
      _parameters = parameters;
      _isInitiatedByBrowserButton = isInitiatedByBrowserButton;
    }

    /// <summary>
    /// Assembly qualified name of the control class to display.
    /// </summary>
    public string ControlTypeName
    {
      get { return _controlTypeName; }
    }

    /// <summary>
    /// Parameters that will be passed to control.  Single string is passed
    /// in, and controls assumes responcibility for parsing it.
    /// You can modify this parameter string as needed.
    /// </summary>
    public string Parameters
    {
      get { return _parameters; }
      set { _parameters = value; }
    }

    /// <summary>
    /// Set to True if you want to cancel from BeforeNavigation event
    /// </summary>
    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    /// <summary>
    /// If navigation is cancelled, optionaly redirect to the
    /// control specified in this property.
    /// </summary>
    public BookmarkInformation RedirectToOnCancel
    {
      get { return _redirectToOnCancel; }
      set { _redirectToOnCancel = value; }
    }

    /// <summary>
    /// If true, the action was intiated by the user clicking on 
    /// a broser's back or forward button.  Otherwise, it was manual
    /// navigatio via direct invoke of Navigate method of Navigator
    /// </summary>
    public bool IsInitiatedByBrowserButton
    {
      get { return _isInitiatedByBrowserButton; }
    }
  }
}
