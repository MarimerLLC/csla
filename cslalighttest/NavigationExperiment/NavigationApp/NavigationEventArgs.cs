using System;

namespace NavigationApp
{
  public class NavigationEventArgs : EventArgs
  {
    private string _controlTypeName;
    private int _bookmarkId;
    private string _parameters;
    public NavigationEventArgs(string controlTypeName, int bookmarkId, string parameters)
    {
      _controlTypeName = controlTypeName;
      _bookmarkId = bookmarkId;
      _parameters = parameters;
    }
    public string ControlTypeName
    {
      get { return _controlTypeName; }
    }
    public int BookmarkId
    {
      get { return _bookmarkId; }
    }

    public string Parameters
    {
      get { return _parameters; }
      set { _parameters = value; }
    }

  }
}
