using System;

namespace Csla.Silverlight
{
  /// <summary>
  /// This event arguments class is part of BeforeNavigation
  /// event of Navigator.
  /// </summary>
  public class NavigationEventArgs : EventArgs
  {
    private string _controlTypeName;
    private int _bookmarkId;
    private string _parameters;

    /// <summary>
    /// New instance of NavigationEventArgs.
    /// </summary>
    /// <param name="controlTypeName">
    /// Assembly qualified name of the control class to display.
    /// </param>
    /// <param name="bookmarkId">
    /// Id of the bookmark that will be added to browser history.
    /// </param>
    /// <param name="parameters">
    /// Parameters that will be passed to control.  Single string is passed
    /// in, and controls assumes responcibility for parsing it.
    /// </param>
    public NavigationEventArgs(string controlTypeName, int bookmarkId, string parameters)
    {
      _controlTypeName = controlTypeName;
      _bookmarkId = bookmarkId;
      _parameters = parameters;
    }

    /// <summary>
    /// Assembly qualified name of the control class to display.
    /// </summary>
    public string ControlTypeName
    {
      get { return _controlTypeName; }
    }

    /// <summary>
    /// Id of the bookmark that will be added to browser history.
    /// </summary>
    public int BookmarkId
    {
      get { return _bookmarkId; }
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

  }
}
